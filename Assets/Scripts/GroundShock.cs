using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundShock : MonoBehaviour
{
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

            // Se encontrou o script de vida, aplica 1 de dano
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

        }
    }
}
