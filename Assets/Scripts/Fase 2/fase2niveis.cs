using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class fase2niveis : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI credencial;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private GameObject statusFase1;
    [SerializeField] private Image[] status;

    private void Start()
    {
        SaveFase();
        MarcarFase();
    }

    void SaveFase()
    {

            if (PlayerPrefs.HasKey("playerName"))
            {
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
            }
            else
            {
                Debug.LogError("Nenhum nome de jogador encontrado. Usando 'Jogador' como padrão.");
                EmboscadaController.gameData.playerName = "Jogador";
            }

            if (PlayerPrefs.HasKey("credencial"))
            {
                EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("credencial", 0);
            }
            else
            {
                Debug.LogError("Nenhuma classificação encontrada. Usando 'NENHUMA' como padrão.");
                EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Amador;
            }
                
            if (!credencial) return;
            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            playerName.text = EmboscadaController.gameData.playerName;
            PlayerPrefs.SetInt("currentLevel", 16);
            PlayerPrefs.Save();
    }

    void MarcarFase()
    {
        if (EmboscadaController.gameData.niveisganhos[0])
        {
            statusFase1.GetComponent<Image>().sprite = status[0].sprite;
            
        }
        else
        {
            statusFase1.GetComponent<Image>().sprite = status[1].sprite;
        }
    }
}
