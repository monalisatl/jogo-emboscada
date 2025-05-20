using System;
using System.Collections;
using System.Collections.Generic;
using Fase_4;
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
            if (puzzleUIPrefab == null)
            {
                Debug.LogError("puzzleUIPrefab não foi atribuído no Inspector!");
                return;
            }

            if (enigmas == null || enigmas.Count == 0 || index >= enigmas.Count || enigmas[index] == null)
            {
                Debug.LogError($"Enigma inválido no índice {index}. Verifique a lista de enigmas!");
                return;
            }

            // Instanciar o prefab UI
            var go = Instantiate(puzzleUIPrefab, null, false);
            if (go == null)
            {
                Debug.LogError("Falha ao instanciar puzzleUIPrefab!");
                return;
            }

            // Configurar o canvas
            Canvas cv = go.GetComponent<Canvas>();
            if (cv != null)
            {
                cv.renderMode = RenderMode.ScreenSpaceCamera;
                cv.worldCamera = Camera.main;
                cv.sortingOrder = 3;
                go.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogWarning("Canvas não encontrado no prefab puzzleUIPrefab!");
            }

            // Buscar o componente UI de enigma
            var ui = go.GetComponentInChildren<EnigmaUI>();
            if (ui == null)
            {
                Debug.LogError("Componente EnigmaUI não encontrado no prefab! Verifique se o prefab contém este componente.");
                Destroy(go); // Destruir o objeto instanciado para evitar objetos órfãos
                return;
            }

            // Inicializar o enigma
            try
            {
                ui.Inicializar(enigmas[index], OnEnigmaRespondido, origemBtn);
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao inicializar o enigma: {e.Message}");
                Destroy(go);
            }
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
                retryBtn.onClick.AddListener(() => { StartCoroutine(RestartScene()); });
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
            // Instanciar a tela de loading com slider
            GameObject loadingInstance = Instantiate(loadingPrefab);
            loadingInstance.SetActive(true);

            // Obter referência ao Slider
            Slider progressBar = loadingInstance.GetComponentInChildren<Slider>();
            if (progressBar != null)
            {
                progressBar.value = 0f;
            }

            // Preparar e carregar o áudio com atualização de progresso
            yield return StartCoroutine(PrepareAudioWithProgress(prologoClip, progressBar));

            // Configurar o áudio depois de carregado
            if (audioSource != null && prologoClip != null)
            {
                audioSource.clip = prologoClip;

                if (progressBar != null)
                {
                    progressBar.value = 1f;
                    yield return new WaitForSecondsRealtime(1f); // Pequena pausa para ver o 100%
                }

                // Destruir a tela de carregamento e tocar o áudio
                Destroy(loadingInstance);
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("AudioSource ou prologoClip não configurados.");
                Destroy(loadingInstance);
            }
        }

// Novo método para carregar áudio com feedback de progresso
        private IEnumerator PrepareAudioWithProgress(AudioClip clip, Slider progressBar)
        {
            if (clip == null)
            {
                Debug.LogWarning("PrepareAudioWithProgress: clip não atribuído.");
                yield break;
            }

            // Iniciar carregamento
            clip.LoadAudioData();

            // Valores para animação suave da barra
            float targetProgress = 0f;
            float currentProgress = 0f;
            float progressVelocity = 0f;

            // Aguardar carregamento com atualização de progresso
            float loadingTime = 0f;
            while (clip.loadState != AudioDataLoadState.Loaded)
            {
                if (clip.loadState == AudioDataLoadState.Failed)
                {
                    Debug.LogError("PrepareAudioWithProgress: Falha ao carregar os dados de áudio.");
                    yield break;
                }

                // Simular progresso enquanto carrega (o Unity não fornece progresso real para carregamento de áudio)
                loadingTime += Time.deltaTime;
                targetProgress = Mathf.Clamp01(loadingTime / 3f); // Assume que carrega em aproximadamente 3 segundos

                // Atualizar o slider com suavidade se existir
                if (progressBar != null)
                {
                    currentProgress = Mathf.SmoothDamp(currentProgress, targetProgress, ref progressVelocity, 0.1f);
                    progressBar.value = currentProgress;
                }

                yield return null;
            }

            // Garantir que o progresso chegue a 100% quando o carregamento estiver completo
            if (progressBar != null)
            {
                while (progressBar.value < 0.99f)
                {
                    progressBar.value = Mathf.SmoothDamp(progressBar.value, 1f, ref progressVelocity, 0.1f);
                    yield return null;
                }

                progressBar.value = 1f;
            }

            Debug.Log("Áudio carregado com sucesso.");

        }
    }
}
