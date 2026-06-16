using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroCutscene : MonoBehaviour
{
    [System.Serializable]
    public struct CutsceneFrame
    {
        public Sprite imagemDaCena;
        [TextArea(3, 5)]
        public string textoDaCena;
    }

    [Header("Configuração dos Quadros")]
    [SerializeField] private CutsceneFrame[] quadros;
    [SerializeField] private string nomeDaProximaCena = "Stage1_et";
    [SerializeField] private float velocidadeDoFade = 1.5f; //quanto maior, mais rápido o efeito

    [Header("Referências da UI Componentes")]
    [SerializeField] private Image exibicaoImagem;
    [SerializeField] private TextMeshProUGUI exibicaoTexto;
    [SerializeField] private CanvasGroup canvasGroupDoConteudo; //controla a opacidade de tudo

    private int indiceAtual = 0;
    private bool emTransicao = false;

    void Start()
    {
        if (quadros.Length > 0 && canvasGroupDoConteudo != null)
        {
            ConfigurarQuadroInstantaneo();
        }
        else
        {
            if (canvasGroupDoConteudo == null) Debug.LogError("Falta arrastar o Canvas Group!");
            AvancarCena();
        }
    }

    void Update()
    {
        // Se o GameController existir e o jogo estiver pausado, não deixa avançar o texto
        if (GameController.instance != null && Time.timeScale == 0f) return;

        // Só avança se NÃO estiver no meio de uma transição de fade
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
        exibicaoImagem.sprite = quadros[indiceAtual].imagemDaCena;
        exibicaoTexto.text = quadros[indiceAtual].textoDaCena;
        canvasGroupDoConteudo.alpha = 1f; 
    }

    IEnumerator TransicaoDeQuadro()
    {
        emTransicao = true;

        //FADE OUT 
        while (canvasGroupDoConteudo.alpha > 0f)
        {
            canvasGroupDoConteudo.alpha -= Time.unscaledDeltaTime * velocidadeDoFade;
            yield return null;
        }
        canvasGroupDoConteudo.alpha = 0f;

        indiceAtual++;

        if (indiceAtual < quadros.Length)
        {
            exibicaoImagem.sprite = quadros[indiceAtual].imagemDaCena;
            exibicaoTexto.text = quadros[indiceAtual].textoDaCena;

            //FADE IN
            while (canvasGroupDoConteudo.alpha < 1f)
            {
                canvasGroupDoConteudo.alpha += Time.unscaledDeltaTime * velocidadeDoFade;
                yield return null;
            }
            canvasGroupDoConteudo.alpha = 1f;
            emTransicao = false;
        }
        else
        {
            AvancarCena();
        }
    }

    void AvancarCena()
    {
        SceneManager.LoadScene(nomeDaProximaCena);
    }
}