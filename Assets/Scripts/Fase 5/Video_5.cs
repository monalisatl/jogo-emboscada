using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fase_5
{
    public class VideoToFase5Manager : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private Button skipButton;
        [SerializeField] private GameObject loadingPrefab;
        [SerializeField] private string targetSceneName = "fase5gameplay";

        private bool videoFinished = false;
        
        private void Start()
        {
            // Configurar o evento para quando o vídeo terminar
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached += OnVideoFinished;
                videoPlayer.Play();
            }
            
            // Configurar o botão de pular/continuar
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipButtonClicked);
            }
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            // Evita que este método seja chamado várias vezes
            if (videoFinished) return;
            videoFinished = true;
            
            // Carrega a próxima cena após o vídeo terminar
            StartCoroutine(LoadNextScene());
        }

        private void OnSkipButtonClicked()
        {
            // Interrompe o vídeo se estiver reproduzindo
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            
            // Evita que LoadNextScene seja chamado novamente quando o vídeo terminar
            videoFinished = true;
            
            // Carrega a próxima cena quando o botão for clicado
            StartCoroutine(LoadNextScene());
        }

        private IEnumerator LoadNextScene()
        {
            // Instancia a tela de loading
            var loadI = Instantiate(loadingPrefab);
            loadI.SetActive(true);
            
            // Inicia o carregamento assíncrono
            AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
            
            // Atualiza o progresso da barra de loading
            while (!operation.isDone)
            {
                Slider progressBar = loadI.GetComponent<Slider>();
                if (progressBar != null)
                {
                    progressBar.value = operation.progress;
                }
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
        }

        private void OnDestroy()
        {
            // Remove o evento ao destruir este objeto para evitar vazamentos de memória
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoFinished;
            }
        }
    }
}
