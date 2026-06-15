using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyMuseumManager : MonoBehaviour
{
    // Singleton simples (sem DontDestroyOnLoad para não duplicar objetos)
    public static LegacyMuseumManager Instancia;

    [Header("Templates Visuais do Museu")]
    public GameObject templateAtariET;
    public GameObject templateFlappyBird;
    public GameObject templateClubPenguin;
    public GameObject templateBossFinal;

    // As variáveis static ficam salvas na memória RAM do jogo, elas NUNCA somem ao mudar de cena!
    public static bool liberouET = false;
    public static bool liberouFlappy = false;
    public static bool liberouClub = false;
    public static bool liberouBossFinal = false;

    private void Awake()
    {
        // Sempre atualiza a instância para o gerenciador da cena atual
        Instancia = this;
    }

    private void Start()
    {
        // Sempre que o Menu Principal abrir, ele limpa e ativa apenas o que foi conquistado
        AtualizarPainelDoMuseu();
    }

    // Função pública para atualizar o estado visual dos itens na tela
    public void AtualizarPainelDoMuseu()
    {
        if (templateAtariET != null) templateAtariET.SetActive(liberouET);
        if (templateFlappyBird != null) templateFlappyBird.SetActive(liberouFlappy);
        if (templateClubPenguin != null) templateClubPenguin.SetActive(liberouClub);
        if (templateBossFinal != null) templateBossFinal.SetActive(liberouBossFinal);
    }

    // --- Métodos de liberação chamados pelos itens coletáveis ---
    public void LiberarItemET() { liberouET = true; }
    public void LiberarItemFlappy() { liberouFlappy = true; }
    public void LiberarItemClub() { liberouClub = true; }

    public void LiberarItemBossFinal()
    {
        liberouBossFinal = true;
        // Não precisamos atualizar o painel aqui, pois a cena vai mudar logo em seguida!
    }
}