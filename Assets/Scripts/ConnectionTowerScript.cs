using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionTowerScript : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int currentHealth = 3;

    [Header("Random Electricity Attack")]
    [Tooltip("Minimum time (in seconds) to wait before charging the next shock")]
    public float minAttackInterval = 3f;
    [Tooltip("Maximum time (in seconds) to wait before charging the next shock")]
    public float maxAttackInterval = 7f;


    [Tooltip("Duração do raio ativo no chão")]
    public float shockDuration = 1.5f;
    private float currentRandomInterval; // Holds the current sorted wait time
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
        // Gets or configures the Line Renderer attached to the tower
        shieldLaser = GetComponent<LineRenderer>();
        if (shieldLaser != null && bossTransform != null)
        {
            shieldLaser.positionCount = 2;
            shieldLaser.startWidth = 0.1f;
            shieldLaser.endWidth = 0.1f;
        }

        // Sorts the first attack interval right at the start of the game
        SortNextAttackInterval();
    }

    void Update()
    {
        // 1. Updates the laser position in real-time
        if (shieldLaser != null && bossTransform != null)
        {
            shieldLaser.SetPosition(0, transform.position);
            shieldLaser.SetPosition(1, bossTransform.position);
        }

        // 2. Timer control with random intervals
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


    // Helper function to pick a random time for the next attack cycle
    private void SortNextAttackInterval()
    {
        currentRandomInterval = Random.Range(minAttackInterval, maxAttackInterval);
        Debug.Log("Next shock wave will trigger in " + currentRandomInterval.ToString("F2") + " seconds.");
    }

    // Coroutine that manages the warning sign and the ground shock attack
    private IEnumerator ShockAttackRoutine()
    {
        isAttacking = true;

        //  WARNING SIGN
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = Color.white; // Cor padrao da torre

        if (sprite != null)
        {
            originalColor = sprite.color;
            // Faz a torre piscar em vermelho rapido indicando perigo eminente
            sprite.color = Color.red;
        }

        Debug.Log("Tower charging electricity... WATCH OUT!");
        yield return new WaitForSeconds(1.0f); // 1 segundo para o jogador ver a torre vermelha e reagir

        // Volta a torre para a cor original logo antes de soltar o raio
        if (sprite != null)
        {
            sprite.color = originalColor;
        }


        // DISCHARGE ATTACK 
        // Activates the shock wave visual/damage
        if (shockVisualPrefab != null) shockVisualPrefab.SetActive(true);
        Debug.Log("ELECTRICITY DISCHARGED ON THE GROUND!");

        // Keeps the shock active hurting the player
        yield return new WaitForSeconds(shockDuration);


        // COOLDOWN 
        // Turns off the attack and prepares for the next cycle
        if (shockVisualPrefab != null) shockVisualPrefab.SetActive(false);

        SortNextAttackInterval();

        isAttacking = false;
    }

    // THIS FUNCTION WILL BE CALLED BY THE PLAYER ATTACK SCRIPT (When pressing K)
    public void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;
        Debug.Log("Tower took damage! Remaining health: " + currentHealth);

        // Hit feedback (Red flash effect on the sprite)
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

        // Disables the laser immediately to show the shield dropped
        if (shieldLaser != null) shieldLaser.enabled = false;

        // Disables the tower object
        gameObject.SetActive(false);
    }
}
