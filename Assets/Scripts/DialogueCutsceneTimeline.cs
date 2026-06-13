using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables; 
using System.Collections;

public class DialogueCutsceneTimeline : MonoBehaviour
{
    public enum LadoDoDialogo { Esquerda, Direita }

    [System.Serializable]
    public struct CutsceneFrame
    {
        public LadoDoDialogo quemFala; 
        [TextArea(3, 5)]
        public string textoDaCena;
    }

    [Header("Configuração da Timeline")]
    [SerializeField] private PlayableDirector timelineDirector; 

    [Header("Configuração dos Quadros")]
    [SerializeField] private CutsceneFrame[] quadros;
    [SerializeField] private float velocidadeDoFade = 1.5f;

    [Header("Referências da UI da Esquerda")]
    [SerializeField] private TextMeshProUGUI textoEsquerda;
    [SerializeField] private CanvasGroup canvasGroupEsquerda;

    [Header("Referências da UI da Direita")]
    [SerializeField] private TextMeshProUGUI textoDireita;
    [SerializeField] private CanvasGroup canvasGroupDireita;

    [Header("Física do Personagem")]
    [SerializeField] private Rigidbody2D rbPersonagem; 

    private int indiceAtual = 0;
    private bool emTransicao = false;
    private CanvasGroup canvasGroupAtual; 
    private bool cutsceneFinalizada = false;

    void OnEnable()
    {
        if (timelineDirector != null && timelineDirector.time < 5.9f) return; 

        cutsceneFinalizada = false; 

        if (rbPersonagem != null) rbPersonagem.bodyType = RigidbodyType2D.Kinematic;

        if (quadros.Length > 0)
        {
            if (timelineDirector != null) timelineDirector.Pause();

            indiceAtual = 0;
            
            if (canvasGroupEsquerda != null) canvasGroupEsquerda.alpha = 0f;
            if (canvasGroupDireita != null) canvasGroupDireita.alpha = 0f;

            ConfigurarQuadroInstantaneo();
        }
        else
        {
            FinalizarCutscene();
        }
    }

    void Update()
    {
        if (GameController.instance != null && Time.timeScale == 0f) return;

        if (cutsceneFinalizada) return;

        if (!emTransicao)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(TransicaoDeQuadro());
            }
        }
    }

    void ConfigurarQuadroInstantaneo()
    {
        // Descobre quem fala primeiro e ativa a caixa certa
        if (quadros[indiceAtual].quemFala == LadoDoDialogo.Esquerda)
        {
            textoEsquerda.text = quadros[indiceAtual].textoDaCena;
            canvasGroupEsquerda.alpha = 1f;
            canvasGroupAtual = canvasGroupEsquerda;
        }
        else
        {
            textoDireita.text = quadros[indiceAtual].textoDaCena;
            canvasGroupDireita.alpha = 1f;
            canvasGroupAtual = canvasGroupDireita;
        }
    }

    IEnumerator TransicaoDeQuadro()
    {
        emTransicao = true;

        while (canvasGroupAtual.alpha > 0f)
        {
            canvasGroupAtual.alpha -= Time.unscaledDeltaTime * velocidadeDoFade;
            yield return null;
        }
        canvasGroupAtual.alpha = 0f;

        indiceAtual++;

        if (indiceAtual < quadros.Length)
        {
            if (quadros[indiceAtual].quemFala == LadoDoDialogo.Esquerda)
            {
                textoEsquerda.text = quadros[indiceAtual].textoDaCena;
                canvasGroupAtual = canvasGroupEsquerda;
            }
            else
            {
                textoDireita.text = quadros[indiceAtual].textoDaCena;
                canvasGroupAtual = canvasGroupDireita;
            }

            while (canvasGroupAtual.alpha < 1f)
            {
                canvasGroupAtual.alpha += Time.unscaledDeltaTime * velocidadeDoFade;
                yield return null;
            }
            canvasGroupAtual.alpha = 1f;
            emTransicao = false;
        }
        else
        {
            FinalizarCutscene();
        }
    }

    void FinalizarCutscene()
    {
        cutsceneFinalizada = true;

        if (canvasGroupAtual != null) canvasGroupAtual.alpha = 0f;
        
        if (rbPersonagem != null) rbPersonagem.bodyType = RigidbodyType2D.Dynamic;

        if (timelineDirector != null) 
        {
            timelineDirector.Play();
        }
    }
    
}