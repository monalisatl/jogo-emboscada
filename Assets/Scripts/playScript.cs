using System;
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
        SaveLoadManager.Instance.LoadGame();
        if (EmboscadaController.gameData.currentLevel == 0 && EmboscadaController.gameData.playerName == "")
        {
            playButton.interactable = false;
        }
        else
        {
            playButton.interactable = true;
        }
    }


    public void playGame()
    {
        EmboscadaController.gameData = new EmboscadaController.GameData();
        SaveLoadManager.Instance.LoadGame();
        SceneManager.LoadSceneAsync("main");
    }

    public void NewGame()
    {
        EmboscadaController.gameData = new EmboscadaController.GameData();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SaveLoadManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync("main");
    }
}
