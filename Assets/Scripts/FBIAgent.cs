using System.Collections;
using UnityEngine;

public class FBIAgent : MonoBehaviour
{
    [Header("Movimentação")]
    public float speed = 3f;
    public float shootingDistance = 5f; 
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

    [Header("Sons")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        //SÓ SE MOVE se estiver longe do player E não estiver recarregando
        if (distanceToPlayer > shootingDistance && !isReloading)
        {
            transform.Translate(new Vector3(direction * speed * Time.deltaTime, 0, 0));
            
            //ativa a animação de caminhar
            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            //parou para atirar ou recarregar -> Desliga a animação de caminhar
            if (animator != null) animator.SetBool("isWalking", false);
        }
        
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
            
            if (player == null) 
            {
                yield break; 
            }
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            //se precisar recarregar
            if (shotsFired >= maxShots)
            {
                isReloading = true;
                if (animator != null) animator.SetBool("isReloading", true);

                yield return new WaitForSeconds(reloadTime);

                shotsFired = 0;
                isReloading = false;
                if (animator != null) animator.SetBool("isReloading", false);
            }

            //só tenta atirar se estiver perto o suficiente e NÃO estiver recarregando
            if (!isReloading && distanceToPlayer <= shootingDistance)
            {
                Shoot();
                yield return new WaitForSeconds(fireRate);
            }
            else
            {
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

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            
            if (bulletScript != null)
            {
                Vector2 shotDirection = (transform.localScale.x < 0) ? Vector2.right : Vector2.left;
            
                bulletScript.SetDirection(shotDirection);
            }

            shotsFired++;
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;
        
        isDead = true;
        
        StageManager.Instance.AgentDefeated();

        Destroy(gameObject); 
    }

}