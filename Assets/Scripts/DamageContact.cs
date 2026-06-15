using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageContact : MonoBehaviour
{
    [Header("Configurações de Dano")]
    [Tooltip("Quantidade de corações que o jogador perde ao encostar")]
    public int dano = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem encostou foi o Player usando a Layer ou a Tag
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = collision.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(dano);
                Debug.Log($"💥 {gameObject.name} causou dano por contato ao jogador!");
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth == null) playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(dano);
            }
        }
    }
}
