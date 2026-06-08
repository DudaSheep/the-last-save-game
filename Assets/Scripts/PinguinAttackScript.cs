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
    public float spikeAttackDuration = 6f;

    [Header("Mecânica: Pulo na Plataforma")]
    [Tooltip("Tempo mínimo para tentar um pulo surpresa")]
    public float minJumpInterval = 4f;
    [Tooltip("Tempo máximo para tentar um pulo surpresa")]
    public float maxJumpInterval = 8f;
    [Tooltip("Força do pulo do pinguim (para cima) na própria plataforma")]
    public float jumpForce = 12f;

    [Header("Fase de Queda (Vulnerável)")]
    [Tooltip("Velocidade com que o chefe desce até o chão ao ficar vulnerável")]
    public float fallSpeed = 10f;
    [Tooltip("A altura (Y) exata do chão da sua arena onde o pinguim deve pousar")]
    public float groundYPosition = -3.37f; // Atualizado com o valor exato do seu Inspector!

    [Header("Fase 1: Torres de Escudo")]
    [Tooltip("Número total de torres sequenciais nesta fase")]
    public int totalTowersCount = 3; // 3 torres em ondas
    private int destroyedTowersCount = 0;
    private bool isVulnerable = false;
    private bool isMidJumpAttack = false;
    private bool isFallingToGround = false;

    private Rigidbody2D rb;
    private Collider2D bossCollider;
    private Vector2 platformPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();
        platformPosition = transform.position; // Salva a posição inicial na plataforma

        // Começa como Kinematic para ignorar qualquer força externa indesejada
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(RandomJumpAttackRoutine());
    }

    void Update()
    {
        if (simulateLandTrigger)
        {
            TriggerSpikeAttack();
            simulateLandTrigger = false;
        }

        // SISTEMA DE MOVIMENTAÇÃO DIRETA: Força o pinguim a descer sem depender da física da Unity
        if (isFallingToGround)
        {
            Vector3 currentPos = transform.position;
            currentPos.y -= fallSpeed * Time.deltaTime;

            // Se alcançou ou passou do chão (-3.37)
            if (currentPos.y <= groundYPosition)
            {
                currentPos.y = groundYPosition; // Fixa na posição exata do chão
                isFallingToGround = false;      // Desliga a descida

                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Kinematic; // Garante que continue imune a forças dos spikes
                    rb.velocity = Vector2.zero;
                }

                if (bossCollider != null)
                {
                    bossCollider.isTrigger = false; // Volta a colidir normalmente para o player bater nele
                }

                Debug.Log("⚡ O Chefe pousou perfeitamente no chão em Y: " + groundYPosition);
            }

            transform.position = currentPos;
        }
    }

    IEnumerator RandomJumpAttackRoutine()
    {
        while (!isVulnerable)
        {
            float randomWait = Random.Range(minJumpInterval, maxJumpInterval);
            yield return new WaitForSeconds(randomWait);

            // Só pula se não estiver vulnerável e não estiver caindo
            if (!isVulnerable && !isMidJumpAttack && !isFallingToGround)
            {
                Debug.Log("🎲 Boss Pinguim: Pulo de impacto na plataforma!");
                isMidJumpAttack = true;

                // Ativa temporariamente como Dynamic apenas para realizar o arco físico do pulo
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.velocity = new Vector2(0, jumpForce);
                }
            }
        }
    }

    public void ReportTowerDestroyed()
    {
        destroyedTowersCount++;
        Debug.Log("Torre destruída! Ondas concluídas: " + destroyedTowersCount + "/" + totalTowersCount);

        if (destroyedTowersCount >= totalTowersCount)
        {
            SetBossVulnerable();
        }
    }

    private void SetBossVulnerable()
    {
        isVulnerable = true;
        isMidJumpAttack = false;
        StopAllCoroutines(); // Corta imediatamente a rotina de pulos normais

        Debug.Log("⚡ TODAS AS TORRES CAÍRAM! FORÇANDO DESCIDA DIRETA ATÉ O CHÃO... ⚡");

        // Corta totalmente a física tradicional para o pinguim nunca ser isolado pelos Spikes
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
        }

        // Transforma o colisor em Trigger para que ele atravesse a plataforma vermelha sem colidir nela
        if (bossCollider != null)
        {
            bossCollider.isTrigger = true;
        }

        // Ativa a descida manual controlada pelo Update
        isFallingToGround = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Se estiver vulnerável ou caindo para o chão, o código do Update gerencia tudo. Ignora colisões antigas.
        if (isVulnerable || isFallingToGround) return;

        // PULO NORMAL DA FASE 1: Só colide se bater na plataforma de cima
        if (isMidJumpAttack && collision.gameObject.name.Contains("Platform"))
        {
            Debug.Log("💥 Impacto na plataforma! Ativando terremoto de estacas.");
            TriggerSpikeAttack();

            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = Vector2.zero;
            }

            transform.position = platformPosition;
            isMidJumpAttack = false;
        }
    }

    private void TriggerSpikeAttack()
    {
        SpikeSpawner spawner = FindObjectOfType<SpikeSpawner>();
        if (spawner != null)
        {
            spawner.AtivarChuvaDeSpikes(spikeAttackDuration);
        }
    }
}