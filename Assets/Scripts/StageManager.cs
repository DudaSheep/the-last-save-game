using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Configurações da Fase")]
    public int agentsToDefeat = 10;
    private int currentDefeated = 0;

    public GameObject fbiSpawner;  
    public bool isCutsceneActive = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Invoke("StartFBIWave", 3f);
    }

    public void StartFBIWave()
    {
        isCutsceneActive = false;
        if (fbiSpawner != null) fbiSpawner.SetActive(true);
        Debug.Log("Começou a onda do FBI.");
    }

    public void AgentDefeated()
    {
        currentDefeated++;
        Debug.Log($"Agentes derrotados: {currentDefeated}/{agentsToDefeat}");

        if (currentDefeated >= agentsToDefeat)
        {
            EndFBIWave();
        }
    }

    void EndFBIWave()
    {
        if (fbiSpawner != null) fbiSpawner.SetActive(false);
        Debug.Log("Onda do FBI limpa!");
    
    }
}