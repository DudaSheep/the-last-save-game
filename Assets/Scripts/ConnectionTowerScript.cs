using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionTowerScript : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int currentHealth = 3;

    [Header("Random Electricity Attack")]
    [Tooltip("Tempo mínimo (em segundos) para esperar antes de carregar o próximo choque")]
    public float minAttackInterval = 3f;
    [Tooltip("Tempo máximo (em segundos) para esperar antes de carregar o próximo choque")]
    public float maxAttackInterval = 7f;

    [Tooltip("Duração do raio ativo no chão")]
    public float shockDuration = 1.5f;
    private float currentRandomInterval;
    private float attackTimer = 0f;
    private bool isAttacking = false;

    [Header("Conexão do Escudo (Laser)")]
    public Transform bossTransform;
    private LineRenderer shieldLaser;

    [Header("Referência do Objeto Filho")]
    [Tooltip("Arraste aqui o objeto do Raio que está DENTRO/FILHO desta torre na Hierarquia")]
    public GameObject shockVisualChild;

    void Start()
    {
        shieldLaser = GetComponent<LineRenderer>();

        // Tenta achar o Transform do Boss automaticamente se não tiver sido arrastado no Inspector
        if (bossTransform == null)
        {
            PinguinAttackScript boss = FindObjectOfType<PinguinAttackScript>();
            if (boss != null) bossTransform = boss.transform;
        }

        if (shieldLaser != null && bossTransform != null)
        {
            shieldLaser.positionCount = 2;
            shieldLaser.startWidth = 0.1f;
            shieldLaser.endWidth = 0.1f;
            shieldLaser.enabled = true; // Garante que o laser começa ativo
        }

        // Garante que o raio começa desativado ao iniciar o jogo
        if (shockVisualChild != null)
        {
            shockVisualChild.SetActive(false);
        }

        SortNextAttackInterval();
    }

    void Update()
    {
        // Se o laser estiver ativo, ele segue a posição da torre atual até o chefe
        if (shieldLaser != null && bossTransform != null && shieldLaser.enabled)
        {
            shieldLaser.SetPosition(0, transform.position);
            shieldLaser.SetPosition(1, bossTransform.position);
        }

        if (!isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= currentRandomInterval)
            {
                StartCoroutine(ShockAttackRoutine());
                attackTimer = 0f;
            }
        }
    }

    private void SortNextAttackInterval()
    {
        currentRandomInterval = Random.Range(minAttackInterval, maxAttackInterval);
        Debug.Log(gameObject.name + " vai atacar em " + currentRandomInterval.ToString("F2") + " segundos.");
    }

    private IEnumerator ShockAttackRoutine()
    {
        isAttacking = true;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;

        if (sprite != null)
        {
            originalColor = sprite.color;
            sprite.color = Color.red; // Pisca vermelho indicando perigo na torre atual
        }

        Debug.Log(gameObject.name + " carregando eletricidade... CUIDADO!");
        yield return new WaitForSeconds(1.0f);

        if (sprite != null)
        {
            sprite.color = originalColor; // Volta à cor normal
        }

        // ATIVAÇÃO DO FILHO: Liga o raio que já está posicionado embaixo dela
        if (shockVisualChild != null)
        {
            shockVisualChild.SetActive(true);
            Debug.Log("RAIO ATIVADO ABAIXO DE: " + gameObject.name);
        }

        // Espera o tempo do raio ativo machucando o jogador
        yield return new WaitForSeconds(shockDuration);

        // DESATIVAÇÃO DO FILHO: Desliga o raio para a próxima rodada
        if (shockVisualChild != null)
        {
            shockVisualChild.SetActive(false);
        }

        SortNextAttackInterval();
        isAttacking = false;
    }

    public void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;
        Debug.Log(gameObject.name + " tomou dano! Vida restante: " + currentHealth);

        StartCoroutine(FlashDamageRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashDamageRoutine()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color originalColor = sprite.color;
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = originalColor;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " destruída com sucesso!");

        if (shieldLaser != null) shieldLaser.enabled = false;

        // Avisa o boss que UMA torre caiu (ele pode ir perdendo o escudo gradativamente se você quiser)
        PinguinAttackScript boss = FindObjectOfType<PinguinAttackScript>();
        if (boss != null)
        {
            boss.ReportTowerDestroyed();
        }

        // Garante que o raio não fique ligado flutuando se a torre quebrar no meio do ataque
        if (shockVisualChild != null) shockVisualChild.SetActive(false);

        // Apenas se destrói, sem spawnar nada no lugar!
        Destroy(gameObject);
    }
}