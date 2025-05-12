using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChronoUIController : MonoBehaviour
{
    [Header("Detalhe de Notícia (reaproveita OpenView)")]
    public GameObject openPanel;
    public TextMeshProUGUI openTitulo, openData, openConteudo, openLink;
    public Button closeButton;

    [Header("Botões de notícia")]
    public Button[] newsButtons;          
    public TextMeshProUGUI[] buttonTexts; 

    [Header("Feedback de ordem")]
    public TextMeshProUGUI feedbackText;  

    [Header("Painel de conclusão")]
    public GameObject completePanel;      
    public Button continueButton;
    public Button resetButton;

    [Header("Cores")]
    public Color selectedColor = Color.green;

    private List<Noticia> listaNoticias;
    private List<Noticia> ordemSelecionada = new List<Noticia>();
    private int nextChronoIndex = 1;
    private Color defaultButtonColor;

    void Start()
    {
        // pega as 4 notícias da fase 1
        listaNoticias = Fase2Manager.perguntasSelecionadas;

        // config painel completaão
        completePanel.SetActive(false);
        continueButton.onClick.AddListener(OnContinue);
        resetButton.onClick.AddListener(OnReset);

        // salva cor preta padrão do botão (assume todos iguais)
        if (newsButtons.Length > 0)
            defaultButtonColor = newsButtons[0].GetComponent<Image>().color;

        // fecha detalhe
        openPanel.SetActive(false);
        closeButton.onClick.AddListener(() => openPanel.SetActive(false));

        feedbackText.text = $"Selecione a notícia #{nextChronoIndex}";

        // configura cada botão
        for (int i = 0; i < newsButtons.Length; i++)
        {
            int idx = i;
            // texto original
            buttonTexts[i].text = listaNoticias[i].titulo;

            // clique rápido = abrir detalhe
            newsButtons[i].onClick.AddListener(() => OpenDetail(idx));

            // longo = selecionar ordem
            var lp = newsButtons[i].gameObject.GetComponent<LongPressButton>();
            lp.holdThreshold = 0.5f;
            lp.onLongPress.AddListener(() => OnSelectChrono(idx));
        }
    }

    void OpenDetail(int i)
    {
        var n = listaNoticias[i];
        openPanel.SetActive(true);
        openTitulo.text   = n.titulo;
        openData.text     = n.data;
        openConteudo.text = n.conteudo;
        openLink.text     = n.linkFonte;
    }

    void OnSelectChrono(int i)
    {
        var n = listaNoticias[i];
        if (ordemSelecionada.Contains(n)) return;

        // marca visual
        var img = newsButtons[i].GetComponent<Image>();
        img.color = selectedColor;

        // registra
        ordemSelecionada.Add(n);
        newsButtons[i].interactable = false;
        buttonTexts[i].text += $"  ({nextChronoIndex})";

        // avança
        nextChronoIndex++;
        if (nextChronoIndex <= listaNoticias.Count)
            feedbackText.text = $"Selecione a notícia #{nextChronoIndex}";
        else
            OnChronoComplete();
    }

    void OnChronoComplete()
    {
        feedbackText.text = "Ordenação completa!";
        completePanel.SetActive(true);
    }
    private float verificarResultado()
    {
        // 1) Gera a lista “correta” ordenada por data
        List<Noticia> correta = listaNoticias
            .OrderBy(n =>
            {
                // tenta parsear "YYYY-MM-DD"
                DateTime dt;
                if (!DateTime.TryParseExact(n.data, "yyyy-MM-dd", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    Debug.LogWarning($"Formato de data inválido em {n.titulo}: '{n.data}'");
                    dt = DateTime.MinValue;
                }
                return dt;
            })
            .ToList();

        // 2) Conta quantos índices batem
        int acertos = 0;
        for (int i = 0; i < ordemSelecionada.Count; i++)
            if (ordemSelecionada[i] == correta[i])
                acertos++;

        // 3) Retorna porcentagem [0..100]
        float percent = (float)acertos / listaNoticias.Count * 100f;
        Debug.Log($"Ordenação: {acertos}/{listaNoticias.Count} → {percent}%");
        return percent;
    }

    void OnContinue()
    {
        float faseQuizOK = Fase2Manager.statusFase2;
        float ordenacaoPct = verificarResultado();
        
        bool fase2OK =( (faseQuizOK+ordenacaoPct)/2 >=50f);
        
        saveFase(fase2OK);

        if (fase2OK)
        {
            MainManager.indiceCanvainicial = 23;
        }
        else
        {
            MainManager.indiceCanvainicial = 11;
        }
        SceneManager.LoadSceneAsync("main");
    }

  

    void OnReset()
    {
        ordemSelecionada.Clear();
        nextChronoIndex = 1;
        feedbackText.text = $"Selecione a notícia #{nextChronoIndex}";
        completePanel.SetActive(false);
        openPanel.SetActive(false);
        for (int i = 0; i < newsButtons.Length; i++)
        {
            newsButtons[i].interactable = true;
            buttonTexts[i].text = listaNoticias[i].titulo;
            newsButtons[i].GetComponent<Image>().color = defaultButtonColor;
        }
    }
    
    void saveFase(bool status)
    {
        EmboscadaController.gameData ??= new EmboscadaController.GameData();
        EmboscadaController.gameData.niveisganhos[1] = status;
        EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
        EmboscadaController.gameData.classificacao = status ? EmboscadaController.gameData.classificacao + 1 : EmboscadaController.gameData.classificacao;
        EmboscadaController.gameData.currentLevel = 23;
        PlayerPrefs.SetInt("nivel" + 1, EmboscadaController.gameData.niveisganhos[1] ? 1 : 0);
        PlayerPrefs.SetInt("currentLevel", 23);
        PlayerPrefs.Save();
    }
}
