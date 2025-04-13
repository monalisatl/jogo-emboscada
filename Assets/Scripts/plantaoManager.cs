using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class PlantaoManager : MonoBehaviour{    
    private VideoPlayer videoPlayer;
    private GameObject config;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingProgressBar;
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private float fadeTime = 0.5f;
    private bool videoReady = false;
    private bool videoStarted = false;

    void Start()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
            
        if (loadingCanvasGroup == null && loadingScreen != null)
            loadingCanvasGroup = loadingScreen.GetComponent<CanvasGroup>();
            
        config = GameObject.Find("Video Player");
        if (config != null)
        {
            videoPlayer = config.GetComponent<VideoPlayer>();
        }
        
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer não encontrado no objeto 'Video Player'. Certifique-se de que o objeto existe e contém o componente VideoPlayer.");
            return;
        }
        
        // Configurar eventos do video player
        videoPlayer.prepareCompleted += VideoPrepared;
        
        // Iniciar o processo de preparação do vídeo
        StartCoroutine(PrepareVideo());
        
        // A coroutine para verificar o fim do vídeo será iniciada apenas após o vídeo começar
    }
    
    private IEnumerator PrepareVideo()
    {
        // Ocultar o video player enquanto prepara
        if (config != null)
            config.SetActive(false);
            
        // Preparar o vídeo
        videoPlayer.Prepare();
        
        // Atualizar a barra de progresso enquanto carrega
        while (!videoReady)
        {
            if (loadingProgressBar != null)
            {
                if (videoPlayer.frameCount > 0)
                    loadingProgressBar.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
                else
                    loadingProgressBar.fillAmount = Mathf.PingPong(Time.time * 0.5f, 1.0f); // Animação de carregamento
            }
            yield return null;
        }
        
        // Quando o vídeo estiver pronto, fazer a transição
        StartCoroutine(FadeOutLoading());
    }
    
    private IEnumerator FadeOutLoading()
    {
        // Mostrar o player de vídeo
        if (config != null)
            config.SetActive(true);
            
        // Fade out da tela de carregamento
        if (loadingCanvasGroup != null)
        {
            float startTime = Time.time;
            float endTime = startTime + fadeTime;
            
            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / fadeTime;
                loadingCanvasGroup.alpha = 1 - t;
                yield return null;
            }
            
            loadingCanvasGroup.alpha = 0;
        }
        
        // Desativar a tela de carregamento
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
            
        // Iniciar a reprodução do vídeo
        videoPlayer.Play();
        videoStarted = true;
        
        // Iniciar a coroutine para verificar quando o vídeo terminar
        StartCoroutine(CheckAndPlayAudio());
    }
    
    private void VideoPrepared(VideoPlayer vp)
    {
        videoReady = true;
        Debug.Log("Vídeo pronto para reprodução!");
    }

    private IEnumerator CheckAndPlayAudio()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!videoPlayer.isPlaying && videoPlayer != null && videoStarted)
            {
                mainManager.main.ProximoCanvas();
                yield break; // Encerra a coroutine quando o vídeo terminar
            }
        }
    }

    void Update()
    {
        // Lógica adicional se necessário
    }
}
