using System;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set; }
    private string _filePath;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        
        _filePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }
    
    public void SaveGame()
    {
        EmboscadaController.GameData data = EmboscadaController.gameData;

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        try
        {
            File.WriteAllText(_filePath, json);
            Debug.Log("Jogo salvo em: " + _filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Falha ao salvar: " + e.Message);
        }
    }
    
    public void LoadGame()
    {
        if (!File.Exists(_filePath))
        {
            Debug.Log("Nenhum save encontrado em: " + _filePath);
            return;
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            EmboscadaController.GameData data = JsonUtility.FromJson<EmboscadaController.GameData>(json);
            EmboscadaController.gameData = data;
            PlayerPrefs.Save();
            Debug.Log("Jogo carregado com sucesso!");
        }
        catch (Exception e)
        {
            Debug.LogError("Falha ao carregar: " + e.Message);
        }
    }
}
