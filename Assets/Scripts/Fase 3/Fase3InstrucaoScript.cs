using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fase_3
{
    public class Fase3InstrucaoScript : MonoBehaviour
    { 
        public event Action OnComplete;
        [Header("Áudio")]
        public AudioClip audioClip;
        public AudioSource audioSource;
        void Start() => StartCoroutine(RunAudio());
    
    
        private IEnumerator RunAudio()
        {
            var steps = new List<Func<IEnumerator>>()
            {
                () => PrepareAudio(audioClip),
            };

            yield return LoadingScreenController.Instance.ShowLoading(steps);
            audioSource.Play();
            yield return new WaitForSeconds(audioClip.length);
            OnComplete?.Invoke();
        }


        private IEnumerator PrepareAudio(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("Audio Fase 3: prologoClip não atribuído.");
                yield break;
            }
            audioSource.clip = audioClip;
        
            while (clip.loadState != AudioDataLoadState.Loaded)
            {
                if (clip.loadState == AudioDataLoadState.Failed)
                {
                    Debug.LogError("PrologoManager: Falha ao carregar os dados de áudio.");
                    yield break;
                }
                yield return null;
            }
        }
    }
}
