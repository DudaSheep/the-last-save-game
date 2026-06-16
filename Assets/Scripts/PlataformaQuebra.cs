using System.Collections;
using UnityEngine;

public class PlataformaQuebra : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisor;
    
    private Vector3 posicaoOriginal;
    private bool jaCaiu = false;

    [Header("Configurações de Alerta")]
    public Color corDeAlerta = Color.red;
    public float tempoDeAviso = 0.6f;

    [Header("Configurações de Retorno")]
    [Tooltip("Quanto tempo a plataforma demora para reaparecer após cair (em segundos)")]
    public float tempoParaRetornar = 4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisor = GetComponent<Collider2D>();
        
        //salva a posição da plataforma
        posicaoOriginal = transform.position;
    }

    public bool EstaDisponivel()
    {
        return !jaCaiu;
    }

    public void IniciarQueda()
    {
        if (!jaCaiu && rb != null)
        {
            jaCaiu = true;
            StartCoroutine(SequenciaAlertaEQueda());
        }
    }

    IEnumerator SequenciaAlertaEQueda()
    {
        Color corOriginal = spriteRenderer.color;
        float tempoPorPisca = 0.15f;
        float cronometro = 0f;

        while (cronometro < tempoDeAviso)
        {
            spriteRenderer.color = corDeAlerta;
            yield return new WaitForSeconds(tempoPorPisca);
            cronometro += tempoPorPisca;

            spriteRenderer.color = corOriginal;
            yield return new WaitForSeconds(tempoPorPisca);
            cronometro += tempoPorPisca;
        }

        spriteRenderer.color = corDeAlerta;

        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(1.5f);

        spriteRenderer.enabled = false; 
        colisor.enabled = false;        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;     

        yield return new WaitForSeconds(tempoParaRetornar);

        transform.position = posicaoOriginal; 
        transform.rotation = Quaternion.identity;
        spriteRenderer.color = corOriginal;   
        spriteRenderer.enabled = true;        
        colisor.enabled = true;               
        
        jaCaiu = false; 
    }
}