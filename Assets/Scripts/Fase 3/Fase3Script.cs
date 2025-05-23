using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Fase_5; // para RepescagemManager

namespace Fase_3
{

    public class Fase3Manager : MonoBehaviour
    {
        [Header("Timer")]
        [Tooltip("Tempo total compartilhado para responder as duas perguntas (em segundos)")]
        [SerializeField]
        private float tempoTotal = 30f;

        private float _tempoRestante;
        private bool _timerAtivo = false;
        private bool _tempoEsgotado = false;
        private Image _timerImage;

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
        [SerializeField] private GameObject pistaPrefab;
        [SerializeField] private bool debug = false;
        private PerguntaScript _perguntaAtual;
        private bool _resp1, _resp2;

        public static bool _isRepescagemMode = false;
        private const int ThisLevel = 2;

        void Start()
        {
            if (!LoadingScreenController.Instance)
                DontDestroyOnLoad(loadingScreen);

            _tempoRestante = tempoTotal;
            if (debug)
                _isRepescagemMode = debug;
            StartCoroutine(RunFase());
        }


        private IEnumerator RunFase()
        {
            yield return LoadingScreenController.Instance.ShowLoading(new List<Func<IEnumerator>>
            {
                () => PrepareAudio(instrucoesClip)
            });

            instrucaoAudioSource.clip = instrucoesClip;
            instrucaoAudioSource.Play();
            yield return new WaitUntil(() => _skipInstrucao);

            yield return PlayVideo(secretariosClip[0]);

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
            _timerAtivo = true;
            // Segunda pergunta
            yield return AskQuestion(pergunta2Prefab, correto =>
            {
                _resp2 = correto;
                _timerAtivo = false;
            });

            yield return StartCoroutine(EndFase3());
        }

        void Update()
        {
            if (_timerAtivo)
            {
                _tempoRestante -= Time.deltaTime;
                if (_timerImage != null)
                    _timerImage.fillAmount = _tempoRestante / tempoTotal;

                if (_tempoRestante <= 0)
                {
                    _tempoRestante = 0;
                    _timerAtivo = false;
                    _tempoEsgotado = true;
                    if (_perguntaAtual != null)
                        _perguntaAtual.ForceFinish(false);
                }
            }
        }

        private IEnumerator EndFase3()
        {
            bool ganhou = (_resp1 || _resp2) && !_tempoEsgotado;

            if (_isRepescagemMode)
            {
                if (ganhou)
                {
                    TelaVitoria();
                }
                else
                {
                    TelaDerrota();
                }

                yield break;
            }

            SaveGame(ganhou);
            // MODO NORMAL: carrega a MAIN como antes
            MainManager.indiceCanvainicial = ganhou ? 29 : 12;
            GameObject loadingInstance = Instantiate(loadingScreen);
            var loadingCanvas = loadingInstance.GetComponent<Canvas>();
            if (loadingCanvas != null)
            {
                loadingCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                loadingCanvas.worldCamera = Camera.main;
                loadingCanvas.sortingOrder = 10;
            }

            loadingInstance.SetActive(true);

            AsyncOperation op = SceneManager.LoadSceneAsync("main");
            op.allowSceneActivation = false;
            Slider progressBar = loadingInstance.GetComponentInChildren<Slider>();

            while (op.progress < 0.9f)
            {
                float p = op.progress / 0.9f;
                if (progressBar != null)
                    progressBar.value = Mathf.Lerp(progressBar.value, p, Time.deltaTime * 5f);
                yield return null;
            }

            if (progressBar != null)
            {
                while (progressBar.value < 1f)
                {
                    progressBar.value = Mathf.Lerp(progressBar.value, 1f, Time.deltaTime * 5f);
                    if (progressBar.value >= 0.99f) progressBar.value = 1f;
                    yield return null;
                }
            }

            yield return new WaitForSecondsRealtime(1f);
            op.allowSceneActivation = true;
        }

        private void TelaVitoria()
        {
            PlayerPrefs.SetInt($"repescagem{ThisLevel}", 0);
            PlayerPrefs.SetInt("nivel2", 1);
            PlayerPrefs.SetInt("currentLevel", 83);
            PlayerPrefs.Save();
            RepescagemManager.Clear();
            var vitoria = Instantiate(vitoriaPrefab);
            var canvas = vitoria.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 5;
            }

