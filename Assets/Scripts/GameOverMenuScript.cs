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
        Time.timeScale = 1f; // Descongela o tempo do jogo antes de carregar

        // 1. VERIFICA SE O PLAYER MORREU NO BOSS
        if (PlayerPrefs.HasKey("BossCheckpoint"))
        {
            int bossSceneIndex = PlayerPrefs.GetInt("BossCheckpoint");
            GameController.lives = 1;

            SceneManager.LoadScene(bossSceneIndex);
        }
        else
        {
            if (PlayerPrefs.HasKey("BossCheckpoint"))
            {
                PlayerPrefs.DeleteKey("BossCheckpoint");
                PlayerPrefs.Save();
            }

            GameController.totalScore = 0;
            GameController.lives = 1;

            SceneManager.LoadScene(level1SceneName);
        }
    }

    // O botão "Menu Inicial" chama isso:
    public void VoltarParaOMenu()
    {
        PlayerPrefs.DeleteKey("BossCheckpoint");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}