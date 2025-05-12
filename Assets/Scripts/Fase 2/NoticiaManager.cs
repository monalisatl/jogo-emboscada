using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoticiaManager : MonoBehaviour
{
    public GameObject me;
    public GameObject prefabInstructions;
    [Header("Painéis")]
    public GameObject closedPanel;    // painel “fechado” com título, data, opções e botão Confirmar
    public GameObject openPanel;      // painel “aberto” mostrando conteúdo e link

    [Header("Closed View")]
    public TextMeshProUGUI closedTitulo;
    public TextMeshProUGUI closedData;
    public Toggle[] optionToggles;    // arraste aqui 4 Toggles
    public Text[] optionTexts;
    public Button confirmButton;
    public Button openButton;         // botão “Abrir notícia”

    [Header("Open View")]
    public TextMeshProUGUI openTitulo;
    public TextMeshProUGUI openData;
    public TextMeshProUGUI openConteudo;
    public TextMeshProUGUI openLink;
    public Button closeButton;        // botão “Fechar notícia”

    [Header("Resultado")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private Noticia currentNoticia;
    private int totalAnswered = 0;
    private int totalCorrect = 0;
    private int totalQuestions;

    void Start()
    {
        // UI inicial
        resultPanel.SetActive(false);
        openPanel.SetActive(false);
        closedPanel.SetActive(true);

        totalQuestions = Fase2Manager.instance.TotalPerguntas;

        confirmButton.onClick.AddListener(OnConfirm);
        openButton.onClick.AddListener(OnOpen);
        closeButton.onClick.AddListener(OnClose);

        ShowNextQuestion();
    }

    void ShowNextQuestion()
    {
        currentNoticia = Fase2Manager.instance.GetNextQuestion();
        if (currentNoticia == null)
        {
            ShowResult();
            return;
        }

        // Preenche closed view
        closedTitulo.text = currentNoticia.titulo;
        closedData.text   = currentNoticia.data;

        for (int i = 0; i < optionToggles.Length; i++)
        {
            if (i < currentNoticia.opcoesResposta.Count)
            {
                optionToggles[i].gameObject.SetActive(true);
                optionToggles[i].isOn = false;
                optionTexts[i].text   = currentNoticia.opcoesResposta[i].texto;
            }
            else
            {
                optionToggles[i].gameObject.SetActive(false);
            }
        }

        // Preenche open view
        openTitulo.text    = currentNoticia.titulo;
        openData.text      = currentNoticia.data;
        openConteudo.text  = currentNoticia.conteudo;
        openLink.text      = currentNoticia.linkFonte;
    }

    void OnOpen()
    {
        openPanel.SetActive(true);
    }

    void OnClose()
    {
        openPanel.SetActive(false);
    }

    void OnConfirm()
    {
        // compara exata correspondência entre isOn e isCorreto
        bool correct = true;
        for (int i = 0; i < currentNoticia.opcoesResposta.Count; i++)
        {
            bool selecionado = optionToggles[i].isOn;
            bool deviaSer   = currentNoticia.opcoesResposta[i].isCorreto;
            if (selecionado != deviaSer)
            {
                correct = false;
                break;
            }
        }

        totalAnswered++;
        if (correct) totalCorrect++;

        ShowNextQuestion();
    }

    void ShowResult()
    {
        closedPanel.SetActive(false);
        openPanel.SetActive(false);
        resultPanel.SetActive(true);
        float taxa = (float) totalCorrect / totalQuestions * 100f;
        if (taxa > 50f)
        {
            resultText.text = $"🎉 Você venceu! Acertos: {totalCorrect}/{totalQuestions}";
            Fase2Manager.statusFase2 = taxa;
        } 

        else
        {
            Fase2Manager.statusFase2 = taxa;
            resultText.text = $"😞 Você perdeu. Acertos: {totalCorrect}/{totalQuestions}";
        }
        var inst =Instantiate(prefabInstructions, null);
        var canvas = inst.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 10;
        Destroy(me);
    }
    
}
