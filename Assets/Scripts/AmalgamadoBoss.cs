using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Adicionado para gerenciar a troca de cenas

public class AmalgamadoBoss : MonoBehaviour
{
    // Fases do Boss baseadas na evolução da batalha
    public enum BossPhase { Phase1_Foice, Phase2_Espinhos, Phase3_ClubPenguin, Dead }
    [Header("Estado Atual")]
    public BossPhase currentPhase = BossPhase.Phase1_Foice;

    [Header("Configurações Gerais de Ataque")]
    public float attackCooldown = 3.5f;
    private bool isAttacking = false;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;

    // Referência ao script de vida genérico
    private BossHealth bossHealth;
    private float maxHealthOriginal;

    // Interruptores de segurança para as transições estáveis
    private bool iniciouFase2 = false;
    private bool iniciouFase3 = false;

    [Header("Fase 2 - Configurações")]
    public GameObject paredeEspinhosEsquerda;
    public float forcaEmpurrao = 15f;
    public float tempoAvisoVermelho = 1.5f;
    [Tooltip("Duração (em segundos) que os canos vão continuar spawnando antes de parar")]
    public float tempoAtaqueCanos = 6.0f;
    [Tooltip("Arraste o objeto Pipe_Spawner_0 da cena aqui")]
    public GameObject spawnerFlappyBird;

    [Header("Fase 3 - Configurações")]
    [Tooltip("Arraste o objeto Spike_Spawner do teto da cena aqui")]
    public GameObject spawnerEstacasTeto;
    public float tempoChuvaEstacas = 2.0f;
    [Tooltip("Arraste o objeto GroundShock que já está posicionado na cena aqui")]
    public GameObject groundShockCena;
    public float tempoDuraçãoShock = 2.0f;
    public float tempoEntreEstacaERaio = 1.5f;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossHealth = GetComponent<BossHealth>();

