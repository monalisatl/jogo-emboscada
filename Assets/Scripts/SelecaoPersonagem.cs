using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelecaoPersonagem : MonoBehaviour
{
    [Header("Dados de Personagem")]
    public List<Personagem> personagens;
    private int currentIndex = 0;

    [Header("Referências de UI")]
    public Image displayImage;
    public Button buttonLeft, buttonRight, buttonSelect;
    public GameObject panelName;
    public TMP_InputField inputName;
    public Button buttonContinue;

    [Header("Escalas de Destaque")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 highlightedScale = Vector3.one * 1.2f;

    void Start()
    {
        buttonLeft.onClick.AddListener(OnLeft);
        buttonRight.onClick.AddListener(OnRight);
        buttonSelect.onClick.AddListener(OnSelect);
        buttonContinue.onClick.AddListener(OnContinue);

        panelName.SetActive(false);
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        displayImage.sprite = personagens[currentIndex].sprite;
        displayImage.rectTransform.localScale = normalScale;
    }

    void OnLeft()
    {
        currentIndex = (currentIndex - 1 + personagens.Count) % personagens.Count;
        UpdateDisplay();
    }

    void OnRight()
    {
        currentIndex = (currentIndex + 1) % personagens.Count;
        UpdateDisplay();
    }

    void OnSelect()
    {
        displayImage.rectTransform.localScale = highlightedScale;
        panelName.SetActive(true);
    }

    void OnContinue()
    {
        string nome = inputName.text.Trim();
        if (string.IsNullOrEmpty(nome))
        {
            Debug.LogWarning("Digite um nome antes de continuar!");
            return;
        }

        try
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.selectedCharacterId = personagens[currentIndex].id;
            EmboscadaController.gameData.playerName = nome;
            EmboscadaController.gameData.currentLevel = 5;
            EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Amador;
            PlayerPrefs.SetString("playerName", nome);
            PlayerPrefs.SetInt("selectedCharacterId", personagens[currentIndex].id);
            PlayerPrefs.SetInt("currentLevel", 5);
            PlayerPrefs.SetInt("classification", 0);
            PlayerPrefs.Save();
            OnNextPage();
        }
        catch (Exception e)
        {
           Debug.Log($"Erro ao salvar dados do jogador: {e.Message}");
            throw;
        }
    }

    private void OnNextPage()
    {
        if (MainManager.main != null)
        {
            MainManager.main.ProximoCanvas();
        }
        else
        {
            MainManager.indiceCanvainicial = 5;
            SceneManager.LoadSceneAsync("main");
        }
    }
}
