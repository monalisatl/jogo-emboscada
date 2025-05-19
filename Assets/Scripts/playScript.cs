using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playScript : MonoBehaviour{
    
    [SerializeField] Button playButton;
    [SerializeField] private GameObject load;

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
        EmboscadaController.gameData.niveisRepescagem = new bool[5];
        for (int i = 1; i < EmboscadaController.gameData.niveisganhos.Length-1; i++)
        {
            EmboscadaController.gameData.niveisganhos[i] = PlayerPrefs.GetInt("repescagem"+i, 0) == 1;
        }
        if (EmboscadaController.gameData.currentLevel == 0 && EmboscadaController.gameData.playerName == "")
        {
            playButton.interactable = false;
            playButton.gameObject.SetActive(false);
        }
        else
        {
                 playButton.interactable = true;
        }
    }


    public void PlayGame()
    {
               EmboscadaController.gameData = new EmboscadaController.GameData();
               PlayerPrefs.DeleteAll();
               PlayerPrefs.Save();
               StartCoroutine(LoadRepescagem("main"));
    }

    public void ContinueGame()
    {
        if (EmboscadaController.gameData.currentLevel > 99)
        {
            StartCoroutine(LoadRepescagem("FINAL_certificado"));
        }
        else if (EmboscadaController.gameData.currentLevel >= 80)
        {
            StartCoroutine(LoadRepescagem("fase5"));
        }
        else
        {
            MainManager.indiceCanvainicial = EmboscadaController.gameData.currentLevel;
            SceneManager.LoadSceneAsync("main");
        }
  
      

    }

    private IEnumerator LoadRepescagem(string scene)
    {
            var loadI = Instantiate(load);
            loadI.SetActive(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            while (operation is { isDone: false })
            {
                Slider progressBar = loadI.GetComponent<Slider>();
                if (progressBar != null)
                {
                    progressBar.value = operation.progress;
                }
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
    }
}