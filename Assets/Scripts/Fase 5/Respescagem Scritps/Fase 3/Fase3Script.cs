using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Fase_5.Respescagem_Scritps.Fase_3
{
    public class Fase3RepescagemManager : MonoBehaviour
    {
        public static Fase3RepescagemManager instance;
        public static float statusFase3 = 0f;

        [Header("objetos raiz")] 
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject repescagem_aviso;
        [SerializeField] private GameObject repescagem_aviso_vitoria;
    
        [Header("Áudio de Instrução")] 
        [SerializeField] private AudioClip instrucoesClip;
        [SerializeField] private AudioSource instrucaoAudioSource;
        [SerializeField] private GameObject instrucaoGO;
        private bool _skipInstrucao = false;
        
        [Header("Vídeo")] 
        [SerializeField] private GameObject videoPrefab;
        [SerializeField] private VideoClip[] secretariosClip;

        [Header("Perguntas")] 
        [SerializeField] private PerguntaScript pergunta1Prefab;
        [SerializeField] private PerguntaScript pergunta2Prefab;

        private bool _resp1, _resp2;
        private bool fase3Ok = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            if (LoadingScreenController.Instance == null)
            {
                DontDestroyOnLoad(loadingScreen);
            }

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
            yield return new WaitUntil(() =>
                _skipInstrucao
            );

            yield return PlayVideo(secretariosClip[0]);

            yield return AskQuestion(pergunta1Prefab, correto => _resp1 = correto);

            yield return PlayVideo(secretariosClip[1]);

            yield return AskQuestion(pergunta2Prefab, correto => _resp2 = correto);

            yield return OnContinue();
        }

        private IEnumerator OnContinue()
        {
            statusFase3 = (_resp1 && _resp2) ? 100f : 0f;
            fase3Ok = statusFase3 >= 50f;

            if (!fase3Ok)
            {
                var obj = Instantiate(repescagem_aviso, null);
                obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                obj.GetComponent<Canvas>().worldCamera = Camera.main;
                obj.GetComponent<Canvas>().sortingOrder = 3;
                var btn = GameObject.FindWithTag("buttonFase3");
                if (btn != null)
                    btn.GetComponent<Button>().onClick.AddListener(OnRestart);
            }
            else
            {
                var obj = Instantiate(repescagem_aviso_vitoria, null);
                obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                obj.GetComponent<Canvas>().worldCamera = Camera.main;
                obj.GetComponent<Canvas>().sortingOrder = 3;
                SaveFase(true);
            }

            yield break;
        }

        public void OnRestart()
        {
            StartCoroutine(RestartScene());
        }

        private IEnumerator RestartScene()
        {
            // Destroi o singleton se existir
            if (instance)
                Destroy(instance.gameObject);

            var loadingObj = Instantiate(loadingScreen);

            var asyncOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    
            yield return LoadingScreenController.Instance.ShowLoading(asyncOp);
        }

        private void SaveFase(bool sucesso)
        {
            if (sucesso)
            {
                // Atualizar dados para repescagem da Fase 3
                Fase5Comeco.Repescagens[2] = true;
                EmboscadaController.gameData ??= new EmboscadaController.GameData();
                EmboscadaController.gameData.niveisRepescagem[2] = true;
                PlayerPrefs.SetInt("repescagem2", 1);
                PlayerPrefs.SetInt("currentLevel", 50);
                
;
            }
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
            var canva = p.GetComponent<Canvas>();
            canva.renderMode = RenderMode.ScreenSpaceCamera;
            canva.worldCamera = Camera.main;
            bool acabou = false;
            p.OnAnswered += correto => {
                onAnswered(correto);
                acabou = true;
            };
            yield return new WaitUntil(() => acabou);
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
