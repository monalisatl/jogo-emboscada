using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fase_5
{
    public class VideoToFase5Manager : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private Button skipButton;
        [SerializeField] private GameObject loadingPrefab;
        [SerializeField] private string targetSceneName = "fase5gameplay";
        [SerializeField] private GameObject videoDisplay;

        private bool videoFinished = false;
        private bool _videoReady = false;
        private bool _videoStarted = false;
        private void Start()
        {
            if (videoPlayer)
            { 
                
                StartCoroutine(LoadAll());
            }
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipButtonClicked);
            }
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
                _videoStarted = true;
                StartCoroutine(CheckVideoEnd());
            }
        }
        private IEnumerator PrepareVideo()
        {
            if (videoPlayer == null)
            {
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
        private IEnumerator CheckVideoEnd()
        {
            if (videoPlayer == null) yield break;
            yield return new WaitForSeconds(1f);
            while (true)
            {
                if (!videoPlayer.isPlaying && _videoStarted)
                {
                    OnVideoFinished(videoPlayer);
                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        private void OnVideoFinished(VideoPlayer vp)
        {
            if (videoFinished) return;
            videoFinished = true;
            StartCoroutine(LoadNextScene());
        }
        private void Update()
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                if (videoPlayer.time >= 1.0f && videoPlayer.time < videoPlayer.length - 0.5f && videoPlayer.isPlaying == false && !videoFinished)
                {
                    Debug.Log("Vídeo parou inesperadamente. Tentando retomar...");
                    videoPlayer.Play();
                }
            }
        }
        private void OnSkipButtonClicked()
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            videoFinished = true;
            StartCoroutine(LoadNextScene());
        }

        private IEnumerator LoadNextScene()
        {
            var loadI = Instantiate(loadingPrefab);
            loadI.SetActive(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
            while (!operation.isDone)
            {
                Slider progressBar = loadI.GetComponent<Slider>();
                if (progressBar != null)
                {
                    progressBar.value = operation.progress;
                }
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
        }

        private void OnDestroy()
        {
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoFinished;
            }
        }
    }
}
