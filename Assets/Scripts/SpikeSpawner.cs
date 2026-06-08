using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    [Header("Configurações do Spike")]
    public GameObject spikePrefab;
    [Tooltip("Intervalo entre as quedas de spikes durante a chuva (ex: 0.4 para cair rápido)")]
    public float spawnRate = 0.4f;

    [Header("Variação Horizontal (Baseada na Tela)")]
    [Tooltip("O quanto a estaca pode desviar para a esquerda ou direita a partir do CENTRO DA CÂMERA (Ex: 8f a 10f costuma cobrir a tela cheia)")]
    public float widthOffset = 9f;

    // Controla se os spikes podem cair
    private bool podeSpawnar = false;


    void SpawnSpike()
    {
        float centroX = transform.position.x;

        // Tenta encontrar a câmera principal do jogo dinamicamente
        if (Camera.main != null)
        {
            // O centro do ataque agora é a posição X atual da câmera que segue o player!
            centroX = Camera.main.transform.position.y >= -100 ? Camera.main.transform.position.x : transform.position.x;
        }

        // Calcula os limites baseando-se no centro dinâmico da câmera
        float lowestPointX = centroX - widthOffset;
        float highestPointX = centroX + widthOffset;

        // Sorteia uma posição X aleatória na tela, mantendo a altura Y fixa do seu teto/Spawner
        Vector3 spawnPosition = new Vector3(Random.Range(lowestPointX, highestPointX), transform.position.y, 0);

        // Instancia o spike no teto acompanhando a visão do jogador
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

    // Desenha uma linha visual na Scene baseada no objeto para ajuste inicial
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 esquerda = new Vector3(transform.position.x - widthOffset, transform.position.y, 0);
        Vector3 derecha = new Vector3(transform.position.x + widthOffset, transform.position.y, 0);
        Gizmos.DrawLine(esquerda, derecha);
    }
}