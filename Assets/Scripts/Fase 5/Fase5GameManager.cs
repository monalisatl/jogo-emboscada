using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fase_5
{
    public class Fase5GameManager : MonoBehaviour
    {
        [Header("Telas")]
        [SerializeField] private GameObject telaExplicacaoInicial;  // Canvas com a explicação inicial
        [SerializeField] private GameObject telaMensagemErro;       // Canvas com mensagem de erro
        [SerializeField] private GameObject telaTempEsgotado;       // Canvas com mensagem de tempo esgotado
        [SerializeField] private GameObject loadingPrefab;
        
        [Header("Botões")]
        [SerializeField] private Button botaoPersonagem1;
        [SerializeField] private Button botaoPersonagem2;
        [SerializeField] private Button botaoPersonagem3;
        [SerializeField] private Button botaoComecarJogo;           // Botão "Next" na tela de explicação
        [SerializeField] private Button botaoTentarNovamente;       // Botão na tela de erro
        [SerializeField] private Button botaoTentarNovoTempo;       // Botão na tela de tempo esgotado
        
        [Header("Timer")]
        [SerializeField] private float tempoTotal = 30f;            // Tempo em segundos
        [SerializeField] private Image barraTimer;                  // Imagem do timer (tipo Fill)
        private float tempoRestante;
        private bool timerAtivo = false;
        
        [Header("Configurações")]
        [SerializeField] private int personagemVilaoIndex;          // 1, 2 ou 3 (qual é o vilão correto)
        [SerializeField] private string cenaVitoria = "fase5_vitoria";
        [SerializeField] private TextMeshProUGUI textoClassificacao;
        
        private EmboscadaController.Classificacao classificacaoAtual;  // Começa com classificação máxima
        
        private void saveload()
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            classificacaoAtual = EmboscadaController.gameData.classificacao;
            EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
            textoClassificacao.text = classificacaoAtual.ToString();
        }
        
        private void Start()
        {   
            saveload();
            telaExplicacaoInicial.SetActive(true);
            telaMensagemErro.SetActive(false);
            
            if (telaTempEsgotado != null)
                telaTempEsgotado.SetActive(false);
            
            if (botaoComecarJogo != null)
            {
                botaoComecarJogo.onClick.AddListener(ComecarJogo);
            }
            
            if (botaoTentarNovamente != null)
            {
                botaoTentarNovamente.onClick.AddListener(FecharMensagemErro);
            }
            
            if (botaoTentarNovoTempo != null)
            {
                botaoTentarNovoTempo.onClick.AddListener(ReiniciarTimer);
            }
            
            // Configura os botões de personagens
            ConfigurarBotoesPersonagens();
            
            // Configura o timer
            tempoRestante = tempoTotal;
            AtualizarTextoClassificacao();
            AtualizarVisualTimer();
        }
        
        private void Update()
        {
            if (timerAtivo)
            {
                if (tempoRestante > 0)
                {
                    tempoRestante -= Time.deltaTime;
                    AtualizarVisualTimer();
                }
                else
                {
                    TempoEsgotado();
                }
            }
        }
        
        private void AtualizarVisualTimer()
        {
            if (barraTimer != null)
            {
                barraTimer.fillAmount = tempoRestante / tempoTotal;
            }

        }
        
        private void TempoEsgotado()
        {
            timerAtivo = false;
            
            // Reduz a classificação se não estiver em 0
            if (classificacaoAtual > 0)
            {
                classificacaoAtual--;
            }
            
            AtualizarTextoClassificacao();
            
            // Mostra a tela de tempo esgotado
            if (telaTempEsgotado != null)
            {
                telaTempEsgotado.SetActive(true);
            }
        }
        
        private void ReiniciarTimer()
        {
            tempoRestante = tempoTotal;
            if (telaTempEsgotado != null)
            {
                telaTempEsgotado.SetActive(false);
            }
            timerAtivo = true;
            AtualizarVisualTimer();
        }
        
        private void ConfigurarBotoesPersonagens()
        {
            if (botaoPersonagem1 != null)
            {
                botaoPersonagem1.onClick.AddListener(() => SelecionarPersonagem(0));
            }
            
            if (botaoPersonagem2 != null)
            {
                botaoPersonagem2.onClick.AddListener(() => SelecionarPersonagem(1));
            }
            
            if (botaoPersonagem3 != null)
            {
                botaoPersonagem3.onClick.AddListener(() => SelecionarPersonagem(2));
            }
        }
        
        private void ComecarJogo()
        {
            // Esconde a tela de explicação e mostra o jogo
            telaExplicacaoInicial.SetActive(false);
            
            // Inicia o timer
            timerAtivo = true;
        }
        
        private void SelecionarPersonagem(int personagemIndex)
        {
            // Para o timer quando selecionar um personagem
            timerAtivo = false;
            
            if (personagemIndex == personagemVilaoIndex)
            {
                // Acertou o vilão
                StartCoroutine(CarregarCenaVitoria());
            }
            else
            {
                if (classificacaoAtual > 0)
                {
                    classificacaoAtual--;
                }
                AtualizarTextoClassificacao();
                MostrarMensagemErro();
            }
        }
        
        private void MostrarMensagemErro()
        {
            telaMensagemErro.SetActive(true);
        }
        
        private void FecharMensagemErro()
        {
            telaMensagemErro.SetActive(false);
            ReiniciarTimer();
        }
        
        private void AtualizarTextoClassificacao()
        {
            if (textoClassificacao)
            {
                textoClassificacao.text = classificacaoAtual.ToString();
            }
        }
        
        private IEnumerator CarregarCenaVitoria()
        {
            PlayerPrefs.SetInt("classificacaoFinal", (int)classificacaoAtual);
            PlayerPrefs.Save();
            
            var loadI = Instantiate(loadingPrefab);
            loadI.SetActive(true);
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(cenaVitoria);
            
            while (!operation.isDone)
            {
                var progressBar = loadI.GetComponent<Slider>();
                if (progressBar != null)
                {
                    progressBar.value = operation.progress;
                }
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
        }
    }
}
