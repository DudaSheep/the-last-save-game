using UnityEngine;

public class DeadZoneScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem colidiu é o Player ou parte dele
        if (collision.CompareTag("Player") || (collision.transform.parent != null && collision.transform.parent.CompareTag("Player")))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = collision.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth == null)
            {
                playerHealth = collision.GetComponentInChildren<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                Debug.Log("❤️ Dona morte levou muito dano!");
                playerHealth.TakeDamage(100);
            }
        }
    }
}