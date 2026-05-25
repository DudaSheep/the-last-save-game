using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Configurações do Coletável")]
    public int scoreValue = 1;

    [Header("Efeitos (Opcional)")]
    public GameObject collectEffectPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        Debug.Log("Cartucho de Atari coletado! + " + scoreValue + " ponto(s)");

        if (collectEffectPrefab != null)
        {
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}