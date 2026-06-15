using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WaterHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("💦 Caiu na água! Reiniciando a fase sem dar Game Over...");

            Player playerScript = collision.GetComponent<Player>();
            if (playerScript == null)
            {
                playerScript = collision.GetComponentInParent<Player>();
            }

            if (playerScript != null)
            {
                playerScript.enabled = false;
            }

            StartCoroutine(RestartPhaseRoutine());
        }
    }

    private IEnumerator RestartPhaseRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (GameController.instance != null)
        {
            GameController.instance.RestartLevel();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}