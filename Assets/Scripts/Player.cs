using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    public bool isJumping;
    public bool doubleJump;

    [Header("Progressão de Habilidades")]
    [Tooltip("Controla se o jogador já pode dar o pulo duplo")]
    public static bool liberouPuloDuplo = false; // Começa bloqueado. Fica como 'static' para persistir ou ser acessado facilmente.

    [Header("Configuração Especial: Club Penguin")]
    public bool estaNoGelo = false;
    [Tooltip("Quanto menor o valor, mais ela desliza (Ex: 1.5f desliza bastante, 5f para rápido)")]
    public float deslizamentoGelo = 2f;
    private float velocidadeHorizontalAtual = 0f;

    [Header("Configurações de Ataque")]
    public Transform attackPoint;
    [Tooltip("Alcance/Raio do golpe da pa")]
    public float attackRange = 0.5f;
    [Tooltip("Selecione a Layer que você criou para a torre (ex: Interactable)")]
    public LayerMask attackableLayers;

    [Header("Configurações do Dash (Botão J)")]
    [Tooltip("A distância/força do avanço rápido")]
    public float dashForce = 15f;
    [Tooltip("Quanto tempo ela passa sumida/invulnerável (Ex: 0.2 segundos)")]
    public float dashDuration = 0.2f;
    [Tooltip("Tempo de espera para poder usar o dash novamente")]
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer spriteRenderer; 
    private Collider2D playerCollider;    

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        playerCollider = GetComponent<Collider2D>();     
    }

    void Update()
    {
        if (isDashing) return;

        if (StageManager.Instance != null && StageManager.Instance.isCutsceneActive)
        {
            return; 
        }

        Move();
        Jump();
        CheckAttack();
        CheckDash(); 
    }

    void Move()
    {
        float inputHorizontal = Input.GetAxis("Horizontal");

        if (estaNoGelo)
        {
            if (inputHorizontal != 0)
            {
                velocidadeHorizontalAtual = Mathf.MoveTowards(velocidadeHorizontalAtual, inputHorizontal, Time.deltaTime * deslizamentoGelo * 2f);
            }
            else
            {
                velocidadeHorizontalAtual = Mathf.MoveTowards(velocidadeHorizontalAtual, 0f, Time.deltaTime * deslizamentoGelo);
            }
        }
        else
        {
            velocidadeHorizontalAtual = inputHorizontal;
        }

        Vector3 movement = new Vector3(velocidadeHorizontalAtual, 0f, 0f);
        transform.position += movement * Time.deltaTime * Speed;

        if (inputHorizontal > 0)
        {
            anim.SetBool("walking", true);
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (inputHorizontal < 0)
        {
            anim.SetBool("walking", true);
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else
        {
            if (Mathf.Abs(velocidadeHorizontalAtual) < 0.05f)
            {
                anim.SetBool("walking", false);
            }
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            // PULO SIMPLES: Funciona normalmente se o jogador estiver no chão
            if (!isJumping)
            {
                rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                doubleJump = true;
                anim.SetBool("jump", true);
            }
            // TENTATIVA DE PULO DUPLO: Só entra aqui se já estiver no ar
            else
            {
                // ADICIONADO: Só executa se 'doubleJump' for true E se a habilidade já foi liberada
                if (doubleJump && liberouPuloDuplo)
                {
                    rig.velocity = new Vector2(rig.velocity.x, 0f);
                    rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                    doubleJump = false;
                    anim.SetTrigger("jump");
                }
            }
        }
    }

    void CheckAttack()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        if (attackPoint == null) return;

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayers);

        foreach (Collider2D obj in hitObjects)
        {
            ConnectionTowerScript tower = obj.GetComponent<ConnectionTowerScript>();
            if (tower != null)
            {
                tower.TakeDamage();
            }

            FBIAgent fbiAgent = obj.GetComponent<FBIAgent>();
            if (fbiAgent != null)
            {
                fbiAgent.TakeDamage(); 
            }

            BossHealth bossHealth = obj.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(1); 
            }
        }
    }

    void CheckDash()
    {
        if (Input.GetKeyDown(KeyCode.J) && canDash)
        {
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        
        float originalGravity = rig.gravityScale;
        rig.gravityScale = 0f;

        float direcaoDash = (transform.eulerAngles.y == 180f) ? -1f : 1f;
        
        rig.velocity = new Vector2(direcaoDash * dashForce, 0f);

        if (spriteRenderer != null) spriteRenderer.enabled = false; 

        int layerPlayer = LayerMask.NameToLayer("Player");
        int layerInimigo = LayerMask.NameToLayer("Interactable");
        Physics2D.IgnoreLayerCollision(layerPlayer, layerInimigo, true);

        yield return new WaitForSeconds(dashDuration);

        rig.velocity = Vector2.zero;
        rig.gravityScale = originalGravity; 
        
        if (spriteRenderer != null) spriteRenderer.enabled = true;  

        Physics2D.IgnoreLayerCollision(layerPlayer, layerInimigo, false);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isJumping = false;
            anim.SetBool("jump", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isJumping = true;
        }
    }

    // --- FUNÇÃO PARA LIBERAR O PULO DUPLO ---
    // Chame isso a partir do script de morte do ET/Boss
    public static void AtivarPuloDuplo()
    {
        liberouPuloDuplo = true;
    }
}