        // Checkpint para Boss
        Debug.Log("boss checkpoint!!");
        PlayerPrefs.SetInt("BossCheckpoint", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();

        if (bossHealth != null)
        {
            maxHealthOriginal = bossHealth.currentHealth;
        }
        else
        {
            Debug.LogError("🚨 ERRO: O objeto precisa ter o script BossHealth adicionado a ele!");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (paredeEspinhosEsquerda != null) paredeEspinhosEsquerda.SetActive(false);
        if (spawnerFlappyBird != null) spawnerFlappyBird.SetActive(false);
        if (spawnerEstacasTeto != null) spawnerEstacasTeto.SetActive(false);
        if (groundShockCena != null) groundShockCena.SetActive(false);

        StartCoroutine(BossAISequence());
    }

    void Update()
    {
        if (bossHealth != null && currentPhase != BossPhase.Dead)
        {
            CheckPhaseTransition();
        }
    }

    private void CheckPhaseTransition()
    {
        float healthPercentage = ((float)bossHealth.currentHealth / maxHealthOriginal) * 100f;

        if (bossHealth.currentHealth <= 0)
        {
            currentPhase = BossPhase.Dead;
            StopAllCoroutines(); // Interrompe sequências de ataque ativas imediatamente

            Debug.Log("💀 Vida zerada! Disparando animação de morte controlada por Evento.");
            LimparAtaquesCena();

            if (anim != null)
            {
                anim.SetTrigger("death");
            }
        }
        else if (healthPercentage <= 40f && !iniciouFase3)
        {
            iniciouFase3 = true;
            currentPhase = BossPhase.Phase3_ClubPenguin;
            Debug.Log($"❄️ FASE 3: {healthPercentage:F1}% de vida restante! Iniciando sequência do Club Penguin.");

            if (spawnerFlappyBird != null) spawnerFlappyBird.SetActive(false);
            if (paredeEspinhosEsquerda != null) paredeEspinhosEsquerda.SetActive(false);

            StopCoroutine("BossAISequence");
            StartCoroutine(SequenciaAtaquesFase3());
        }
        else if (healthPercentage <= 70f && healthPercentage > 40f && !iniciouFase2)
        {
            iniciouFase2 = true;
            currentPhase = BossPhase.Phase2_Espinhos;
            Debug.Log($"🟥 FASE 2: {healthPercentage:F1}% de vida restante! Ativando Arena de Espinhos.");

            if (paredeEspinhosEsquerda != null) paredeEspinhosEsquerda.SetActive(true);
        }
    }

    IEnumerator BossAISequence()
    {
        while (currentPhase == BossPhase.Phase1_Foice || currentPhase == BossPhase.Phase2_Espinhos)
        {
            yield return new WaitForSeconds(attackCooldown);

            if (!isAttacking)
            {
                isAttacking = true;

                if (currentPhase == BossPhase.Phase1_Foice)
                {
                    AtaqueFoiceHorizontal();
                    yield return new WaitForSeconds(1.5f);
                }
                else if (currentPhase == BossPhase.Phase2_Espinhos)
                {
                    yield return StartCoroutine(ExecutarCicloFase2());
                }

                isAttacking = false;
            }
        }
    }

    IEnumerator ExecutarCicloFase2()
    {
        if (spawnerFlappyBird != null)
        {
            Debug.Log("🎈 Canos começando a surgir!");
            spawnerFlappyBird.SetActive(true);

            yield return new WaitForSeconds(tempoAtaqueCanos);

            spawnerFlappyBird.SetActive(false);
            Debug.Log("🛑 Canos pararam! Preparando investida.");
        }

        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(AvisoEInvestida());
    }

    private void AtaqueFoiceHorizontal()
    {
        Debug.Log("⚔️ Amalgamado atacou com a foice!");
        if (anim != null) anim.SetTrigger("attack");
    }

    IEnumerator AvisoEInvestida()
    {
        Debug.Log("🟥 Alerta Vermelho! O Boss vai avançar!");
        Color corOriginal = spriteRenderer.color;
        float timer = 0f;

        while (timer < tempoAvisoVermelho)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = corOriginal;
            yield return new WaitForSeconds(0.2f);
            timer += 0.4f;
        }
        spriteRenderer.color = corOriginal;

        if (anim != null) anim.SetTrigger("attack");
        Debug.Log("💥 INVESTIDA! Empurrando o jogador contra a parede esquerda!");

        if (playerTransform != null)
        {
            Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(-forcaEmpurrao, playerRb.velocity.y);
            }
        }
    }

    IEnumerator SequenciaAtaquesFase3()
    {
        while (currentPhase == BossPhase.Phase3_ClubPenguin)
        {
            yield return new WaitForSeconds(attackCooldown);

            if (spawnerEstacasTeto != null)
            {
                Debug.Log("📌 Passo 1: LIGANDO a chuva de estacas do céu!");
                spawnerEstacasTeto.SetActive(true);

                SpikeSpawner scriptSpawner = spawnerEstacasTeto.GetComponent<SpikeSpawner>();
                if (scriptSpawner != null)
                {
                    scriptSpawner.AtivarChuvaDeSpikes(tempoChuvaEstacas);
                }

                yield return new WaitForSeconds(tempoChuvaEstacas);
                spawnerEstacasTeto.SetActive(false);
            }

            yield return new WaitForSeconds(tempoEntreEstacaERaio);

            Debug.Log("⚡ Passo 2: LIGANDO o GroundShock da cena!");
            if (anim != null) anim.SetTrigger("attack");

            yield return new WaitForSeconds(1.0f);

            if (groundShockCena != null)
            {
                groundShockCena.SetActive(true);
                yield return new WaitForSeconds(tempoDuraçãoShock);

                groundShockCena.SetActive(false);
                Debug.Log("🛡️ GroundShock desligado! Fim do ciclo.");
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void LimparAtaquesCena()
    {
        if (spawnerFlappyBird != null) spawnerFlappyBird.SetActive(false);
        if (spawnerEstacasTeto != null) spawnerEstacasTeto.SetActive(false);
        if (groundShockCena != null) groundShockCena.SetActive(false);
    }


    public void FinalizarMorteDoBoss()
    {
        Debug.Log("🏆 Animação terminada! Mudando de cena com segurança.");

        PlayerPrefs.DeleteKey("BossCheckpoint");
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Não há mais fases no Build Settings! Retornando ao MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }

        // Desativa o objeto do Boss para evitar lógicas residuais antes de descarregar a cena completamente
        gameObject.SetActive(false);
    }
}