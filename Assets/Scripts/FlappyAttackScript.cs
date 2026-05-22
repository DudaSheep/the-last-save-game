using System.Collections;
using UnityEngine;

public class FlappyAttackScript : MonoBehaviour
{
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator anim;
    private PipeSpawner spawner;

    [Header("Configurações do Ataque")]
    public float dashSpeed = 25f;        
    public float prepareTime = 1f; // Tempo que ele passa irritado se preparando

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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        spawner = FindObjectOfType<PipeSpawner>();
    }

    // Este método público será chamado pelo PipeSpawner quando o tempo acabar
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

        // PASSO 3: O flappy entra no modo irritado e se prepara para atacar
        rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("irritado", true); 
        }

        // Espera o tempo configurado na fase de preparação
        yield return new WaitForSeconds(prepareTime);

        // PASSO 4: Os pipes param de spawnar (desliga logo antes do golpe)
        if (spawner != null)
        {
            spawner.isSpawning = false;
        }

        // Prepara para o ataque mudando a animação
        if (anim != null)
        {
            anim.SetBool("irritado", false);
            anim.SetBool("investindo", true);
        }

        // Salva a posição do player para o bote
        Vector2 posicaoDaMorte = Vector2.zero;
        if (playerTransform != null)
        {
            posicaoDaMorte = playerTransform.position;
        }

        // LÓGICA DO FLIP (Transform)
        if (posicaoDaMorte.x > transform.position.x)
        {
            transform.localScale = spriteOriginalOlhaEsquerda ? new Vector3(-escalaOriginal.x, escalaOriginal.y, escalaOriginal.z) : escalaOriginal;
        }
        else
        {
            transform.localScale = spriteOriginalOlhaEsquerda ? escalaOriginal : new Vector3(-escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        }

        Vector2 direcao = (posicaoDaMorte - (Vector2)transform.position).normalized;
        rb.velocity = direcao * dashSpeed;

        yield return new WaitForSeconds(0.8f);

        // Fim do ataque, desliga a animação e reseta o flip
        rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("investindo", false);
        }
        transform.localScale = escalaOriginal; 

        // Retorno suave para a posição inicial
        yield return new WaitForSeconds(0.3f); 
        float tempoDeRetorno = 0f;
        Vector3 posicaoAtual = transform.position;

        while (tempoDeRetorno < 1f)
        {
            tempoDeRetorno += Time.deltaTime * 2f; 
            transform.position = Vector3.Lerp(posicaoAtual, posicaoInicial, tempoDeRetorno);
            yield return null;
        }

        transform.position = posicaoInicial;

        
    }
}