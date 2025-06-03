using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoControl : MonoBehaviour
{
 [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoDisplay;
    [SerializeField] private GameObject loadingPrefab;
    [SerializeField] private string videoname;
    private bool _videoReady = false;

    void Start()
    {
        // Inicia o processo de carregamento do vídeo
        StartCoroutine(LoadAll());
    }

    private IEnumerator LoadAll()
    {
        GameObject loadingInstance = null;
        if (loadingPrefab != null)
        {
            loadingInstance = Instantiate(loadingPrefab);
            loadingInstance.SetActive(true);
        }
        var steps = new List<Func<IEnumerator>>()
        {
            () => PrepareVideo(),
        };
        
        foreach (var step in steps)
        {
            yield return StartCoroutine(step());
        }
        
        if (loadingInstance != null)
        {
            Destroy(loadingInstance);
        }
        if (videoDisplay != null)
        {
            videoDisplay.SetActive(true);
        }
        
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            
        }
        else
        {
            AvançarParaProximoCanvas();
        }
    }

    private IEnumerator PrepareVideo()
    {
        if (videoname != null && videoPlayer != null)
        {
            string url = System.IO.Path.Combine(Application.streamingAssetsPath, videoname);
            Debug.Log($"Preparando vídeo: {url}");
            videoPlayer.url = url;
        }
        else{
            Debug.LogWarning("VideoPlayer não configurado!");
            yield break;
        }
        videoPlayer.prepareCompleted += vp => _videoReady = true;
        videoPlayer.Prepare();
        float timeoutSeconds = 10f;
        float elapsed = 0f;
        
        while (!_videoReady && elapsed < timeoutSeconds)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (!_videoReady)
        {
            Debug.LogWarning("Timeout ao preparar o vídeo!");
        }
    }
    

    private void AvançarParaProximoCanvas()
    {
        if (MainManager.main != null)
        {
            MainManager.main.ProximoCanvas();
        }
        else
        {
            Debug.LogError("MainManager não encontrado! Não foi possível avançar para o próximo canvas.");
        }
    }
    
}
