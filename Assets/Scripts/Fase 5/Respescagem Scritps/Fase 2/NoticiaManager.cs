using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fase_5.Respescagem_Scritps.Fase_2
{
    public class NoticiaManagerRepescagem : MonoBehaviour
    {
        [Header("Cores de seleção")] 
        public Color32 normalColor = Color.white; // #86009F new Color32(0x86, 0x00, 0x9F, 0xFF);
        public Color32 selectedColor = new Color32(0xCA, 0x80, 0x07, 0xFF); // #CA8007

        [Header("Painéis")]
        public GameObject closedPanel;
        public GameObject openPanel;
        public GameObject resultPanel;

        [Header("Closed View")]
        public TextMeshProUGUI closedTitulo;
        public TextMeshProUGUI closedConteudo;
        public TextMeshProUGUI closedLink;
        public TextMeshProUGUI closedData;
        public Toggle[] optionToggles;
        public Text[] optionTexts;
        public Button confirmButton;
        public Button openButton;

        [Header("Open View")]
        public TextMeshProUGUI openTitulo;
        public TextMeshProUGUI openData;
        public TextMeshProUGUI openConteudo;
        public TextMeshProUGUI openLink;
        public Button closeButton;

        [Header("Resultado")]
        public TextMeshProUGUI resultText;
        public GameObject prefabInstructions;
        public GameObject me;

        private Noticia currentNoticia;
        private int totalQuestions, totalAnswered, totalCorrect;
        [SerializeField] private TextMeshProUGUI credencial;
        void Awake()
        {
            foreach (var tog in optionToggles)
                tog.group = null;
            for (int i = 0; i < optionToggles.Length; i++)
            {
                int idx = i;
                var tog = optionToggles[idx];
                var txt = optionTexts[idx];

                tog.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        int count = 0;
                        foreach (var t in optionToggles)
                            if (t.isOn)
                                count++;
                        if (count > 3)
                        {
                            tog.isOn = false;
                            Debug.Log("Você só pode escolher até 3 opções.");
                            return;
                        }
                    }

                    var cb = tog.colors;
                    cb.normalColor = isOn ? selectedColor : normalColor;
                    cb.highlightedColor = isOn ? selectedColor : normalColor;
                    cb.pressedColor = isOn ? selectedColor : normalColor;
                    cb.selectedColor = isOn ? selectedColor : normalColor;
                    cb.disabledColor = normalColor;
                    tog.colors = cb;
                    int onCount = 0;
                    foreach (var t in optionToggles)
                        if (t.isOn)
                            onCount++;
                    confirmButton.interactable = (onCount == 3);
                });
            }
            confirmButton.onClick.AddListener(OnConfirm);
            openButton.onClick.AddListener(OnOpen);
            closeButton.onClick.AddListener(OnClose);
        }

        void Start()
        { if(EmboscadaController.gameData == null)
            {
                EmboscadaController.gameData = new EmboscadaController.GameData();
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
                EmboscadaController.gameData.classificacao =
                    (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            }
            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            resultPanel.SetActive(false);
            openPanel.SetActive(false);
            closedPanel.SetActive(true);
            confirmButton.interactable = false;

            totalQuestions = Fase2ManagerRepescagem.instance.TotalPerguntas;
            ShowNextQuestion();
        }

        void ShowNextQuestion()
        {
            currentNoticia = Fase2ManagerRepescagem.instance.GetNextQuestion();
            if (currentNoticia == null)
            {
                ShowResult();
                return;
            }
            closedTitulo.text   = currentNoticia.titulo;
            closedData.text     = currentNoticia.data;
            closedLink.text     = currentNoticia.linkFonte;
            closedConteudo.text = currentNoticia.conteudo;
            for (int i = 0; i < optionToggles.Length; i++)
            {
                if (i < currentNoticia.opcoesResposta.Count)
                {
                    optionToggles[i].gameObject.SetActive(true);
                    optionToggles[i].isOn = false;
                    optionTexts[i].text   = currentNoticia.opcoesResposta[i].texto;

                    var cb = optionToggles[i].colors;
                    cb.normalColor      = normalColor;
                    cb.highlightedColor = normalColor;
                    cb.pressedColor     = normalColor;
                    cb.selectedColor    = selectedColor;
                    cb.disabledColor    = normalColor;
                    optionToggles[i].colors = cb;
                }
                else
                {
                    optionToggles[i].gameObject.SetActive(false);
                }
            }
            openTitulo.text   = currentNoticia.titulo;
            openData.text     = currentNoticia.data;
            openConteudo.text = currentNoticia.conteudo;
            openLink.text     = currentNoticia.linkFonte;
            confirmButton.interactable = false;
        }
        void OnOpen()   => openPanel.SetActive(true);
        void OnClose()  => openPanel.SetActive(false);

        void OnConfirm()
        {
            bool correct = true;
            for (int i = 0; i < currentNoticia.opcoesResposta.Count; i++)
            {
                if (optionToggles[i].isOn != currentNoticia.opcoesResposta[i].isCorreto)
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

            float taxa = (float)totalCorrect / totalQuestions * 100f;
            Fase2ManagerRepescagem.statusFase2 = taxa;

            if (taxa > 50f)
                resultText.text = $"🎉 Você venceu! Acertos: {totalCorrect}/{totalQuestions}";
            else
                resultText.text = $"😞 Você perdeu. Acertos: {totalCorrect}/{totalQuestions}";

            var inst   = Instantiate(prefabInstructions, null);
            var canvas = inst.GetComponent<Canvas>();
            canvas.renderMode    = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera   = Camera.main;
            canvas.sortingOrder  = 10;
            Destroy(me);
        }
    }
}
