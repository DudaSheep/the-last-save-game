using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Header("Item da Hierarquia (Desativado)")]
    [SerializeField] private GameObject itemCenaParaAtivar;

    public void DroparItem()
    {
        if (itemCenaParaAtivar == null) return;

        // Ativa o item na cena para o player conseguir ver e pegar!
        itemCenaParaAtivar.SetActive(true);
    }
}
