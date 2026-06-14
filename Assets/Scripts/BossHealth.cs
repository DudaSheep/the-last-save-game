using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

        if (GameController.instance != null && GameController.instance.enemyDeathSound != null)
        {
            GameController.instance.PlayEffect(GameController.instance.enemyDeathSound);
        }

        // Ativa o gatilho de morte no Animator
        // if (anim != null) anim.SetTrigger("MorteTrigger");

        foreach (MonoBehaviour script in componentsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        Invoke("ExecutarDropEfeito", 1.5f);

        // Remove o Boss da cena
        Destroy(gameObject, 1.5f);
    }


    // funcao do pra chamar o drop do item (boss)
    void ExecutarDropEfeito()
    {
        LootDropper dropper = GetComponent<LootDropper>();
        if (dropper != null)
        {
            dropper.DroparItem();
        }
        else
        {
            CarregarProximaFase(); // caso nao ache o objeto passa pra main menu
        }
    }

    void CarregarProximaFase()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Verifica se a próxima fase realmente existe na lista do Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Não há mais fases depois desta no Build Settings! Voltando ao menu principal...");
            SceneManager.LoadScene("MainMenu");
        }
    }
}