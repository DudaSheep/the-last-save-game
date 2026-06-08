using UnityEngine;

public class AreaDanoBoss : MonoBehaviour
{
    [SerializeField] private int quantidadeDeDano = 2;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("O Boss acertou a Morte com um golpe físico!");

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(quantidadeDeDano);
            }
        }
    }
}