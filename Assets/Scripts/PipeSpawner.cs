using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float spawnRate = 2f; 
    private float timer = 0f; 
    public float heightOffset = 10f; 
    public bool isSpawning = true;

    [Header("Configuração de Tempo da Fase")]
    public float tempoAteAtaque = 60f; 
    private float cronometroFase = 0f;
    private bool flappyFoiAtivado = false;

    private FlappyAttackScript flappy;

    void Start()
    {
        flappy = FindObjectOfType<FlappyAttackScript>();

        if (isSpawning)
        {
            SpawnPipe(); 
        }
    }

    void Update()
    {
        if (!isSpawning) return; 

        if (cronometroFase < tempoAteAtaque)
        {
            cronometroFase += Time.deltaTime;
        }
        else if (!flappyFoiAtivado)
        {
            if (flappy != null)
            {
                flappy.IniciarAtaquePorTempo();
                flappyFoiAtivado = true; 
            }
        }

        if (timer < spawnRate)
        {
            timer += Time.deltaTime; 
        }
        else
        {
            SpawnPipe();
            timer = 0f; 
        }
    }

    void SpawnPipe()
    {
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;
        Instantiate(pipePrefab, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
    }

    public void ResetaCronometroAtaque()
    {
        cronometroFase = 0f;
        flappyFoiAtivado = false;
    }
}