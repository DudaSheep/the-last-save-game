using System.Collections;
using UnityEngine;

public class FBIAgent : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pontoA;
    public Transform pontoB;
    public float patrolSpeed = 2f;
    private Transform pontoDestinoAtual;

    private float limiteEsquerdo;
    private float limiteDireito;

    [Header("Combate")]
    public float aggroDistance = 8f; // Distância que ele "enxerga" a personagem
    public float shootingDistance = 5f; 
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.8f;
    public int maxShots = 3;         
    public float reloadTime = 2f;
    
    private Transform player;
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

        pontoDestinoAtual = pontoA;

        // Calcula exatamente onde a zona começa e termina no mapa baseada nos pontos A e B
        if (pontoA != null && pontoB != null)
        {
            limiteEsquerdo = Mathf.Min(pontoA.position.x, pontoB.position.x);
            limiteDireito = Mathf.Max(pontoA.position.x, pontoB.position.x);
        }

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (StageManager.Instance != null && StageManager.Instance.isCutsceneActive) return;

        if (isDead) return;

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        bool playerDentroDaZona = false;
        if (player != null)
        {
            playerDentroDaZona = (player.position.x >= limiteEsquerdo && player.position.x <= limiteDireito);
        }

        // Só entra em combate se a Dona Morte estiver perto E dentro da zona dele
        if (distanceToPlayer <= aggroDistance && playerDentroDaZona)
        {
            ModoCombate(distanceToPlayer);
        }
        else
        {
            ModoPatrulha();
        }
    }

    void ModoPatrulha()
    {
        if (pontoA == null || pontoB == null) return; 

        if (animator != null) animator.SetBool("isWalking", true);

        transform.position = Vector2.MoveTowards(transform.position, pontoDestinoAtual.position, patrolSpeed * Time.deltaTime);

        float direction = Mathf.Sign(pontoDestinoAtual.position.x - transform.position.x);
        AjustarDirecaoSprite(direction);

        if (Mathf.Abs(transform.position.x - pontoDestinoAtual.position.x) < 0.2f)
        {
            pontoDestinoAtual = (pontoDestinoAtual == pontoA) ? pontoB : pontoA;
        }
    }

    void ModoCombate(float distanceToPlayer)
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        AjustarDirecaoSprite(direction);

        // Trava para o agente não sair correndo do mapa atrás dela
        bool fbiPodeAndar = (transform.position.x > limiteEsquerdo && transform.position.x < limiteDireito) || 
                            (transform.position.x <= limiteEsquerdo && direction > 0) || 
                            (transform.position.x >= limiteDireito && direction < 0);

        if (distanceToPlayer > shootingDistance && !isReloading && fbiPodeAndar)
        {
            transform.Translate(new Vector3(direction * patrolSpeed * Time.deltaTime, 0, 0));
            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            if (animator != null) animator.SetBool("isWalking", false);
        }
    }

    void AjustarDirecaoSprite(float direction)
    {
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
            if (StageManager.Instance != null && StageManager.Instance.isCutsceneActive)
            {
                yield return null; 
                continue; 
            }

            if (player == null) yield break;
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Checa a zona de novo para garantir que ele não atire se ela sair da área
            bool playerDentroDaZona = (player.position.x >= limiteEsquerdo && player.position.x <= limiteDireito);

            if (distanceToPlayer <= aggroDistance && playerDentroDaZona)
            {
                if (shotsFired >= maxShots)
                {
                    isReloading = true;
                    if (animator != null) animator.SetBool("isReloading", true);

                    yield return new WaitForSeconds(reloadTime);

                    shotsFired = 0;
                    isReloading = false;
                    if (animator != null) animator.SetBool("isReloading", false);
                }

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
            else
            {
                yield return new WaitForSeconds(0.5f);
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

        Destroy(gameObject); 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            if (pontoA != null && pontoB != null)
            {
                pontoDestinoAtual = (pontoDestinoAtual == pontoA) ? pontoB : pontoA;
            }
        }
    }
}