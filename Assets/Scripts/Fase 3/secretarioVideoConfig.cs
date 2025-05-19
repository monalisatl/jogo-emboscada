using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Fase_3
{
    public class secretarioVideoConfig : MonoBehaviour
    {
        public event Action OnVideoEnd;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject videoDisplay;

        void Start() => StartCoroutine(LoadAndPlay());

        private IEnumerator LoadAndPlay()
        {
            videoPlayer.prepareCompleted += vp => {
                videoDisplay.SetActive(true);
                vp.Play();
            };
            videoPlayer.loopPointReached += vp => OnVideoEnd?.Invoke();
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;
        }
    }
}