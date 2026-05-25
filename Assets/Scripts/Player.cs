using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    public bool isJumping;
    public bool doubleJump;

    [Header("Configuração Especial: Club Penguin")]
    public bool estaNoGelo = false;
    [Tooltip("Quanto menor o valor, mais ela desliza (Ex: 1.5f desliza bastante, 5f para rápido)")]
    public float deslizamentoGelo = 2f;
    private float velocidadeHorizontalAtual = 0f; // Controla a inercia do movimento

    [Header("Configurações de Ataque")]
    public Transform attackPoint;
    [Tooltip("Alcance/Raio do golpe da pa")]
    public float attackRange = 0.5f;
    [Tooltip("Selecione a Layer que você criou para a torre (ex: Interactable)")]
    public LayerMask attackableLayers;

    private Rigidbody2D rig;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        CheckAttack();
    }

    void Move()
    {
        float inputHorizontal = Input.GetAxis("Horizontal");

        // --- SISTEMA DE MOVIMENTO COM INÉRCIA (GELO VS NORMAL) ---
        if (estaNoGelo)
        {
            // Se o jogador está apertando para mover, ganha velocidade aos poucos
            if (inputHorizontal != 0)
            {
                velocidadeHorizontalAtual = Mathf.MoveTowards(velocidadeHorizontalAtual, inputHorizontal, Time.deltaTime * deslizamentoGelo * 2f);
            }
            // Se soltar o botão, perde velocidade bem devagar (o efeito de deslizar!)
            else
            {
                velocidadeHorizontalAtual = Mathf.MoveTowards(velocidadeHorizontalAtual, 0f, Time.deltaTime * deslizamentoGelo);
            }
        }
        else
        {
            // Nas outras fases, o controle continua direto, travado e instantâneo como era antes
            velocidadeHorizontalAtual = inputHorizontal;
        }

        // Aplica o movimento final usando a velocidade horizontal calculada
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
            if (!isJumping)
            {
                rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                doubleJump = true;
                anim.SetBool("jump", true);
            }
            else
            {
                if (doubleJump)
                {
                    rig.velocity = new Vector2(rig.velocity.x, 0f);

                    rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
                    doubleJump = false;
                    anim.SetTrigger("jump");
                }
            }

        }
    }


    // --- VERIFICA SE O JOGADOR APERTOU A TECLA K ---
    void CheckAttack()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Dispara o gatilho da animação de ataque no Animator
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        if (attackPoint == null) return;

        // Cria a esfera de colisão invisível para detectar se a torre está no alcance
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayers);

        // Varre os objetos atingidos procurando o script da torre
        foreach (Collider2D obj in hitObjects)
        {
            ConnectionTowerScript tower = obj.GetComponent<ConnectionTowerScript>();
            if (tower != null)
            {
                tower.TakeDamage();
            }
        }
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

}
