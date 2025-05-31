
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class niveisfase1 : MonoBehaviour {

    [SerializeField] private Queue<GameObject> tutoriais;
    [SerializeField] private Queue<GameObject> detailTutoriais;
    public GameObject[] panels;
    public GameObject painel_atual;
    [SerializeField] private GameObject detailPanelAtual;
    [SerializeField] private GameObject button;
    [SerializeField] private TextMeshProUGUI credencial;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private GameObject[] detailImage;
    
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
        if (detailImage.Length == 0)
        {
            detailImage = GameObject.FindGameObjectsWithTag("detail_image");
            if (detailImage.Length == 0)
            {
                Debug.LogError("Nenhuma imagem de detalhe encontrada com a tag 'detail_image'.");
                return;
            }
        }
       // Debug.Log("Painéis encontrados: " + panels.Length);
        foreach (GameObject panel in panels) {
            panel.SetActive(false);
            
            tutoriais.Enqueue(panel);
        }
        foreach (GameObject image in detailImage)
        {
            image.SetActive(false);
            detailTutoriais.Enqueue(image);
        }
        painel_atual = tutoriais.Dequeue();
        detailPanelAtual = detailTutoriais.Dequeue();
        detailPanelAtual.SetActive(true);
        painel_atual.SetActive(true);
        Debug.Log("Exibindo painel inicial: " + painel_atual.name);
        Debug.Log("Total de tutoriais: " + tutoriais.Count);
    }
    

    public void ExibirProximoTutorial() {
        if (tutoriais.Count > 0) {
            painel_atual.SetActive(false);
            detailPanelAtual.SetActive(false);
            detailPanelAtual = detailTutoriais.Dequeue();
             painel_atual = tutoriais.Dequeue();
            painel_atual.SetActive(true);
            detailPanelAtual.SetActive(true);
        }
        else {
            painel_atual.SetActive(false);
            detailPanelAtual.SetActive(false);
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
