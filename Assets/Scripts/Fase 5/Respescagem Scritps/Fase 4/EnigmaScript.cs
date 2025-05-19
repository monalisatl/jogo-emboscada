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
        [SerializeField] private GameObject loadingPrefab;
        [SerializeField] private GameObject vitoriaPrefab; // Prefab com Fase4VitoriaScript
        [SerializeField] private GameObject derrotaUI;
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
                // Verificar resultado final
                bool passou = respostasCorretas >= 2;
                
                if (passou)
                {
                    // Mostra tela de vitória com pista
                    MostrarTelaVitoria();
                }
                else
                {
                    // Mostra tela de derrota com opção de tentar novamente
                    MostrarTelaDerrota();
                }
            }
        }
        
        private void MostrarTelaVitoria()
        {
            // Instancia o prefab de vitória que contém o Fase4VitoriaScript
            var vitoria = Instantiate(vitoriaPrefab, null);
            Canvas canvas = vitoria.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.sortingOrder = 5;
            }
            
            // O script Fase4VitoriaScript vai lidar com o salvamento e navegação
        }
        
        private void MostrarTelaDerrota()
        {
            GameObject derrota = Instantiate(derrotaUI, null);
            Canvas canvas = derrota.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingOrder = 5;
            
            // Configurar botão para tentar novamente
            Button retryBtn = derrota.GetComponentInChildren<Button>();
            if (retryBtn)
            {
                retryBtn.onClick.AddListener(() => {
                    StartCoroutine(RestartScene());
                });
            }
        }

        private IEnumerator RestartScene()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            
            var loadI = Instantiate(loadingPrefab);
            loadI.SetActive(true);
            
            while (!op.isDone)
            {
                loadI.GetComponent<Slider>().value = op.progress;
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
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
