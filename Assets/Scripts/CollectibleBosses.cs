using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleBosses : MonoBehaviour
{
    public enum TipoDrop { AtariET, FlappyBird, ClubPenguin, BossFinal }

    [Header("Configurações do Drop")]
    public TipoDrop tipoDesteItem;

    // [Header("Nome da Cena do Menu Inicial")]
    // public string nomeCenaMenu = "CutsceneFinal";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ColetarItem();
        }
    }

    private void ColetarItem()
    {
        switch (tipoDesteItem)
        {
            case TipoDrop.AtariET:
                LegacyMuseumManager.liberouET = true;
                break;
            case TipoDrop.FlappyBird:
                LegacyMuseumManager.liberouFlappy = true;
                break;
            case TipoDrop.ClubPenguin:
                LegacyMuseumManager.liberouClub = true;
                break;
            case TipoDrop.BossFinal:
                LegacyMuseumManager.liberouBossFinal = true;
                break;
        }

        // --- Regra Especial para o Boss Final ---
        if (tipoDesteItem == TipoDrop.BossFinal)
        {
            Destroy(gameObject);
            SceneManager.LoadScene("CutsceneFinal");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
