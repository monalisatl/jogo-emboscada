using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playScript : MonoBehaviour{
    
    [SerializeField] Button playButton;

    private void Start()
    {
        if (playButton == null)
            playButton = GetComponent<Button>();
        EmboscadaController.gameData = new EmboscadaController.GameData();
        EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "");
        EmboscadaController.gameData.currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
        EmboscadaController.gameData.selectedCharacterId = PlayerPrefs.GetInt("selectedCharacterId", 0);
        EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
        EmboscadaController.gameData.niveisganhos = new bool[5];
        for (int i = 0; i < EmboscadaController.gameData.niveisganhos.Length; i++)
        {
            EmboscadaController.gameData.niveisganhos[i] = PlayerPrefs.GetInt("nivel"+i, 0) == 1;
        }
        if (EmboscadaController.gameData.currentLevel == 0 && EmboscadaController.gameData.playerName == "")
        {
            playButton.interactable = false;
        }
        else
        {
                 playButton.interactable = true;
        }
    }


    public void PlayGame()
    {
        MainManager.indiceCanvainicial = EmboscadaController.gameData.currentLevel;
        SceneManager.LoadSceneAsync("main");
    }

    public void NewGame()
    {
        EmboscadaController.gameData = new EmboscadaController.GameData();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("main");
    }
}
