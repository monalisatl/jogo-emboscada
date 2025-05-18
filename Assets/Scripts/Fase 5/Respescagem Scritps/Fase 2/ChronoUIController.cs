using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_5.Respescagem_Scritps.Fase_2
{
    public class ChronoUIControllerRepescagem : MonoBehaviour
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
        
        [Header("repescagem")]
        private bool fase2Ok = false;
        public GameObject repescagem_aviso;
        public GameObject repescagem_aviso_vitoria;
        
    
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
            listaNoticias = Fase2ManagerRepescagem.perguntasSelecionadas;

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
                var lp = newsButtons[i].gameObject.GetComponent<LongPressButtonRepescagem>();
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
            float percent = (float)acertos / listaNoticias.Count * 100f;
            Debug.Log($"Ordenação: {acertos}/{listaNoticias.Count} → {percent}%");
            return percent;
        }

        void OnContinue()
        {
            float faseQuizOk    = Fase2ManagerRepescagem.statusFase2;
            float ordenacaoPct  = verificarResultado();
            fase2Ok  = ((faseQuizOk + ordenacaoPct) / 2f >= 50f);

            if (!fase2Ok)
            {
                var obj = Instantiate(repescagem_aviso, null);
                obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                obj.GetComponent<Canvas>().worldCamera = Camera.main;
                obj.GetComponent<Canvas>().sortingOrder = 3;

            }
            else
            {
                var obj = Instantiate(repescagem_aviso_vitoria, null);
                obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                obj.GetComponent<Canvas>().worldCamera = Camera.main;
                obj.GetComponent<Canvas>().sortingOrder = 3;
                
            }
            SaveFase(true);
            MainManager.indiceCanvainicial = 50;
            SceneManager.LoadSceneAsync("main");
        }

        public void OnRestart()
        {
            if (Fase2ManagerRepescagem.instance)
                Destroy(Fase2ManagerRepescagem.instance.gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void OnExit()
        {
            if (Fase2ManagerRepescagem.instance)
                Destroy(Fase2ManagerRepescagem.instance.gameObject);
            SceneManager.LoadScene("");
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

        private void SaveFase(bool status)
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisRepescagem[1] = status;
            var cls = (int)EmboscadaController.gameData.classificacao;
            Debug.Log("Salvando fase 2 da repescagem: " + status);
            EmboscadaController.gameData.currentLevel = 51; //
            PlayerPrefs.SetInt("repescagem" + 1, EmboscadaController.gameData.niveisganhos[1] ? 1 : 0);
            PlayerPrefs.SetInt("currentLevel", 50);
            PlayerPrefs.Save();
        }
    }
}
