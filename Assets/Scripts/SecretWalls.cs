using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretArea : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private float fadeSpeed = 2f; 
    private Coroutine fadeCoroutine;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTilemap(0.5f)); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTilemap(1f)); 
        }
    }

    private IEnumerator FadeTilemap(float targetAlpha)
    {
        Color currentColor = tilemap.color;

        while (!Mathf.Approximately(currentColor.a, targetAlpha))
        {
            currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            tilemap.color = currentColor;
            yield return null;
        }
    }
}