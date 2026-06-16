using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Feedback de Dano")]
    private SpriteRenderer spriteRenderer;
    public float flashDuration = 0.15f;
    private bool isInvincible = false;
    public float invincibilityDuration = 1f; // Tempo que ela fica piscando sem levar dano de novo

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Pega o sprite da DonaMorte

        if (GameController.instance != null)
        {
            GameController.instance.AtualizarBarraDeVidaUI(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        // Se estiver no tempo de invencibilidade ou já sem vida, ignora
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("❤️ Dona Morte levou dano! Vida restante: " + currentHealth);

        if (GameController.instance != null)
        {
            GameController.instance.AtualizarBarraDeVidaUI(currentHealth);
        }

        if (GameController.instance != null && GameController.instance.playerHitSound != null)
        {
            GameController.instance.PlayEffect(GameController.instance.playerHitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageFeedbackRoutine());
        }
    }

    private IEnumerator DamageFeedbackRoutine()
    {
        isInvincible = true;

        // Efeito de piscar vermelho (indica que levou dano)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }

        // Espera o resto do tempo de invencibilidade piscando ou apenas esperando
        yield return new WaitForSeconds(invincibilityDuration - flashDuration);

        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("💀 A Dona Morte perdeu todos os corações!");

        Player playerScript = GetComponentInChildren<Player>();

        if (playerScript != null)
        {
            playerScript.enabled = false; // Desativa os comandos de andar/pular
        }

        // Ativa a animação de morte se você tiver o gatilho criado
        // Animator anim = GetComponentInChildren<Animator>();
        // if (anim != null)
        // {
        //     anim.SetTrigger("death"); // Ajuste o nome se sua animação for diferente
        // }

        if (GameController.instance != null)
        {
            GameController.instance.Invoke("LoseLife", 0.5f);
        }

        Destroy(gameObject, 1.5f);
    }
}
