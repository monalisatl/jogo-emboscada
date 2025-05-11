using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class secretarioVideoConfig : MonoBehaviour
{
    [SerializeField] private VideoClip[] secretariosClip;
    static int index = 0;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoDisplay;
    private bool _videoReady = false;
    private bool _videoStarted = false;
    void Start()
    {
        StartCoroutine(LoadAll());
    }
    void Update()
    {
        
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
        StartCoroutine(CheckAndPlayVideo());
    }

    private IEnumerator PrepareVideo()
    {
        videoPlayer.prepareCompleted += vp => _videoReady = true;
        videoPlayer.Prepare();
        while (!_videoReady) yield return null;
    }

    private IEnumerator CheckAndPlayVideo()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!videoPlayer.isPlaying && videoPlayer != null && _videoStarted)
            {
                
                yield break;
            }
        }
    }

}
