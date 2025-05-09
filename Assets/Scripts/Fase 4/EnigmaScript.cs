using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;

public class EnigmaScript : MonoBehaviour
{
    public List<Enigma> enigmas;
    public GameObject puzzleUIPrefab;
    public Transform parentCanvas;
    public int cenaVitoria, cenaDerrota;
    [SerializeField] private GameObject Instruct;
    private int respostasCorretas = 0;
    private int totalRespondidos = 0;
    public AudioClip prologoClip;
    public AudioSource audioSource;
    public void SelecionarEnigma(int index)
    {
        var go = Instantiate(puzzleUIPrefab, parentCanvas);
        var ui = go.GetComponent<EnigmaUI>();
        ui.Inicializar(enigmas[index], OnEnigmaRespondido);
    }

    private void Start()
    {
        StartCoroutine(RunAudio());
    }

    private void OnEnigmaRespondido(bool acertou)
    {
        totalRespondidos++;
        if (acertou) respostasCorretas++;


        if (totalRespondidos >= enigmas.Count)
        {

            if (respostasCorretas >= 2)
            {
                if (mainManager.main == null)
                {
                    mainManager.main = new mainManager();

                    mainManager.indiceCanvainicial = cenaVitoria;
                    
                }
                else
                {
                    mainManager.main.InstanciarCanva(cenaVitoria);
                }
            }
            else
            {
                if (mainManager.main == null)
                {
                    mainManager.main = new mainManager();

                    mainManager.indiceCanvainicial = cenaDerrota;

                }
                else
                {
                    mainManager.main.InstanciarCanva(cenaDerrota);
                }
            }
        }
    }

    public void CloseInstruct()
    {
        if (Instruct != null)
        {
            Instruct.SetActive(false);
            Destroy(Instruct);
        }
    }

    private IEnumerator RunAudio()
    {
        var steps = new List<Func<IEnumerator>>()
        {
            () => PrepareAudio(prologoClip),
        };

        yield return LoadingScreenController.Instance.ShowLoading(steps);
        audioSource.Play();
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
}
