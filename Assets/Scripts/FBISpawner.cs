using UnityEngine;

public class FBISpawner : MonoBehaviour
{
    public GameObject fbiAgentPrefab;
    public Transform spawnPoint; 
    
    [Header("Controle de Ondas")]
    public int totalAgentsToSpawn = 10;   
    public int maxAgentsOnScreen = 3;     
    public float spawnInterval = 2f;      

    private int agentsSpawnedSoFar = 0;   
    private float timer;

    void Start()
    {
        agentsSpawnedSoFar = 0;
        timer = spawnInterval; 
    }

    void Update()
    {
        if (agentsSpawnedSoFar >= totalAgentsToSpawn)
        {
            enabled = false;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            int currentActiveAgents = GameObject.FindGameObjectsWithTag("Enemy").Length; 

            if (currentActiveAgents < maxAgentsOnScreen)
            {
                SpawnAgent();
                timer = 0f;
            }
        }
    }

    void SpawnAgent()
    {
        if (spawnPoint == null || fbiAgentPrefab == null) return;

        Instantiate(fbiAgentPrefab, spawnPoint.position, Quaternion.identity);
        agentsSpawnedSoFar++;
        
        Debug.Log($"Agente {agentsSpawnedSoFar}/{totalAgentsToSpawn} enviado para o combate!");
    }
}