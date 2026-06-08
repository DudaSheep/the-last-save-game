using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuScript : MonoBehaviour
{
    [Header("Configurações de Cenas")]
    [SerializeField] private string level1SceneName = "Stage1_et";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // O botão "Tentar Novamente" chama isso:
    public void TentarNovamente()
    {
        // Reseta o score e as vidas estáticas de forma segura antes de carregar
        GameController.totalScore = 0;
        GameController.lives = 1;

        Time.timeScale = 1f; // Descongela o tempo do jogo
        SceneManager.LoadScene(level1SceneName);
    }

    // O botão "Menu Inicial" chama isso:
    public void VoltarParaOMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}