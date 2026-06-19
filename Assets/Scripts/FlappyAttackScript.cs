using System.Collections;
using UnityEngine;

public class FlappyAttackScript : MonoBehaviour
{
    [Header("Referência do Player")]
    [Tooltip("Arraste a Dona Morte (Player) da hierarquia da cena direto para cá!")]
    public Transform playerTransform; 

    private Rigidbody2D rb;
    private Animator anim;
    private PipeSpawner spawner;

    [Header("Configurações do Ataque")]
    public float dashSpeed = 25f;        
    public float prepareTime = 1f; // Tempo que ele passa irritado se preparando
    [Tooltip("Tempo que os canos voltam a correr enquanto ele descansa na base")]
    public float tempoDeEsperaNaBase = 2.0f; 

    [Header("Configuração do Sprite")]
    public bool spriteOriginalOlhaEsquerda = true; 

    private Vector3 posicaoInicial;
    private bool isPreparing = false;
    private Vector3 escalaOriginal; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        posicaoInicial = transform.position;
        escalaOriginal = transform.localScale; 

        spawner = FindObjectOfType<PipeSpawner>();
    }

    public void IniciarAtaquePorTempo()
    {
        if (!isPreparing)
        {
            StartCoroutine(SequenciaAtaqueRoutine());
        }
    }

    IEnumerator SequenciaAtaqueRoutine()
    {
        isPreparing = true;

        while (true)
        {
            // BLINDAGEM: Se você esquecer de arrastar o player no inspetor, ele apenas espera para não dar erro
            if (playerTransform == null)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            // ----------------------------------------------------
            // ETAPA 1: TRAVA OS PIPES E FICA IRRITADO
            // ----------------------------------------------------
            if (spawner != null)
            {
                spawner.isSpawning = false; 
            }

            rb.velocity = Vector2.zero;
            if (anim != null)
            {
                anim.SetBool("investindo", false);
                anim.SetBool("irritado", true); 
            }

            yield return new WaitForSeconds(prepareTime);

            // ----------------------------------------------------
            // ETAPA 2: DASH (INVESTIDA)
            // ----------------------------------------------------
            if (anim != null)
            {
                anim.SetBool("irritado", false);
                anim.SetBool("investindo", true);
            }

            // Pega a posição puramente 2D (Ignora o eixo Z para evitar bugs na Build)
            Vector2 posicaoDaMorte = new Vector2(playerTransform.position.x, playerTransform.position.y);
            Vector2 posFlappy2D = new Vector2(transform.position.x, transform.position.y);

            if (posicaoDaMorte.x > transform.position.x)
            {
                transform.localScale = spriteOriginalOlhaEsquerda ? new Vector3(-escalaOriginal.x, escalaOriginal.y, escalaOriginal.z) : escalaOriginal;
            }
            else
            {
                transform.localScale = spriteOriginalOlhaEsquerda ? escalaOriginal : new Vector3(-escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
            }

            // Calcula a direção puramente no eixo X e Y
            Vector2 direcao = (posicaoDaMorte - posFlappy2D).normalized;
            rb.velocity = direcao * dashSpeed;

            yield return new WaitForSeconds(0.8f);

            // Freia após o avanço
            rb.velocity = Vector2.zero;
            if (anim != null)
            {
                anim.SetBool("investindo", false);
            }
            transform.localScale = escalaOriginal;

            // ----------------------------------------------------
            // ETAPA 3: RETORNO SUAVE PARA A BASE
            // ----------------------------------------------------
            yield return new WaitForSeconds(0.2f); 
            float tempoDeRetorno = 0f;
            Vector3 posicaoAtual = transform.position;

            while (tempoDeRetorno < 1f)
            {
                tempoDeRetorno += Time.deltaTime * 2.5f; 
                transform.position = Vector3.Lerp(posicaoAtual, posicaoInicial, tempoDeRetorno);
                yield return null;
            }

            transform.position = posicaoInicial;

            // ----------------------------------------------------
            // ETAPA 4: VOLTA A SPAWNAR OS PIPES ENQUANTO ELE DESCANSA
            // ----------------------------------------------------
            if (spawner != null)
            {
                spawner.isSpawning = true; 
            }

            // O Boss fica lá na base parado enquanto o jogador desvia dos canos neste intervalo
            yield return new WaitForSeconds(tempoDeEsperaNaBase);
        }
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.isSpawning = true;
        }
    }
}