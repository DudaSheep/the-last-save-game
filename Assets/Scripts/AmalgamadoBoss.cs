using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

    private BossHealth bossHealth;
    private float maxHealthOriginal;

    private bool iniciouFase2 = false;
    private bool iniciouFase3 = false;

    private Color corDefinitivaDoBoss;

    [Header("Configurações de Balanceamento")]
    [Tooltip("Duração (em segundos) que o Boss ficará invulnerável ao mudar de fase")]
    public float tempoInvuneravelTransicao = 2.5f;


    [Header("Fase 1 - Configurações da Foice")]
    [Tooltip("Arraste aqui um GameObject vazio criado dentro do Boss, posicionado onde a foice bate")]
    public Transform hitPointFoice;
    [Tooltip("Raio da área de dano circular do golpe")]
    public float raioAtaqueFoice = 1.8f;
    [Tooltip("Quantidade de corações que esse golpe vai tirar do jogador")]
    public int danoFoice = 1;
    [Tooltip("Selecione a Layer do seu Player para que o Boss saiba quem atingir")]
    public LayerMask layerDoPlayer;

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

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            corDefinitivaDoBoss = spriteRenderer.color;
        }

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
            Debug.Log($"❄️ FASE 3: {healthPercentage:F1}% de vida restante! Ativando tempo de respiro.");

            // Ativa o escudo e para a sequência anterior para iniciar a transição limpa
            StopCoroutine("BossAISequence");
            StartCoroutine(RoutineRespiroTransicao(3));
        }
        else if (healthPercentage <= 70f && healthPercentage > 40f && !iniciouFase2)
        {
            iniciouFase2 = true;
            currentPhase = BossPhase.Phase2_Espinhos;
            Debug.Log($"🟥 FASE 2: {healthPercentage:F1}% de vida restante! Ativando tempo de respiro.");

            StartCoroutine(RoutineRespiroTransicao(2));
        }
    }

    // Coroutine responsável por dar o escudo de invulnerabilidade e o feedback visual
    IEnumerator RoutineRespiroTransicao(int proximaFase)
    {
        if (bossHealth != null) bossHealth.podeTomarDano = false;
        isAttacking = true; // Bloqueia ataques normais temporariamente

        LimparAtaquesCena();

        // Feedback Visual: Pisca o Boss meio transparente para indicar invulnerabilidade
        Color corOriginal = spriteRenderer != null ? spriteRenderer.color : Color.white;
        float transcorrido = 0f;

        while (transcorrido < tempoInvuneravelTransicao)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(corDefinitivaDoBoss.r, corDefinitivaDoBoss.g, corDefinitivaDoBoss.b, 0.4f);

            yield return new WaitForSeconds(0.15f);

            if (spriteRenderer != null)
                spriteRenderer.color = corDefinitivaDoBoss;

            yield return new WaitForSeconds(0.15f);
            transcorrido += 0.3f;
        }

        if (spriteRenderer != null) spriteRenderer.color = corDefinitivaDoBoss;
        // Configurações específicas pós-respiro de cada fase
        if (proximaFase == 2)
        {
            if (paredeEspinhosEsquerda != null) paredeEspinhosEsquerda.SetActive(true);
        }
        else if (proximaFase == 3)
        {
            if (spawnerFlappyBird != null) spawnerFlappyBird.SetActive(false);
            if (paredeEspinhosEsquerda != null) paredeEspinhosEsquerda.SetActive(false);

            // Inicia os loops de ataque da fase 3 após o descanso
            StartCoroutine(SequenciaAtaquesFase3());
        }

        // Libera o Boss para receber dano e agir novamente
        if (bossHealth != null) bossHealth.podeTomarDano = true;
        isAttacking = false;
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

    public void DispararDanoDaFoice()
    {
        if (hitPointFoice == null)
        {
            return;
        }

        Collider2D playerAtingido = Physics2D.OverlapCircle(hitPointFoice.position, raioAtaqueFoice, layerDoPlayer);

        if (playerAtingido != null)
        {
            PlayerHealth vida = playerAtingido.GetComponent<PlayerHealth>();

            if (vida == null)
            {
                vida = playerAtingido.GetComponentInParent<PlayerHealth>();
            }
            if (vida == null)
            {
                vida = playerAtingido.GetComponentInChildren<PlayerHealth>();
            }

            // Se encontrou o script de vida de qualquer uma das formas
            if (vida != null)
            {
                vida.TakeDamage(danoFoice);
            }
            else
            {
                Debug.LogWarning("⚠️ A foice colidiu com " + playerAtingido.name + " na camada Player, mas o script 'PlayerHealth' não foi encontrado em lugar nenhum desse objeto!");
            }
        }
    }

    IEnumerator AvisoEInvestida()
    {
        float timer = 0f;

        while (timer < tempoAvisoVermelho)
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            if (spriteRenderer != null) spriteRenderer.color = corDefinitivaDoBoss; // Usa a cor segura
            yield return new WaitForSeconds(0.2f);
            timer += 0.4f;
        }
        if (spriteRenderer != null) spriteRenderer.color = corDefinitivaDoBoss; // Garante o reset total

        if (anim != null) anim.SetTrigger("attack");

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

            // Evita disparar os mega ataques caso o boss esteja no meio da transição
            if (isAttacking) continue;

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
            SceneManager.LoadScene("MainMenu");
        }

        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (hitPointFoice != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPointFoice.position, raioAtaqueFoice);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}