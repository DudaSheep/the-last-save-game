using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinguinAttackScript : MonoBehaviour
{
    [Header("Configurações Gerais de Ataque")]
    [Tooltip("Duração da chuva de estacas (spikes) em segundos após o pinguim cair no chão")]
    public float spikeAttackDuration = 6f;

    [Header("Fase 1: Estátua (Invulnerável)")]
    [Tooltip("Número total de torres que precisam ser destruídas")]
    public int totalTowersCount = 3; 
    private int destroyedTowersCount = 0;
    
    [Tooltip("Tempo mínimo para tentar um pulo na Fase 1")]
    public float phase1MinJumpInterval = 4f;
    [Tooltip("Tempo máximo para tentar um pulo na Fase 1")]
    public float phase1MaxJumpInterval = 8f;
    [Tooltip("Força do pulo na Fase 1")]
    public float phase1JumpForce = 12f;

    [Header("Fase 2: Colorido (Vulnerável)")]
    
    [Tooltip("Prefab do objeto com o script GroundShock que vai aparecer no impacto")]
    public GameObject shockwavePrefab;
    
    [Tooltip("Arraste aqui o objeto vazio (Spawn Point) que fica no pé do pinguim")]
    public Transform shockwaveSpawnPoint; 
    
    [Tooltip("Tempo mínimo para tentar um pulo na Fase 2")]
    public float phase2MinJumpInterval = 2f;
    
    [Tooltip("Tempo máximo para tentar um pulo na Fase 2")]
    public float phase2MaxJumpInterval = 5f;
    
    [Tooltip("Força do pulo na Fase 2")]
    public float phase2JumpForce = 10f;

    private bool isVulnerable = false;
    private bool isMidJumpAttack = false;

    private Rigidbody2D rb;
    private BossHealth healthScript;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthScript = GetComponent<BossHealth>(); 

        // Configuração Inicial - Fase 1 (Invulnerável)
        if (healthScript != null)
        {
            healthScript.podeTomarDano = false; 
            Debug.Log("🛡️ Boss Pinguim iniciou como estátua (invulnerável).");
        }

        if (anim != null)
        {
            anim.SetBool("podeTomarDano", false); 
        }

        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;

        StartCoroutine(Phase1JumpAttackRoutine());
    }

    IEnumerator Phase1JumpAttackRoutine()
    {
        while (!isVulnerable)
        {
            // Trava de Cutscene
            if (StageManager.Instance != null && StageManager.Instance.isCutsceneActive)
            {
                yield return null;
                continue;
            }

            float randomWait = Random.Range(phase1MinJumpInterval, phase1MaxJumpInterval);
            yield return new WaitForSeconds(randomWait);

            if (!isVulnerable && !isMidJumpAttack)
            {
                Debug.Log("🎲 Boss Pinguim (Fase 1): Pulo para gerar estacas!");
                isMidJumpAttack = true;

                if (rb != null)
                {
                    rb.velocity = new Vector2(0, phase1JumpForce);
                }
            }
        }
    }

    IEnumerator Phase2JumpAttackRoutine()
    {
        while (isVulnerable)
        {
            float randomWait = Random.Range(phase2MinJumpInterval, phase2MaxJumpInterval);
            yield return new WaitForSeconds(randomWait);

            if (isVulnerable && !isMidJumpAttack)
            {
                Debug.Log("🎲 Boss Pinguim (Fase 2): Pulo agressivo!");
                isMidJumpAttack = true;

                if (rb != null)
                {
                    rb.velocity = new Vector2(0, phase2JumpForce);
                }
            }
        }
    }

    public void ReportTowerDestroyed()
    {
        destroyedTowersCount++;
        Debug.Log($"Torre destruída! Ondas concluídas: {destroyedTowersCount}/{totalTowersCount}");

        if (destroyedTowersCount >= totalTowersCount)
        {
            SetBossVulnerable();
        }
    }

    private void SetBossVulnerable()
    {
        isVulnerable = true;
        isMidJumpAttack = false;
        
        // Corta a rotina da Fase 1
        StopAllCoroutines(); 

        Debug.Log("⚡ TODAS AS TORRES CAÍRAM! PINGUIM AGORA ESTÁ VULNERÁVEL... ⚡");

        if (healthScript != null)
        {
            healthScript.podeTomarDano = true;
        }

        if (anim != null)
        {
            anim.SetBool("podeTomarDano", true); 
        }

        // Inicia a rotina de ataques agressivos da Fase 2
        StartCoroutine(Phase2JumpAttackRoutine());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detecta quando o pinguim volta para o chão após um pulo (Fase 1 ou Fase 2)
        if (isMidJumpAttack)
        {
            if (collision.gameObject.name.Contains("Tilemap") || collision.gameObject.CompareTag("Ground"))
            {
                isMidJumpAttack = false;
                
                if (!isVulnerable)
                {
                    Debug.Log("💥 Impacto no chão (Fase 1)! Ativando apenas estacas.");
                    TriggerSpikeAttack();
                }
                else
                {
                    Debug.Log("💥 Impacto no chão (Fase 2)! Estacas + Onda de Choque.");
                    TriggerSpikeAttack();
                    TriggerShockwave();
                }
            }
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

    private void TriggerShockwave()
    {
        if (shockwavePrefab != null)
        {
            Vector3 spawnPosition = shockwaveSpawnPoint != null ? shockwaveSpawnPoint.position : transform.position;
            
            Instantiate(shockwavePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("O prefab da Onda de Choque não foi atribuído no Inspector!");
        }
    }
}