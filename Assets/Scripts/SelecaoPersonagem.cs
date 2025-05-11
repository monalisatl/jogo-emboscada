using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelecaoPersonagem : MonoBehaviour
{
    [Header("Dados de Personagem")]
    public List<Personagem> personagens;   
    private int currentIndex = 0;

    [Header("Referências de UI")]
    public Image displayImage;             // UI Image no Canvas onde mostraremos o sprite
    public Button buttonLeft;
    public Button buttonRight;
    public Button buttonSelect;
    public GameObject panelName;           // Painel que contém o InputField e o Continue
    public InputField inputName;
    public Button buttonContinue;

    [Header("Escalas de Destaque")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 highlightedScale = Vector3.one * 1.2f;

    void Start()
    {
        // Conecta callbacks
        buttonLeft.onClick.AddListener(OnLeft);
        buttonRight.onClick.AddListener(OnRight);
        buttonSelect.onClick.AddListener(OnSelect);
        buttonContinue.onClick.AddListener(OnContinue);

        panelName.SetActive(false);
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        // Atualiza o sprite e reset de escala
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
        // Destaca o sprite
        displayImage.rectTransform.localScale = highlightedScale;
        // Abre painel de nome
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
        // Salva em PlayerPrefs (ou use seu GameManager)
        PlayerPrefs.SetString("SelectedCharacterName", nome);
        PlayerPrefs.SetInt("SelectedCharacterID", personagens[currentIndex].id);
        PlayerPrefs.Save();

        // Carrega próxima cena (confira o nome exato na Build Settings)
        SceneManager.LoadScene("CenaDoJogo");
    }
}

