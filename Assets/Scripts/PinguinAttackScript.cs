using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinguinAttackScript : MonoBehaviour
{
    [Header("Recursos de Teste")]
    [Tooltip("Marque esta caixa no modo Play para simular a queda do Chefe e ativar a chuva de estacas!")]
    public bool simulateLandTrigger = false;

    [Header("Configurações de Ataque")]
    [Tooltip("Duração da chuva de estacas (spikes) em segundos")]
    public float spikeAttackDuration = 10f;

    [Header("Fase 1: Torres de Escudo")]
    [Tooltip("Número total de torres de conexão que protegem o Chefe")]
    public int totalTowersCount = 1; // Ajustado para 1 conforme sua decisão de escopo!
    private int destroyedTowersCount = 0;
    private bool isVulnerable = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // SIMULAÇÃO MANUAL PARA TESTAR A CHUVA DE SPIKES
        if (simulateLandTrigger)
        {
            TriggerSpikeAttack();
            simulateLandTrigger = false; // Desmarca automaticamente
        }
    }

    // --- FUNÇÃO CHAMADA PELAS TORRES QUANDO ELAS SÃO DESTRUÍDAS ---
    public void ReportTowerDestroyed()
    {
        destroyedTowersCount++;
        Debug.Log("Destruição de torre reportada ao Chefe! Progresso: " + destroyedTowersCount + "/" + totalTowersCount);

        if (destroyedTowersCount >= totalTowersCount)
        {
            SetBossVulnerable();
        }
    }

    private void SetBossVulnerable()
    {
        isVulnerable = true;
        Debug.Log("⚡ O CHEFE PINGUIM AGORA ESTÁ VULNERÁVEL! ⚡");

        // --- AÇÃO DA FASE 2: FAZER O CHEFE CAIR DA PLATAFORMA ---
        if (rb != null)
        {
            // Altera o Rigidbody para Dynamic para que a gravidade puxe o Triângulo para o chão naturalmente
            rb.bodyType = RigidbodyType2D.Dynamic;
            Debug.Log("Rigidbody do Chefe agora é Dynamic! Caindo para o chão da arena...");
        }
    }

    // GATILHO DE COLISÃO REAL (Quando o chefe atinge o chão)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto colidido é o chão principal da arena
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.name.Contains("Tilemap"))
        {
            // O chefe só causa o terremoto de estacas se cair no chão
            TriggerSpikeAttack();
        }
    }

    // CONECTA COM O SPIKE SPAWNER DO TETO
    private void TriggerSpikeAttack()
    {
        SpikeSpawner spawner = FindObjectOfType<SpikeSpawner>();

        if (spawner != null)
        {
            Debug.Log("Chefe bateu no chão! Ativando chuva de spikes por " + spikeAttackDuration + " segundos.");
            spawner.AtivarChuvaDeSpikes(spikeAttackDuration);
        }
        else
        {
            Debug.LogWarning("SpikeSpawner não foi encontrado na cena! Certifique-se de que o script está anexado a um objeto.");
        }
    }

    // Getter publico para checar se o jogador pode causar dano ao chefe
    public bool IsVulnerable()
    {
        return isVulnerable;
    }
}
