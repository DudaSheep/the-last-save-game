using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    [Header("Configurações do Spike")]
    public GameObject spikePrefab;
    [Tooltip("Intervalo entre as quedas de spikes durante a chuva (ex: 0.4 para cair rápido)")]
    public float spawnRate = 0.4f;

    [Header("Variação Horizontal")]
    [Tooltip("O quanto a estaca pode desviar para a esquerda ou direita a partir do centro do Spawner")]
    public float widthOffset = 8f;

    // Controla se os spikes podem cair
    private bool podeSpawnar = false;


    void SpawnSpike()
    {
        // Calcula os limites esquerdo e direito baseados no widthOffset
        float lowestPointX = transform.position.x - widthOffset;
        float highestPointX = transform.position.x + widthOffset;

        // Sorteia uma posição X aleatória, mas mantém a altura Y fixa do Spawner
        Vector3 spawnPosition = new Vector3(Random.Range(lowestPointX, highestPointX), transform.position.y, 0);

        // Instancia o spike no teto fora da visao do jogador
        Instantiate(spikePrefab, spawnPosition, transform.rotation);
    }

    // --- Funcao chamada pelo Penguin Boss ---
    public void AtivarChuvaDeSpikes(float duracao)
    {
        if (!podeSpawnar)
        {
            StartCoroutine(RotinaChuvaSpikes(duracao));
        }
    }

    // Corrotina que controla o relogio interno
    private IEnumerator RotinaChuvaSpikes(float duracao)
    {
        podeSpawnar = true;
        float tempoPassado = 0f;
        float timerSpawn = 0f;

        // Fica em loop criando spikes ate atingir a duracao estipulada 
        while (tempoPassado < duracao)
        {
            tempoPassado += Time.deltaTime;
            timerSpawn += Time.deltaTime;

            // Controla o ritmo de criacaoo de cada spike dentro do loop
            if (timerSpawn >= spawnRate)
            {
                SpawnSpike();
                timerSpawn = 0f;
            }

            yield return null; // Aguarda o prox frame da Unity
        }

        podeSpawnar = false; // Desliga o spawner apos o termino do tempo
    }

    // Desenha uma linha visual na Scene para ajudar a enxergar a area de spawn
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 esquerda = new Vector3(transform.position.x - widthOffset, transform.position.y, 0);
        Vector3 direita = new Vector3(transform.position.x + widthOffset, transform.position.y, 0);
        Gizmos.DrawLine(esquerda, direita);
    }
}