using UnityEngine;
using System.Collections;

public class BossET : MonoBehaviour
{
    [Header("Configurações de Tempo")]
    public float tempoEntreCabeçadas = 3f;
    private float timerCabeçada;
    public float tempoEntreStomps = 10f;
    private float timerStomp;

    [Tooltip("Quantos segundos o ET espera parado antes de dar o primeiro ataque")]
    public float pausaInicioDaLuta = 3f;

    [Header("Referências do Player")]
    public Transform playerTransform;
    public float alturaLimiteY = -1f;

    [Header("Áreas de Dano (Objetos Filhos)")]
    public GameObject areaHeadbutt;
    public GameObject areaGroundHeadbutt;
    public float duracaoDoAtaque = 0.6f;

    [Header("Configurações de Movimento de Ataque")]
    [Tooltip("A distância que o ET vai dar o passo para frente antes de golpear")]
    public float distanciaDoPasso = 2f;
    [Tooltip("A velocidade com que ele avança e recua")]
    public float velocidadeDoPasso = 8f;
    
    private Vector3 posicaoCentro;
    private bool estaAtacandoEAndando = false;
    private Vector3 posicaoAlvoAtaque;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        timerCabeçada = tempoEntreCabeçadas + pausaInicioDaLuta;

        timerStomp = tempoEntreStomps + pausaInicioDaLuta;

        //salva onde o ET começa o jogo
        posicaoCentro = transform.position;

    }

    void Update()
    {
        if (playerTransform == null) return;
        
        OlharParaOPlayer();

        if (estaAtacandoEAndando) return;

        //CONTROLADOR DO STOMP
        timerStomp -= Time.deltaTime;
        if (timerStomp <= 0f)
        {
            AtaqueStomp();
            timerStomp = tempoEntreStomps;
            timerCabeçada = tempoEntreCabeçadas; 
            return; 
        }

        //CONTROLADOR DAS CABEÇADAS
        timerCabeçada -= Time.deltaTime;
        if (timerCabeçada <= 0f)
        {
            DecidirCabeçadaPorPosicao();
            timerCabeçada = tempoEntreCabeçadas;
        }
    }

    void OlharParaOPlayer()
    {
        Vector3 escalaAtual = transform.localScale;

        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(escalaAtual.x), escalaAtual.y, escalaAtual.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(escalaAtual.x), escalaAtual.y, escalaAtual.z);
        }
    }

    void DecidirCabeçadaPorPosicao()
    {
        if (playerTransform.position.y < alturaLimiteY)
        {
            StartCoroutine(RotinaPassoEAtaque(true));
        }
        else
        {
            StartCoroutine(RotinaPassoEAtaque(false));
        }
    }

    IEnumerator RotinaPassoEAtaque(bool ehCabeçadaBaixa)
    {
        estaAtacandoEAndando = true;

        float direcaoX = (transform.localScale.x < 0f) ? 1f : -1f;
        posicaoAlvoAtaque = posicaoCentro + new Vector3(direcaoX * distanciaDoPasso, 0f, 0f);

        //move o ET para frente
        while (Vector3.Distance(transform.position, posicaoAlvoAtaque) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicaoAlvoAtaque, velocidadeDoPasso * Time.deltaTime);
            yield return null;
        }
        transform.position = posicaoAlvoAtaque; 

        if (ehCabeçadaBaixa)
        {
            Debug.Log("ET avançou e usou: Ground Headbutt!");
            if (anim != null) anim.SetTrigger("GroundHeadbuttTrigger");
        }
        else
        {
            Debug.Log("ET avançou e usou: Headbutt Alto!");
            if (anim != null) anim.SetTrigger("HeadbuttTrigger");
        }

        yield return new WaitForSeconds(duracaoDoAtaque);

        //faz o ET voltar para o centro 
        while (Vector3.Distance(transform.position, posicaoCentro) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicaoCentro, velocidadeDoPasso * Time.deltaTime);
            yield return null;
        }
        transform.position = posicaoCentro;

        estaAtacandoEAndando = false; 
    }

    void AtaqueStomp()
    {
        Debug.Log("HORA DO STOMP! Sorteando plataforma...");
        if (anim != null) anim.SetTrigger("StompTrigger"); 

        PlataformaQuebra[] todasAsPlataformas = FindObjectsOfType<PlataformaQuebra>();
        System.Collections.Generic.List<PlataformaQuebra> disponiveis = new System.Collections.Generic.List<PlataformaQuebra>();
        
        foreach (PlataformaQuebra plat in todasAsPlataformas)
        {
            if (plat != null && plat.EstaDisponivel()) disponiveis.Add(plat);
        }

        if (disponiveis.Count > 0)
        {
            int indiceSorteado = Random.Range(0, disponiveis.Count);
            disponiveis[indiceSorteado].IniciarQueda();
        }
    }
}