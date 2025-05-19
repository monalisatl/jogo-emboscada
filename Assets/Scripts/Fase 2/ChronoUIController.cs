using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_2
{
    public class ChronoUIController : MonoBehaviour
    {
        [Header("Timer")]
        [Tooltip("Tempo máximo para completar a ordenação (em segundos)")]
        public float tempoLimite = 120f;
        [Tooltip("Referência ao objeto de imagem do cronômetro")]
        public Image cronometro;
        
        private float _tempoRestante;
        private bool _cronometroAtivo = true;

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
        
        [Header("Painel de tempo esgotado")]
        public GameObject timeoutPanel;
        public Button timeoutButton;

        [Header("Cores")]
        public Color selectedColor = Color.green;

        private List<Noticia> listaNoticias;
        private List<Noticia> ordemSelecionada = new List<Noticia>();
        private int nextChronoIndex = 1;
        private Color defaultButtonColor;
    
        [SerializeField] private TextMeshProUGUI credencial;

        void Start()
        {
            if(EmboscadaController.gameData == null)
            {
                EmboscadaController.gameData = new EmboscadaController.GameData();
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
                EmboscadaController.gameData.classificacao =
                    (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            }
            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            // pega as 4 notícias da fase 1
            listaNoticias = Fase2Manager.perguntasSelecionadas;

            // config painel completação
            completePanel.SetActive(false);
            continueButton.onClick.AddListener(OnContinue);
            resetButton.onClick.AddListener(OnReset);
            
            // config painel de tempo esgotado (se existir)
            if (timeoutPanel != null)
            {
                timeoutPanel.SetActive(false);
                if (timeoutButton != null)
                    timeoutButton.onClick.AddListener(OnTimeoutContinue);
            }

            // salva cor preta padrão do botão (assume todos iguais)
            if (newsButtons.Length > 0)
                defaultButtonColor = newsButtons[0].GetComponent<Image>().color;

            // fecha detalhe
            openPanel.SetActive(false);
            closeButton.onClick.AddListener(() => {
                openPanel.SetActive(false);
                // Retoma o cronômetro quando fecha os detalhes
                _cronometroAtivo = true;
            });

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
            
            // Inicializa o cronômetro
            InicializarCronometro();
        }
        
        void Update()
        {
            // Atualiza o cronômetro a cada frame
            AtualizarCronometro();
        }
        
        private void InicializarCronometro()
        {
            if (cronometro == null)
            {
                Debug.LogError("Componente de imagem do cronômetro não atribuído!");
                return;
            }
            _tempoRestante = tempoLimite;
            _cronometroAtivo = true;
            UpdateCronometro();
        }
        
        private void UpdateCronometro()
        {
            if (cronometro != null)
            {
                cronometro.fillAmount = _tempoRestante/tempoLimite;
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
            
            // Desativa interação com os botões
            foreach (var button in newsButtons)
            {
                button.interactable = false;
            }
            openPanel.SetActive(false);
            completePanel.SetActive(false);
            if (timeoutPanel != null)
            {
                timeoutPanel.SetActive(true);
            }
            else
            {
                OnTimeoutContinue();
            }
        }
        
        private void OnTimeoutContinue()
        {
            SaveFase(false);
            MainManager.indiceCanvainicial = 11;
            SceneManager.LoadSceneAsync("main");
        }

        void OpenDetail(int i)
        {
            var n = listaNoticias[i];
            openPanel.SetActive(true);
            openTitulo.text   = n.titulo;
            openData.text     = n.data;
            openConteudo.text = n.conteudo;
            openLink.text     = n.linkFonte;
            
            // Pausa o cronômetro enquanto visualiza os detalhes
            _cronometroAtivo = false;
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
            // Pausa o cronômetro quando completa a ordenação
            _cronometroAtivo = false;
            
            feedbackText.text = "Ordenação completa!";
            completePanel.SetActive(true);
        }
        
        private float VerificarResultado()
        {
            List<Noticia> correta = listaNoticias
                .OrderBy(n => n.FormaterData)
                .ToList();
            int acertos = 0;
            for (int i = 0; i < ordemSelecionada.Count; i++)
                if (ordemSelecionada[i] == correta[i])
                    acertos++;
            float percent = (float)acertos / listaNoticias.Count * 100f;
            Debug.Log($"Ordenação: {acertos}/{listaNoticias.Count} → {percent}%");
            return percent;
        }

        void OnContinue()
        {
            float faseQuizOk = Fase2Manager.statusFase2;
            float ordenacaoPct = VerificarResultado();
        
            bool fase2Ok = ((faseQuizOk+ordenacaoPct)/2 >= 50f);
        
            SaveFase(fase2Ok);

            if (fase2Ok)
            {
                MainManager.indiceCanvainicial = 21;
            }
            else
            {
                MainManager.indiceCanvainicial = 11;
            }
            SceneManager.LoadSceneAsync("main");
        }

        void OnReset()
        {
            // Reinicia a ordenação e o cronômetro
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
            
            // Reinicia o cronômetro
            _tempoRestante = tempoLimite;
            _cronometroAtivo = true;
        }

        private void SaveFase(bool status)
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisganhos[1] = status;
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            var cls = (int)EmboscadaController.gameData.classificacao;
       
            if (status)
            {
                cls++;
                EmboscadaController.gameData.niveisganhos[1] = true;
            }
            else
            {
                EmboscadaController.gameData.niveisganhos[1] = false;
            }
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)cls;
            Debug.Log("Salvando fase 2: " + status + " Terminou com classificacao"+ EmboscadaController.gameData.classificacao);
            EmboscadaController.gameData.currentLevel = 23;
            PlayerPrefs.SetInt("nivel1", EmboscadaController.gameData.niveisganhos[1] ? 1 : 0);
            Debug.Log($"salvo nivel1 com valor:{EmboscadaController.gameData.niveisganhos[1]}\nbuscando o valor{PlayerPrefs.GetInt("nivel1")}");
            PlayerPrefs.SetInt("currentLevel", 23);
            PlayerPrefs.SetInt("classificacao", cls);
            PlayerPrefs.Save();
        }
    }
}
