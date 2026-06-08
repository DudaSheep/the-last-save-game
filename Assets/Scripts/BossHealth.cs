using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [Tooltip("Quantidade de vida inicial para este chefe específico")]
    public int currentHealth = 20;
    private bool isDead = false;

    [Header("Componentes para Desativar na Morte")]
    [Tooltip("Arraste para cá os scripts de IA/Ataque específicos de cada boss (ex: BossET)")]
    public MonoBehaviour[] componentsToDisable;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} tomou {damage} de dano! Vida restante: {currentHealth}");

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
        isDead = true;
        Debug.Log($"{gameObject.name} foi completamente derrotado!");

        // Ativa o gatilho de morte no Animator
        // if (anim != null) anim.SetTrigger("MorteTrigger");

        foreach (MonoBehaviour script in componentsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        Destroy(gameObject, 2f);
    }
}