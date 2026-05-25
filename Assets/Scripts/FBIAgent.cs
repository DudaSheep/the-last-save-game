using System.Collections;
using UnityEngine;

public class FBIAgent : MonoBehaviour
{
    [Header("Movimentação")]
    public float speed = 3f;
    public float shootingDistance = 5f; // Distância ideal para parar e atirar
    private Transform player;

    [Header("Combate")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.8f;
    public int maxShots = 3;         
    public float reloadTime = 2f;
    
    private int shotsFired = 0;
    private bool isReloading = false;
    private bool isDead = false;

    [Header("Animação")]
    private Animator animator;

    void Start()
    {
        // Pega o componente Animator anexado ao Agente do FBI
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Calcula a distância atual até a Dona Morte
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Define a direção (sinal) para onde o player está
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // SÓ SE MOVE se estiver longe do player E não estiver recarregando
        if (distanceToPlayer > shootingDistance && !isReloading)
        {
            transform.Translate(new Vector3(direction * speed * Time.deltaTime, 0, 0));
            
            // Ativa a animação de caminhar
            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            // Parou para atirar ou recarregar -> Desliga a animação de caminhar
            if (animator != null) animator.SetBool("isWalking", false);
        }
        
        // Mantém o flip do sprite olhando para a Dona Morte mesmo parado
        if (direction > 0) 
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction < 0) 
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    IEnumerator AttackRoutine()
    {
        while (!isDead)
        {
            if (player == null) yield return null;

            // Calcula a distância para saber se pode agir
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // 1. Se precisar recarregar
            if (shotsFired >= maxShots)
            {
                isReloading = true;
                if (animator != null) animator.SetBool("isReloading", true);

                yield return new WaitForSeconds(reloadTime);

                shotsFired = 0;
                isReloading = false;
                if (animator != null) animator.SetBool("isReloading", false);
            }

            // 2. Só tenta atirar se estiver perto o suficiente e NÃO estiver recarregando
            if (!isReloading && distanceToPlayer <= shootingDistance)
            {
                Shoot();
                yield return new WaitForSeconds(fireRate);
            }
            else
            {
                // Pequena espera para não travar o loop caso esteja longe do player
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void Shoot()
    {
        if (isReloading || isDead) return;

        if (bulletPrefab && firePoint)
        {
            if (animator != null) animator.SetTrigger("Shoot");

            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            shotsFired++;
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;
        
        isDead = true;
        
        StageManager.Instance.AgentDefeated();

        // Se você tiver animação de morte, pode dar um trigger nela aqui
        // e usar "Destroy(gameObject, tempoDaAnimacao)" em vez de sumir na hora.
        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PaDonaMorte"))
        {
            TakeDamage();
        }
    }
}