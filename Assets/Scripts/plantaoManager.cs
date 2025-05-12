using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlantaoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoDisplay;
    private bool _videoReady = false;
    private bool _videoStarted = false;

    void Start()
    {
        StartCoroutine(LoadAll());
    }

    private IEnumerator LoadAll()
    {
        var steps = new List<Func<IEnumerator>>()
        {
            () => PrepareVideo(),
        };
        Debug.Log("Instance de LoadingScreenController: " + LoadingScreenController.Instance);
        Debug.Log("videoPlayer: " + videoPlayer);
        Debug.Log("videoDisplay: " + videoDisplay);

        yield return LoadingScreenController.Instance.ShowLoading(steps);

        videoDisplay.SetActive(true);
        videoPlayer.Play();
        StartCoroutine(CheckAndPlayAudio());
    }

    private IEnumerator PrepareVideo()
    {
        videoPlayer.prepareCompleted += vp => _videoReady = true;
        videoPlayer.Prepare();
        while (!_videoReady) yield return null;
    }

    private IEnumerator CheckAndPlayAudio()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!videoPlayer.isPlaying && videoPlayer != null && _videoStarted)
            {
                MainManager.main.ProximoCanvas();
                yield break;
            }
        }
    }


}
