using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Fase3Manager : MonoBehaviour
{
    [Header("objetos raiz")] [SerializeField]
    private GameObject loadingScreen;

    [SerializeField] private GameObject main;

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

        yield return StartCoroutine(EndFase3());
    }


    private IEnumerator EndFase3()
    {
        // 1) configura o índice
        if (_resp1 && _resp2)
            MainManager.indiceCanvainicial = 29;
        else
            MainManager.indiceCanvainicial = 45;
        var op = SceneManager.LoadSceneAsync("main");
        
        yield return LoadingScreenController.Instance.ShowLoading(op);
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
