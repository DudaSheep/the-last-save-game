using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{

    // public GameObject efeitoDestruicaoPrefab; // TODO

    void Start()
    {
        // Destrói a estaca automaticamente após 5 segundos se ela não bater no chão antes disso
        Destroy(gameObject, 5f);
    }

    // private void OnCollisionEnter2D(Collision2D colisor)
    // {
    //     // Verifica colisao com o solo
    //     // (Você pode verificar pelo Nome do objeto ou usando uma Tag "Chao" / "Solo")
    //     if (colisor.gameObject.name.Contains("Tilemap") || colisor.gameObject.CompareTag("Ground"))
    //     {
    //         // Se tiver um efeito visual:
    //         // if (efeitoDestruicaoPrefab != null) { Instantiate(efeitoDestruicaoPrefab, transform.position, transform.rotation); }

    //         Destroy(gameObject);
    //     }
    //     else if (colisor.gameObject.CompareTag("Player"))
    //     {
    //         // Opcional: Se o script de vida do seu Player já tiver uma função de tomar dano, 
    //         // você chamaria ela bem aqui antes de destruir a estaca, por exemplo:
    //         // colisor.gameObject.GetComponent<PlayerHealth>().TomarDano(1);

    //         Destroy(gameObject);
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D colisor)
    {
        // Verifica colisao com o solo (Tilemap ou objeto com a Tag Ground)
        if (colisor.gameObject.name.Contains("Tilemap") || colisor.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        // Verifica colisao com a Dona Morte
        else if (colisor.gameObject.CompareTag("Player"))
        {
            Debug.Log("bateu na morte");

            PlayerHealth playerHealth = colisor.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = colisor.gameObject.GetComponentInParent<PlayerHealth>();
            }

            // Se encontrou o script de vida, aplica 1 de dano
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            // Destroi o espinho logo apos dar o dano para nao dar dano multiplo
            Destroy(gameObject);
        }
    }


}
