using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fase5Comeco : MonoBehaviour
{
    public static List<bool> Repescagens = new List<bool>();
    [SerializeField] private List<Button> nivelButtons = new List<Button>();

    private void Awake()
    {
        LoadSave();
    }
    private void Start()
    {

    }

    private static void LoadSave()
    {
        Repescagens.Clear();  

        EmboscadaController.gameData ??= new EmboscadaController.GameData();
        for(var i = 0; i < EmboscadaController.gameData.niveisganhos.Length; i++)
        {
            var ganhou = PlayerPrefs.GetInt($"nivel{i}", 0) == 1;
            EmboscadaController.gameData.niveisganhos[i] = ganhou;
            Repescagens.Add(!ganhou);
        }

        EmboscadaController.gameData.classificacao =
            (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
        EmboscadaController.gameData.currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
        EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName");
        EmboscadaController.gameData.selectedCharacterId = PlayerPrefs.GetInt("selectedCharacterId");
    }

    private void inicializarBotoes()
    {
        foreach (var button in nivelButtons)
        {
            button.interactable = false;
            button.gameObject.SetActive(false);
        }
    }

    private void habilitar_fases()
    {
        for (var i = 0; i < nivelButtons.Count; i++)
        {
            nivelButtons[i].interactable = Repescagens[i];
        }
    }
    
    public void OnClickNivel(int nivel)
    {
        switch (nivel)
        {
            case 1:
                SceneManager.LoadScene("Level1");
                break;
        }
    }
    
    
}