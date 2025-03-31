
using System;
using System.Collections.Generic;
using UnityEngine;

public class niveisfase1 : MonoBehaviour {

    [SerializeField] private Queue<GameObject> tutoriais;
    public GameObject[] panels;
    public GameObject painel_atual;
    [SerializeField] private GameObject button;
    void Start() {
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

    void Update() {
        
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


}
