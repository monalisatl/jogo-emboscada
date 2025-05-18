using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_5.Respescagem_Scritps.Fase_4
{
	public class EnigmaRepescagemScript : MonoBehaviour
	{
		public List<Enigma> enigmas;
		public GameObject puzzleUIPrefab;
		public Transform parentCanvas;
		public int cenaVitoria;
		public int cenaDerrota;	
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
			var go = Instantiate(puzzleUIPrefab, null, false);
			Canvas cv = go.GetComponent<Canvas>();
			cv.renderMode      = RenderMode.ScreenSpaceCamera;
			cv.worldCamera     = Camera.main;
			cv.sortingOrder    = 3;
			go.transform.SetAsLastSibling();
			var ui = go.GetComponentInChildren<EnigmaRepescagemUI>();
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
					SaveGame(respostasCorretas >= 2);
					MainManager.indiceCanvainicial = cenaVitoria;
					SceneManager.LoadSceneAsync("main");

				}
				else
				{
					SaveGame(respostasCorretas >= 2);
					MainManager.indiceCanvainicial = cenaDerrota;
					SceneManager.LoadSceneAsync("main");
				}
			}
		}

		private void SaveGame(bool acertou)
		{
			EmboscadaController.gameData ??= new EmboscadaController.GameData();
			EmboscadaController.gameData.niveisganhos[3] = acertou;
			int cls = PlayerPrefs.GetInt("classificacao", 0);
			if (acertou) cls++;
			EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)cls;

			// atualiza currentLevel (pode ser nivelIndex+1 ou outro valor de sua lógica)
			EmboscadaController.gameData.currentLevel = 37;

			// grava no PlayerPrefs
			PlayerPrefs.SetInt("nivel4", acertou ? 1 : 0);
			PlayerPrefs.SetInt("classificacao", cls);
			PlayerPrefs.SetInt("currentLevel", EmboscadaController.gameData.currentLevel);
			PlayerPrefs.Save();

			Debug.Log($"[SaveGame] Fase 4 passed={acertou}  cls={cls.ToString()}");

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
}
