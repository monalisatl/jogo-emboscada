using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Fase_3
{
    public class Fase3Manager : MonoBehaviour
    {
        [Header("Timer")]
        [Tooltip("Tempo total compartilhado para responder as duas perguntas (em segundos)")]
        [SerializeField] private float tempoTotal = 120f;
        private float _tempoRestante;
        private bool _timerAtivo = false;
        private Image _timerImage; // Referência para a imagem do timer no prefab atual
    
        [Header("objetos raiz")] [SerializeField]
        private GameObject loadingScreen;
    
        [Header("Áudio de Instrução")] [SerializeField]
        private AudioClip instrucoesClip;

        [SerializeField] private AudioSource instrucaoAudioSource;
        [SerializeField] private GameObject instrucaoGO;
        private bool _skipInstrucao = false;
        [Header("Vídeo")] [SerializeField] private GameObject videoPrefab;
        [SerializeField] private VideoClip[] secretariosClip;

        [Header("Perguntas")] [SerializeField] private PerguntaScript pergunta1Prefab;
        [SerializeField] private PerguntaScript pergunta2Prefab;

        [Header("Resultado")] [SerializeField] private GameObject vitoriaPrefab;
        [SerializeField] private GameObject derrotaPrefab;
        private bool _resp1, _resp2;
    
        private PerguntaScript _perguntaAtual;
        private bool _tempoEsgotado = false;

        void Start()
        {
            if (LoadingScreenController.Instance == null)
            {
                DontDestroyOnLoad(loadingScreen);
            }
        
            _tempoRestante = tempoTotal;
        
            StartCoroutine(RunFase());
        }
    
        void Update()
        {
            if (_timerAtivo)
            {
                _tempoRestante -= Time.deltaTime;
            
                // Atualiza a UI do timer
                if (_timerImage != null)
                {
                    _timerImage.fillAmount = _tempoRestante / tempoTotal;
                }
            
                // Verifica se o tempo acabou
                if (_tempoRestante <= 0)
                {
                    _tempoRestante = 0;
                    _timerAtivo = false;
                    _tempoEsgotado = true;
                
                    Debug.Log("Tempo esgotado! O jogador perdeu.");
                    if (_perguntaAtual != null)
                    {
                        _perguntaAtual.ForceFinish(false);
                    }
                }
            }
        }

        private IEnumerator RunFase()
        {
            yield return LoadingScreenController.Instance.ShowLoading(new List<Func<IEnumerator>>
            {
                () => PrepareAudio(instrucoesClip)
            });
            instrucaoAudioSource.clip = instrucoesClip;
            instrucaoAudioSource.Play();
            yield return new WaitUntil(() =>
                _skipInstrucao
            );

            yield return PlayVideo(secretariosClip[0]);

            // Primeira pergunta
            yield return AskQuestion(pergunta1Prefab, correto => 
            {
                _resp1 = correto;
                _timerAtivo = false;
            });
            if (_tempoEsgotado)
            {
                yield return StartCoroutine(EndFase3());
                yield break;
            }

            yield return PlayVideo(secretariosClip[1]);

            // Segunda pergunta
            yield return AskQuestion(pergunta2Prefab, correto => 
            {
                _resp2 = correto;
                _timerAtivo = false;
            });

            yield return StartCoroutine(EndFase3());
        }

  private IEnumerator EndFase3()
{
    bool ganhou = (_resp1 | _resp2) && !_tempoEsgotado;
    
    if (ganhou)
    {
        SaveGame(true);
        MainManager.indiceCanvainicial = 29;
    }
    else
    {   
        SaveGame(false);
        MainManager.indiceCanvainicial = 12;
    }
    
    GameObject loadingInstance = Instantiate(loadingScreen);
    Canvas loadingCanvas = loadingInstance.GetComponent<Canvas>();
    if (loadingCanvas != null)
    {
        loadingCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        loadingCanvas.worldCamera = Camera.main;
        loadingCanvas.sortingOrder = 10;
    }
    loadingInstance.SetActive(true);
    
    AsyncOperation operation = SceneManager.LoadSceneAsync("main");
    operation.allowSceneActivation = false;
    
    Slider progressBar = loadingInstance.GetComponentInChildren<Slider>();
    float progressTarget = 0f;
    
    while (operation.progress < 0.9f)
    {
        progressTarget = operation.progress / 0.9f;
        if (progressBar != null)
        {
            float currentProgress = progressBar.value;
            progressBar.value = Mathf.Lerp(currentProgress, progressTarget, Time.deltaTime * 5f);
        }
        yield return null;
    }
    
    if (progressBar != null)
    {
        while (progressBar.value < 1f)
        {
            progressBar.value = Mathf.Lerp(progressBar.value, 1f, Time.deltaTime * 5f);
            if (progressBar.value >= 0.99f)
                progressBar.value = 1f;
            yield return null;
        }
    }
    yield return new WaitForSecondsRealtime(1f);
    operation.allowSceneActivation = true;
}


        private void SaveGame(bool b)
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisganhos[2] = b;
            int cls = PlayerPrefs.GetInt("classificacao", 0);
            if (b) cls++;
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao) cls;
            EmboscadaController.gameData.currentLevel = 31;
            PlayerPrefs.SetInt("nivel2", b ? 1 : 0);
            Debug.Log($"salvo nivel2 com valor:{b}\nbuscando o valor{PlayerPrefs.GetInt("nivel2")}");
            PlayerPrefs.SetInt("classificacao", cls);
            PlayerPrefs.SetInt("currentLevel", EmboscadaController.gameData.currentLevel);
            PlayerPrefs.Save();
        }

        private IEnumerator PrepareAudio(AudioClip clip)
        {
            if (clip.loadState != AudioDataLoadState.Loaded)
                clip.LoadAudioData();

            while (clip.loadState == AudioDataLoadState.Loading)
                yield return null;

            if (clip.loadState == AudioDataLoadState.Failed)
                Debug.LogError("Falha ao carregar áudio de instrução.");
        }

        private IEnumerator PlayVideo(VideoClip clip)
        {
            var vpGO = Instantiate(videoPrefab);
            var canva = vpGO.GetComponentInChildren<Canvas>();
            canva.renderMode   = RenderMode.ScreenSpaceCamera;
            canva.worldCamera  = Camera.main;
            var vp = vpGO.GetComponentInChildren<VideoPlayer>();
            vp.clip            = clip;
            vp.renderMode      = VideoRenderMode.CameraNearPlane;
            vp.targetCamera    = Camera.main;
        
            yield return LoadingScreenController.Instance.ShowLoading(new List<Func<IEnumerator>> {
                () => PrepareVideo(vp)
            });

            vpGO.SetActive(true);
            vp.Play();
            yield return new WaitUntil(() => !vp.isPlaying);

            Destroy(vpGO);
        }

        private IEnumerator PrepareVideo(VideoPlayer vp)
        {
            bool pronto = false;
            vp.prepareCompleted += _ => pronto = true;
            vp.Prepare();
            while (!pronto)
                yield return null;
        }

        private IEnumerator AskQuestion(PerguntaScript prefab, Action<bool> onAnswered)
        {
            var p = Instantiate(prefab);
            _perguntaAtual = p;
            var canva = p.GetComponent<Canvas>();
            canva.renderMode = RenderMode.ScreenSpaceCamera;
            canva.worldCamera = Camera.main;
            _timerImage = p.GetComponentInChildren<Image>(true);
            if (_timerImage == null || !_timerImage.CompareTag("Timer"))
            {
                Image[] images = p.GetComponentsInChildren<Image>(true);
                foreach (var img in images)
                {
                    if (img.name.Contains("Timer") || img.name.Contains("Cronometro"))
                    {
                        _timerImage = img;
                        break;
                    }
                }
            }
            if (_timerImage != null)
            {
                _timerImage.type = Image.Type.Filled;
                _timerImage.fillMethod = Image.FillMethod.Radial360;
                _timerImage.fillAmount = _tempoRestante / tempoTotal;
            }
            else
            {
                Debug.LogWarning("Imagem do timer não encontrada no prefab da pergunta!");
            }
        
            _timerAtivo = true;
        
            bool acabou = false;
            p.OnAnswered += correto => {
                onAnswered(correto);
                acabou = true;
                _perguntaAtual = null;
            };
        
            yield return new WaitUntil(() => acabou || _tempoEsgotado);
        
            if (_tempoEsgotado && !acabou)
            {
                onAnswered(false);
            }

            if (p != null)
            {
                Destroy(p.gameObject);
            }
        }

        public void OnPressSkipAudio()
        {
            _skipInstrucao = true;
            instrucaoAudioSource.Stop();
            instrucaoGO.SetActive(false);
            Destroy(instrucaoGO);
        }
    }
}
