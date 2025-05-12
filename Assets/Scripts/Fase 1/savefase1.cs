using System;
using TMPro;
using UnityEngine;

public class savefase1 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI credencial;
    [SerializeField] private TextMeshProUGUI playername;

    private void Awake()
    {
        if (EmboscadaController.gameData == null)
        {
            EmboscadaController.gameData = new EmboscadaController.GameData();
            EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
        }
        playername.text = EmboscadaController.gameData.playerName != null 
            ? EmboscadaController.gameData.playerName 
            : "Jogador";
        credencial.text = EmboscadaController.gameData.classificacao.ToString() != null 
            ? EmboscadaController.gameData.classificacao.ToString() 
            : "Amador";
    }
    
}
