using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fase3Script : MonoBehaviour
{
    [Header("Áudio")]
    public AudioClip audioClip;
    public AudioSource audioSource;
    void Start()
    {
        StartCoroutine(RunAudio());
    }
    
    public void onClickFase3()
    {
        SceneManager.LoadScene("11_fase1_minigame");
    }
    
    
    private IEnumerator RunAudio()
    {
        var steps = new List<Func<IEnumerator>>()
        {
            () => PrepareAudio(audioClip),
        };

        yield return LoadingScreenController.Instance.ShowLoading(steps);
        audioSource.Play();
    }


    private IEnumerator PrepareAudio(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Audio Fase 3: prologoClip não atribuído.");
            yield break;
        }

        
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
