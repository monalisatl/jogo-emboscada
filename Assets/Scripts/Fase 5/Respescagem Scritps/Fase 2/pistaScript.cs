using System;
using System.Collections;
using Fase_5;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pistaScript : MonoBehaviour
{
    [SerializeField] private GameObject pista;
    [SerializeField] private GameObject load;

    private void Awake()
    {
        pista.SetActive(false);
    }

    void Start()
    {
        OnSaveVitoria();
    }
    
    void OnSaveVitoria()
    {
        // Salva o progresso da fase 2 (índice 1)
        Fase5Comeco.Repescagens[1] = true;
        EmboscadaController.gameData ??= new EmboscadaController.GameData();
        EmboscadaController.gameData.classificacao =
            (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
        EmboscadaController.gameData.currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
        EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName");
        EmboscadaController.gameData.selectedCharacterId = PlayerPrefs.GetInt("selectedCharacterId");
        EmboscadaController.gameData.niveisRepescagem[1] = true;
        PlayerPrefs.SetInt("repescagem1", EmboscadaController.gameData.niveisRepescagem[1]? 1:0);
        PlayerPrefs.SetInt("currentLevel", 50);
        
        // Configura o prefab de loading e verifica se todas as repescagens foram completadas
        // RepescagemManager.SetLoadingPagePrefab(load);
        // RepescagemManager.CheckAllRepescagensComplete();
    }

    public void OpenPista()
    {
        pista.SetActive(true);
    }

    public void ClosePistaAndLoadNextScene()
    {
        // Usa o RepescagemManager para verificar o fluxo e carregar a próxima cena
        RepescagemManager.SetLoadingPagePrefab(load);
        RepescagemManager.CheckAllRepescagensComplete();
        
        // Mantemos o método original como comentário para referência
        // StartCoroutine(loadRepescagemMain());
    }

    // Método original mantido para referência
    private IEnumerator loadRepescagemMain()
    {
        var loadI = Instantiate(load);
        loadI.SetActive(true);
        var progress = SceneManager.LoadSceneAsync("faserepescagem");

        while (progress is { isDone: false })
        {
           loadI.GetComponent<Slider>().value = progress.progress;
           yield return null;
        }
        yield return new WaitForSecondsRealtime(1.0f);
    }
}

