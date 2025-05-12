
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class niveisfase1 : MonoBehaviour {

    [SerializeField] private Queue<GameObject> tutoriais;
    public GameObject[] panels;
    public GameObject painel_atual;
    [SerializeField] private GameObject button;
    [SerializeField] private TextMeshProUGUI credencial;
    [SerializeField] private TextMeshProUGUI playerName;
    void Start() {
        SaveFase();
        tutoriais = new Queue<GameObject>();
        button = GameObject.Find("passar_tutorial");
        if (panels.Length == 0) {
            panels = GameObject.FindGameObjectsWithTag("totorial_panel");
            if (panels.Length == 0) {
                Debug.LogError("Nenhum painel de tutorial encontrado com a tag 'totorial_panel'.");
                return;
            }
        }
        Debug.Log("Painéis encontrados: " + panels.Length);
        foreach (GameObject panel in panels) {
            panel.SetActive(false);
            tutoriais.Enqueue(panel);
        }
        painel_atual = tutoriais.Dequeue();
        painel_atual.SetActive(true);
        Debug.Log("Exibindo painel inicial: " + painel_atual.name);
        Debug.Log("Total de tutoriais: " + tutoriais.Count);
    }
    

    public void ExibirProximoTutorial() {
        Debug.Log("Tentando exibir o próximo tutorial.");
        if (tutoriais.Count > 0) {
            Debug.Log("Removendo painel atual: " + painel_atual.name);
            painel_atual.SetActive(false);
             painel_atual = tutoriais.Dequeue();
            Debug.Log("Exibindo próximo tutorial: " + painel_atual.name);
            painel_atual.SetActive(true);
        }
        else {
            painel_atual.SetActive(false);
            Debug.Log("Não há mais tutoriais disponíveis.");
            button.SetActive(false);
        }
    }

    private void SaveFase(){
        if(EmboscadaController.gameData != null){
            if (PlayerPrefs.HasKey("playerName"))
            {
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
            }
            else
            {
                Debug.LogError("Nenhum nome de jogador emboscada.");
                EmboscadaController.gameData.playerName = "Jogador";
            }
                
            EmboscadaController.gameData.currentLevel = 8;
            
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            PlayerPrefs.SetInt("currentLevel", EmboscadaController.gameData.currentLevel);
            PlayerPrefs.Save();
            playerName.text = EmboscadaController.gameData.playerName;
            credencial.text = EmboscadaController.gameData.classificacao.ToString();
        }
        else{
            Debug.LogError("gameData é nulo?????");
        }
    }

}