            var btn = vitoria.GetComponentInChildren<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Destroy(vitoria.gameObject);
                    CarregarPista();
                });
            }
        }

        private void CarregarPista()
        {
            var pista = Instantiate(pistaPrefab);
            var canvas = pista.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 5;
            }

            var btn = pista.GetComponentInChildren<Button>();
            if (!btn) return;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { StartCoroutine(CarregaVitoria()); });
        }

        private IEnumerator CarregaVitoria()
        {
            _isRepescagemMode = false;
            const string nameScene = "faserepescagem";
            var li = Instantiate(loadingScreen);
            var load = li.GetComponent<Canvas>();
            if (load != null)
            {
                load.renderMode = RenderMode.ScreenSpaceCamera;
                load.worldCamera = Camera.main;
                load.sortingOrder = 10;
            }

            li.SetActive(true);
            AsyncOperation opt = SceneManager.LoadSceneAsync(nameScene);
            opt.allowSceneActivation = false;
            Slider prBar = li.GetComponentInChildren<Slider>();

            while (opt.progress < 0.9f)
            {
                float p = opt.progress / 0.9f;
                if (prBar != null)
                    prBar.value = Mathf.Lerp(prBar.value, p, Time.deltaTime * 5f);
                yield return null;
            }

            if (prBar != null)
            {
                while (prBar.value < 1f)
                {
                    prBar.value = Mathf.Lerp(prBar.value, 1f, Time.deltaTime * 5f);
                    if (prBar.value >= 0.99f) prBar.value = 1f;
                    yield return null;
                }
            }

            yield return new WaitForSecondsRealtime(1f);
            opt.allowSceneActivation = true;
        }

        private void TelaDerrota()
        {
            var derrota = Instantiate(derrotaPrefab);
            var canvas = derrota.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 5;
            }

            var btn = derrota.GetComponentInChildren<Button>();
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => { StartCoroutine(CarregarDerrota()); });
            }
        }

        private IEnumerator CarregarDerrota()
        {
            GameObject li = Instantiate(loadingScreen);
            var load = li.GetComponent<Canvas>();
            if (load != null)
            {
                load.renderMode = RenderMode.ScreenSpaceCamera;
                load.worldCamera = Camera.main;
                load.sortingOrder = 10;
            }

            li.SetActive(true);
            AsyncOperation opt = SceneManager.LoadSceneAsync(gameObject.scene.name);
            opt.allowSceneActivation = false;
            Slider prBar = li.GetComponentInChildren<Slider>();

            while (opt.progress < 0.9f)
            {
                float p = opt.progress / 0.9f;
                if (prBar != null)
                    prBar.value = Mathf.Lerp(prBar.value, p, Time.deltaTime * 5f);
                yield return null;
            }

            if (prBar != null)
            {
                while (prBar.value < 1f)
                {
                    prBar.value = Mathf.Lerp(prBar.value, 1f, Time.deltaTime * 5f);
                    if (prBar.value >= 0.99f) prBar.value = 1f;
                    yield return null;
                }
            }

            yield return new WaitForSecondsRealtime(1f);
            opt.allowSceneActivation = true;
        }

        private void SaveGame(bool ok)
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisganhos[2] = ok;
            int cls = PlayerPrefs.GetInt("classificacao", 0);
            if (ok) cls++;
            EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)cls;
            EmboscadaController.gameData.currentLevel = 31;
            PlayerPrefs.SetInt("nivel2", ok ? 1 : 0);
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
            canva.renderMode = RenderMode.ScreenSpaceCamera;
            canva.worldCamera = Camera.main;
            var vp = vpGO.GetComponentInChildren<VideoPlayer>();
            vp.clip = clip;
            vp.renderMode = VideoRenderMode.RenderTexture;

            yield return LoadingScreenController.Instance.ShowLoading(new List<Func<IEnumerator>>
            {
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
            _timerImage = GameObject.FindWithTag("Timer").GetComponent<Image>();

            // Verificar se encontrou alguma imagem para o timer
            if (_timerImage == null)
                Debug.LogError("Não foi possível encontrar o sprite do timer!");
            else
                Debug.Log("Timer sprite encontrado: " + _timerImage.name);

            _timerAtivo = true;

            bool acabou = false;
            p.OnAnswered += correto =>
            {
                onAnswered(correto);
                acabou = true;
                _perguntaAtual = null;
            };

            yield return new WaitUntil(() => acabou || _tempoEsgotado);

            if (_tempoEsgotado && !acabou)
                onAnswered(false);

            if (p != null)
                Destroy(p.gameObject);
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