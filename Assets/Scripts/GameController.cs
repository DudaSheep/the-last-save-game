using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //mainmenu e play
    [SerializeField] private string level1SceneName = "Stage1_et";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    //score
    public static int totalScore;
    public TextMeshProUGUI scoreText;

    //vida
    public static int lives = 1;
    public TextMeshProUGUI livesText;

    //sons
    public AudioSource bgMusic;
    public AudioClip playerHitSound;
    public AudioClip enemyDeathSound;

    //gameover e victory
    public GameObject gameOver;
    public GameObject victoryScreen;

    //pause
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused;

    public static GameController instance;

    void Awake()
    {
        //singleton simples
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Cutscene_Intro")
        {
            UpdateScoreText();
            UpdateLivesText();
        }

        if (bgMusic != null && !bgMusic.isPlaying)
        {
            bgMusic.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = totalScore.ToString();
        }
    }


    public void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = lives.ToString();
        }
    }

    public void LoseLife()
    {
        if (lives < 0 && Time.timeScale == 1f && SceneManager.GetActiveScene().name == "GameOver") return;

        PlayEffect(playerHitSound);
        lives--;
        UpdateLivesText();

        if (lives <= 0)
        {
            lives = 0;
            UpdateLivesText();

            if (bgMusic != null)
            {
                bgMusic.Stop();
            }

            CancelInvoke("RestartLevel");
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            Invoke("RestartLevel", 0.5f);
        }
    }

    public void GainLife()
    {
        lives++;
        UpdateLivesText();
    }

    public void Pause()
    {
        if (pauseMenu != null) 
        {
            pauseMenu.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;

        if (bgMusic != null)
        {
            bgMusic.Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenu != null) 
        {
            pauseMenu.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;

        if (bgMusic != null)
        {
            bgMusic.UnPause();
        }
    }

    public void ShowGameOver()
    {
        gameOver.SetActive(true);
        Time.timeScale = 0f;
        if (bgMusic != null) bgMusic.Stop();
    }

    public void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0f;
        if (bgMusic != null) bgMusic.Stop(); 
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayAgainFromStart()
    {
        totalScore = 0;
        lives = 1;
        Time.timeScale = 1f;
        SceneManager.LoadScene(level1SceneName);
    }

    public void PlayGame()
    {
        totalScore = 0;
        lives = 1;
        SceneManager.LoadScene(level1SceneName);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void PlayEffect(AudioClip clip)
    {
        if (clip != null && bgMusic != null)
        {
            bgMusic.PlayOneShot(clip);
        }
    }
}
