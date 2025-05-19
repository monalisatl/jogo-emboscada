using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_2
{
    public class NoticiaManager : MonoBehaviour
    {
        [Header("Timer")] [Tooltip("Tempo máximo para responder às perguntas")]
        public float tempoLimite = 60f;

        [Tooltip("Referência ao objeto de imagem do cronômetro")]
        public Image cronometro;

        private float _tempoRestante;
        private bool _cronometroAtivo = true;

        [Header("Cores de seleção")]
        public Color32 normalColor = Color.white; // #86009F new Color32(0x86, 0x00, 0x9F, 0xFF);

        public Color32 selectedColor = new Color32(0xCA, 0x80, 0x07, 0xFF); // #CA8007

        [Header("Painéis")] public GameObject closedPanel;
        public GameObject openPanel;
        public GameObject resultPanel;

        [Header("Closed View")] public TextMeshProUGUI closedTitulo;
        public TextMeshProUGUI closedConteudo;
        public TextMeshProUGUI closedLink;
        public TextMeshProUGUI closedData;
        public Toggle[] optionToggles;
        public Text[] optionTexts;
        public Button confirmButton;
        public Button openButton;

        [Header("Open View")] public TextMeshProUGUI openTitulo;
        public TextMeshProUGUI openData;
        public TextMeshProUGUI openConteudo;
        public TextMeshProUGUI openLink;
        public Button closeButton;

        [Header("Resultado")] public TextMeshProUGUI resultText;
        public GameObject prefabInstructions;
        public GameObject me;

        private Noticia currentNoticia;
        private int totalQuestions, totalAnswered, totalCorrect;
        [SerializeField] private TextMeshProUGUI credencial;
        private bool isRepescagemMode;

        void Awake()
        {
            // Quebra qualquer ToggleGroup herdado
            foreach (var tog in optionToggles)
                tog.group = null;

            // Inscreve um único listener para cada toggle:
            for (int i = 0; i < optionToggles.Length; i++)
            {
                int idx = i;
                var tog = optionToggles[idx];
                var txt = optionTexts[idx];


                tog.onValueChanged.AddListener(isOn =>
                {
                    // 1) controla máximo de 3
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

                    // 4) habilita o botão de confirmar apenas quando tiver exatamente 3
                    int onCount = 0;
                    foreach (var t in optionToggles)
                        if (t.isOn)
                            onCount++;
                    confirmButton.interactable = (onCount == 3);
                });
            }

            // Listeners de botões
            confirmButton.onClick.AddListener(OnConfirm);
            openButton.onClick.AddListener(OnOpen);
            closeButton.onClick.AddListener(OnClose);
        }

        void Start()
        {
            if (EmboscadaController.gameData == null)
            {
                EmboscadaController.gameData = new EmboscadaController.GameData();
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
                EmboscadaController.gameData.classificacao =
                    (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            }

            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            // desativa resultados / open view, ativa closed view
            resultPanel.SetActive(false);
            openPanel.SetActive(false);
            closedPanel.SetActive(true);
            isRepescagemMode = Fase2Manager.isRepescagemMode;
            // pé-de-obra do botão
            confirmButton.interactable = false;

            totalQuestions = Fase2Manager.instance.TotalPerguntas;

            // Inicializa o cronômetro
            InicializarCronometro();

            ShowNextQuestion();
        }

        void Update()
        {
            // Atualiza o cronômetro a cada frame
            AtualizarCronometro();
        }

        private void InicializarCronometro()
        {
            if (!cronometro)
            {
                Debug.LogError("Componente de imagem do cronômetro não atribuído!");
            }

            _tempoRestante = tempoLimite;
            _cronometroAtivo = true;
            UpdateCronometro();
        }

        private void UpdateCronometro()
        {
            if (cronometro != null)
            {
                cronometro.fillAmount = _tempoRestante / tempoLimite;
            }
        }

        private void AtualizarCronometro()
        {
            if (!_cronometroAtivo)
            {
                return;
            }

            _tempoRestante -= Time.deltaTime;
            _tempoRestante = Mathf.Max(_tempoRestante, 0f);
            UpdateCronometro();

            if (_tempoRestante <= 0f)
            {
                Debug.Log("Tempo esgotado!");
                _cronometroAtivo = false;
                OnTempoEsgotado();
            }
        }

        private void OnTempoEsgotado()
        {
            // Se o tempo acabou, o jogador perde a fase automaticamente
            Debug.Log("Tempo esgotado! Jogador perdeu a fase.");

            // Define uma taxa de acertos de 0% para garantir a derrota
            Fase2Manager.statusFase2 = 0f;

            // Mostra a tela de resultado com mensagem de derrota
            ShowTempoEsgotadoResult();
        }

        private IEnumerator RestartFase()
        {
            var op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            while (op.isDone == false)
            {
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }

        private void ShowTempoEsgotadoResult()
        {
            // Desativa o cronômetro e os painéis
            _cronometroAtivo = false;
            closedPanel.SetActive(false);
            openPanel.SetActive(false);
            resultPanel.SetActive(true);

            if (Fase2Manager.isRepescagemMode)
            {
                StartCoroutine(RestartFase());
            }
            else
            {
                var inst = Instantiate(prefabInstructions, null);
                var canvas = inst.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 5;
            }

            if (!Fase2Manager.isRepescagemMode)
            {
                Destroy(me);
            }
        }

        void ShowNextQuestion()
        {
            currentNoticia = Fase2Manager.instance.GetNextQuestion();
            if (currentNoticia == null)
            {
                ShowResult();
                return;
            }

            // Reinicia o cronômetro para a próxima pergunta
            _tempoRestante = tempoLimite;
            _cronometroAtivo = true;

            // Preenche closed view
            closedTitulo.text = currentNoticia.titulo;
            closedData.text = currentNoticia.data;
            closedLink.text = currentNoticia.linkFonte;
            closedConteudo.text = currentNoticia.conteudo;

            // Reseta toggles e cores
            for (int i = 0; i < optionToggles.Length; i++)
            {
                if (i < currentNoticia.opcoesResposta.Count)
                {
                    optionToggles[i].gameObject.SetActive(true);
                    optionToggles[i].isOn = false;
                    optionTexts[i].text = currentNoticia.opcoesResposta[i].texto;

                    var cb = optionToggles[i].colors;
                    cb.normalColor = normalColor;
                    cb.highlightedColor = normalColor;
                    cb.pressedColor = normalColor;
                    cb.selectedColor = selectedColor;
                    cb.disabledColor = normalColor;
                    optionToggles[i].colors = cb;
                }
                else
                {
                    optionToggles[i].gameObject.SetActive(false);
                }
            }

            // Preenche open view
            openTitulo.text = currentNoticia.titulo;
            openData.text = currentNoticia.data;
            openConteudo.text = currentNoticia.conteudo;
            openLink.text = currentNoticia.linkFonte;

            // desabilita o confirmar até o jogador escolher 3 novamente
            confirmButton.interactable = false;
        }

        void OnOpen()
        {
            openPanel.SetActive(true);
            // Pausa o cronômetro enquanto visualiza os detalhes
            _cronometroAtivo = false;
        }

        void OnClose()
        {
            openPanel.SetActive(false);
            // Retoma o cronômetro quando fecha os detalhes
            _cronometroAtivo = true;
        }

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
            // Desativa o cronômetro quando chega aos resultados
            _cronometroAtivo = false;

            closedPanel.SetActive(false);
            openPanel.SetActive(false);
            resultPanel.SetActive(true);

            float taxa = (float)totalCorrect / totalQuestions * 100f;
            Fase2Manager.statusFase2 = taxa;
            if (taxa > 50f)
            {
                resultText.text = $"🎉 Você venceu! Acertos: {totalCorrect}/{totalQuestions}";

                var inst = Instantiate(prefabInstructions, null);
                var canvas = inst.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 5;
            }
            else
            {
                if (Fase2Manager.isRepescagemMode)
                {
                    StartCoroutine(RestartFase());
                }
                else
                {
                    var inst = Instantiate(prefabInstructions, null);
                    var canvas = inst.GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = Camera.main;
                    canvas.sortingOrder = 5;
                }
            }

            if (!Fase2Manager.isRepescagemMode)
            {
                Destroy(me);
            }
        }
    }
}