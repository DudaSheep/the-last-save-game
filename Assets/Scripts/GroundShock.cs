using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundShock : MonoBehaviour
{
    [Header("Comportamento")]
    [Tooltip("Marque esta caixa se a onda foi gerada pelo pulo do chefe. DESMARQUE nos Servidores/Torres.")]
    public bool destroiAutomaticamente = true;
    
    [Tooltip("Tempo (em segundos) que a onda de choque fica na tela antes de sumir")]
    public float tempoDeVida = 1.0f;

    void Start()
    {
        // Só se autodestrói se a caixa estiver marcada (Onda do Chefe)
        if (destroiAutomaticamente)
        {
            // Se esta ondinha tem um objeto Pai, manda destruir o Pai (apagando o ataque inteiro)
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject, tempoDeVida);
            }
            else
            {
                // Backup de segurança: se não tiver pai, destrói só a si mesmo
                Destroy(gameObject, tempoDeVida);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que entrou no choque tem a Tag "Player"
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = collision.GetComponentInParent<PlayerHealth>();
            }

            // Se encontrou o script de vida, aplica o dano
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(2);
            }
        }
    }
}