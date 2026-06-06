using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionTowerScript : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int currentHealth = 3;

    [Header("Mecânica das Ondas (Spawn)")]
    [Tooltip("Arraste aqui o Prefab da PRÓXIMA torre que deve nascer quando esta cair. Deixe vazio na última torre!")]
    public GameObject nextTowerPrefab;

    [Header("Random Electricity Attack")]
    [Tooltip("Minimum time (in seconds) to wait before charging the next shock")]
    public float minAttackInterval = 3f;
    [Tooltip("Maximum time (in seconds) to wait before charging the next shock")]
    public float maxAttackInterval = 7f;

    [Tooltip("Duração do raio ativo no chão")]
    public float shockDuration = 1.5f;
    private float currentRandomInterval;
    private float attackTimer = 0f;
    private bool isAttacking = false;

    [Header("Conexão do Escudo (Laser)")]
    public Transform bossTransform;
    private LineRenderer shieldLaser;

    [Header("Referências de Prefabs (Opcional por enquanto)")]
    [Tooltip("Se tiver um prefab de efeito visual do raio ou Sprite do raio, coloque aqui")]
    public GameObject shockVisualPrefab;

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

        SortNextAttackInterval();
    }

    void Update()
    {
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
        Debug.Log("Next shock wave will trigger in " + currentRandomInterval.ToString("F2") + " seconds.");
    }

    private IEnumerator ShockAttackRoutine()
    {
        isAttacking = true;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;

        if (sprite != null)
        {
            originalColor = sprite.color;
            sprite.color = Color.red; // Pisca vermelho indicando perigo
        }

        Debug.Log("Tower charging electricity... WATCH OUT!");
        yield return new WaitForSeconds(1.0f);

        if (sprite != null)
        {
            sprite.color = originalColor; // Volta à cor normal da onda
        }

        // VARIÁVEL PARA GUARDAR O CLONE CRIADO
        GameObject spawnedShock = null;

        if (shockVisualPrefab != null)
        {
            float spawnX = -0.61f;
            float spawnY = 0.3f;

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, transform.position.z);

            // Instancia o prefab do choque na posição calibrada no chão
            spawnedShock = Instantiate(shockVisualPrefab, spawnPosition, Quaternion.identity);

            spawnedShock.SetActive(true);
        }

        Debug.Log("ELECTRICITY DISCHARGED ON THE GROUND!");

        // Espera o tempo do raio ativo machucando o jogador
        yield return new WaitForSeconds(shockDuration);

        if (spawnedShock != null)
        {
            // CORREÇÃO: Destrói o clone criado para não entupir a memória do jogo
            Destroy(spawnedShock);
        }

        SortNextAttackInterval();
        isAttacking = false;
    }

    public void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;
        Debug.Log("Tower took damage! Remaining health: " + currentHealth);

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
        Debug.Log("Connection Tower destroyed!");

        if (shieldLaser != null) shieldLaser.enabled = false;

        PinguinAttackScript boss = FindObjectOfType<PinguinAttackScript>();
        if (boss != null)
        {
            boss.ReportTowerDestroyed();
        }

        if (nextTowerPrefab != null)
        {
            // 1. Instancia a nova torre
            GameObject newTower = Instantiate(nextTowerPrefab, transform.position, transform.rotation);

            // 2. FORÇA A ORDEM VISUAL (Garante que ela não vá para trás do cenário)
            SpriteRenderer currentSprite = GetComponent<SpriteRenderer>();
            SpriteRenderer nextSprite = newTower.GetComponent<SpriteRenderer>();
            if (currentSprite != null && nextSprite != null)
            {
                nextSprite.sortingLayerName = currentSprite.sortingLayerName;
                nextSprite.sortingOrder = currentSprite.sortingOrder;
            }

            // 3. Mantém a ordem visual do Laser Renderer se houver
            LineRenderer nextLaser = newTower.GetComponent<LineRenderer>();
            if (shieldLaser != null && nextLaser != null)
            {
                nextLaser.sortingLayerName = shieldLaser.sortingLayerName;
                nextLaser.sortingOrder = shieldLaser.sortingOrder;
            }

            ConnectionTowerScript nextScript = newTower.GetComponent<ConnectionTowerScript>();
            if (nextScript != null && bossTransform != null)
            {
                nextScript.bossTransform = bossTransform;
            }
        }

        Destroy(gameObject);
    }
}