using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f; 
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = -transform.right * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se acertar a Dona Morte (certifique-se da tag "Player")
        if (collision.CompareTag("Player"))
        {
            // Chame o método de dano da Dona Morte aqui
            // Ex: collision.GetComponent<PlayerHealth>().TakeDamage(1);
            Debug.Log("Bala acertou a Dona Morte!");
            Destroy(gameObject);
        }

        // Destrói se acertar o chão/paredes (certifique-se da tag "Ground" ou similar)
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}