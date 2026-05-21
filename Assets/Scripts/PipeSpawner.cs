using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    // Referencia ao Pipe prefab que fizemos, onde vamos colocar o prefab
    public GameObject pipePrefab;
    public float spawnRate = 2f; // tempo entre os spawns dos pipes
    private float timer = 0f; // timer para controlar o spawn
    public float heightOffset = 10f; // offset para a altura do pipe

    // Start is called before the first frame update
    void Start()
    {
        SpawnPipe(); // spawn um pipe no inicio do jogo
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime; // incrementa o timer 
        }
        else
        {
            // cria um novo pipe e reseta o timer
            SpawnPipe();
            timer = 0f; // reseta o timer
        }
    }

    void SpawnPipe()
    {
        // pega o centro de shifta ele 10 pra baixo e 10 para cima
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;
        // gera um numero randomico entre o ponto mais baixo e o mais alto
        Instantiate(pipePrefab, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
    }
}
