using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Playables;

public class DialogueEmergency : MonoBehaviour
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

    private int indiceAtual = 0;
    private bool emTransicao = false;
    private CanvasGroup canvasGroupAtual; 
    private bool cutsceneFinalizada = false;

    void OnEnable() 
    {
        cutsceneFinalizada = false; 
        indiceAtual = 0;

        // Força a invisibilidade no momento zero
        if (canvasGroupEsquerda != null) canvasGroupEsquerda.alpha = 0f;
        if (canvasGroupDireita != null) canvasGroupDireita.alpha = 0f;

        // Já carrega o texto do quadro 0 na memória para não piscar vazio
        if (quadros.Length > 0) ConfigurarQuadro();

        StartCoroutine(IniciarSeguro());
    }

    IEnumerator IniciarSeguro()
    {
        yield return null; 

        if (quadros.Length > 0)
        {
            if (timelineDirector != null) timelineDirector.Pause(); 
            // Liga a caixa certa de primeira, sem fade
            if (canvasGroupAtual != null) canvasGroupAtual.alpha = 1f; 
        }
        else
        {
            FinalizarCutscene();
        }
    }

    void Update() 
    {
        if (canvasGroupAtual == null) return;
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

    void ConfigurarQuadro()
    {
        if (indiceAtual < 0 || indiceAtual >= quadros.Length) return;

        // Define qual é o Canvas atual e o texto
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
    }

    IEnumerator TransicaoDeQuadro()
    {
        emTransicao = true;
        
        // 1. Fade Out SEMPRE (Some o quadro atual)
        while (canvasGroupAtual.alpha > 0f)
        {
            canvasGroupAtual.alpha -= Time.unscaledDeltaTime * velocidadeDoFade;
            yield return null;
        }
        canvasGroupAtual.alpha = 0f;
        
        indiceAtual++;

        if (indiceAtual < quadros.Length)
        {
            // 2. Atualiza o texto e aponta para a nova caixa
            ConfigurarQuadro();

            // 3. Fade In SEMPRE (Aparece o novo quadro)
            while (canvasGroupAtual.alpha < 1f)
            {
                canvasGroupAtual.alpha += Time.unscaledDeltaTime * velocidadeDoFade;
                yield return null;
            }
            
            canvasGroupAtual.alpha = 1f; // Segurança para o visual ficar 100%
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
        if (timelineDirector != null) timelineDirector.Play();
    }
}