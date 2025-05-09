using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrologoManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject prologo;
    private TextMeshProUGUI textMesh;
    public string texto;

    [Header("Áudio")]
    public AudioClip prologoClip;
    public AudioSource audioSource;

    void Start()
    {
        textMesh = prologo.GetComponent<TextMeshProUGUI>();
        StartCoroutine(RunPrologo());
    }

    private IEnumerator RunPrologo()
    {
        var steps = new List<Func<IEnumerator>>()
        {
            () => PrepareAudio(GetPrologoClip()),
        };

        yield return LoadingScreenController.Instance.ShowLoading(steps);
        AtualizarTexto(texto);
        audioSource.clip = prologoClip;
        audioSource.Play();
    }

    private AudioClip GetPrologoClip()
    {
        return prologoClip;
    }

    private IEnumerator PrepareAudio(AudioClip prologoClip)
    {
        if (prologoClip == null)
        {
            Debug.LogWarning("PrologoManager: prologoClip não atribuído.");
            yield break;
        }

        prologoClip.LoadAudioData();
        while (prologoClip.loadState != AudioDataLoadState.Loaded)
        {
            if (prologoClip.loadState == AudioDataLoadState.Failed)
            {
                Debug.LogError("PrologoManager: Falha ao carregar os dados de áudio.");
                yield break;
            }
            yield return null;
        }
    }

    public void AtualizarTexto(string novoTexto)
    {
        if (textMesh != null)
            textMesh.text = novoTexto;
        else
            Debug.LogWarning("O componente TextMeshProUGUI não foi encontrado.");
    }
}
