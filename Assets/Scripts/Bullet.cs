using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    
    private Vector2 moveDirection = Vector2.left; 
    private bool foiDestruida = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;

        if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    

    void Update()
    {
        if (foiDestruida) return; 

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (foiDestruida) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            foiDestruida = true;
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            foiDestruida = true;
            Debug.Log("bateu na morte");

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            Destroy(gameObject);
        }
    }
}