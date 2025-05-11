using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Serialization;

public class EnigmaScript : MonoBehaviour
{
	public List<Enigma> enigmas;
	public GameObject puzzleUIPrefab;
	public Transform parentCanvas;
	public int cenaVitoria, cenaDerrota;	
    [SerializeField] private GameObject instruct;
	private int respostasCorretas = 0;
	private int totalRespondidos = 0;
	public AudioClip prologoClip;
	public AudioSource audioSource;
    public Button[] botoesEnigmas;


    void Start()
    {
        StartCoroutine(RunAudio());
        // liga cada botão ao índice
        for (int i = 0; i < botoesEnigmas.Length; i++)
        {
            int idx = i;
            botoesEnigmas[i].onClick.RemoveAllListeners();
            botoesEnigmas[i].onClick.AddListener(() =>
                SelecionarEnigma(idx, botoesEnigmas[idx])
            );
        }
    }

    public void SelecionarEnigma(int index, Button origemBtn)
    {
        var go = Instantiate(puzzleUIPrefab);
        go.transform.SetParent(null, false); 
        Canvas cv = go.GetComponent<Canvas>();
        cv.renderMode      = RenderMode.ScreenSpaceCamera;
        cv.worldCamera     = Camera.main;
        cv.sortingOrder    = 3;
        go.transform.SetAsLastSibling();
        var ui = go.GetComponentInChildren<EnigmaUI>();
        ui.Inicializar(enigmas[index], OnEnigmaRespondido, origemBtn);
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

                }
                mainManager.indiceCanvainicial = cenaVitoria;
                SceneManager.LoadScene("main");

            }
			else
			{
				if (mainManager.main == null)
				{
					mainManager.main = new mainManager();

                }
                mainManager.indiceCanvainicial = cenaDerrota;
                SceneManager.LoadScene("main");
            }
		}
	}

	public void CloseInstruct()
	{
		if (instruct != null)
		{
			instruct.SetActive(false);
			Destroy(instruct);
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
			Debug.LogWarning("LoadAudio: prologoClip não atribuído.");
			yield break;
		}

		prologoClip.LoadAudioData();
		while (prologoClip.loadState != AudioDataLoadState.Loaded)
		{
			if (prologoClip.loadState == AudioDataLoadState.Failed)
			{
				Debug.LogError("LoadAudio: Falha ao carregar os dados de áudio.");
				yield break;
			}
			yield return null;
		}
	}
}
