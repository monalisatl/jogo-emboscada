using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlantaoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoDisplay;
    [SerializeField] private GameObject loadingPrefab;
    
    private bool _videoReady = false;
    private bool _videoStarted = false;

    void Start()
    {
        // Inicia o processo de carregamento do vídeo
        StartCoroutine(LoadAll());
    }

    private IEnumerator LoadAll()
    {
        // Mostra a tela de loading enquanto prepara o vídeo
        GameObject loadingInstance = null;
        if (loadingPrefab != null)
        {
            loadingInstance = Instantiate(loadingPrefab);
            loadingInstance.SetActive(true);
        }
        
        // Lista de passos para a inicialização
        var steps = new List<Func<IEnumerator>>()
        {
            () => PrepareVideo(),
        };
        
        // Executa cada passo de inicialização
        foreach (var step in steps)
        {
            yield return StartCoroutine(step());
        }
        
        // Remove a tela de loading se estiver presente
        if (loadingInstance != null)
        {
            Destroy(loadingInstance);
        }
        
        // Ativa o display do vídeo e começa a reprodução
        if (videoDisplay != null)
        {
            videoDisplay.SetActive(true);
        }
        
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            _videoStarted = true;
            
            // Inicia a verificação para detectar quando o vídeo terminar
            StartCoroutine(CheckVideoEnd());
        }
        else
        {
            // Caso não haja vídeo configurado, avança direto
            AvançarParaProximoCanvas();
        }
    }

    private IEnumerator PrepareVideo()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("VideoPlayer não configurado!");
            yield break;
        }
        
        // Configura o callback para quando o vídeo estiver pronto
        videoPlayer.prepareCompleted += vp => _videoReady = true;
        
        // Inicia a preparação do vídeo
        videoPlayer.Prepare();
        
        // Aguarda até que o vídeo esteja pronto
        float timeoutSeconds = 10f; // timeout de segurança
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

    private IEnumerator CheckVideoEnd()
    {
        if (videoPlayer == null) yield break;
        
        // Aguarda um pequeno intervalo inicial para garantir que o vídeo começou
        yield return new WaitForSeconds(0.5f);
        
        while (true)
        {
            // Verifica se o vídeo está tocando
            if (!videoPlayer.isPlaying && _videoStarted)
            {
                // O vídeo terminou, avança para o próximo canvas
                AvançarParaProximoCanvas();
                yield break;
            }
            
            // Verifica a cada 0.5 segundos
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void AvançarParaProximoCanvas()
    {
        // Verifica se o MainManager existe
        if (MainManager.main != null)
        {
            MainManager.main.ProximoCanvas();
        }
        else
        {
            Debug.LogError("MainManager não encontrado! Não foi possível avançar para o próximo canvas.");
        }
    }

    public void PularVideo()
    {
        if (videoPlayer != null && _videoStarted)
        {
            videoPlayer.Stop();
            AvançarParaProximoCanvas();
        }
    }
}
