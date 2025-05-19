using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_5
{
    public class VitoriaManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadingPrefab;
        [SerializeField] private string cenaCertificado = "FINAL_certificado";
    
        // Método para ser anexado ao botão na cena de vitória
        public void IrParaCertificado()
        {
            // Inicia a coroutine para carregar a cena do certificado com animação
            StartCoroutine(CarregarCena(cenaCertificado));
        }
    
        private System.Collections.IEnumerator CarregarCena(string nomeCena)
        {
            // Instancia o prefab de carregamento
            var loadI = Instantiate(loadingPrefab);
            loadI.SetActive(true);
        
            // Inicia a operação de carregamento assíncrono
            AsyncOperation operation = SceneManager.LoadSceneAsync(nomeCena);
            if (operation != null)
            {
                operation.allowSceneActivation = false;

                // Referência ao slider do prefab
                Slider progressBar = loadI.GetComponentInChildren<Slider>();

                // Mostra o progresso até 90%
                while (operation.progress < 0.9f)
                {
                    if (progressBar != null)
                    {
                        // Normaliza o valor para ir de 0 a 1 (0.9 é o máximo antes da allowSceneActivation)
                        float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
                        progressBar.value = progressValue;
                    }

                    yield return null;
                }

                // Quando chegar a 90% (que é o máximo antes da ativação da cena)
                if (progressBar != null)
                {
                    progressBar.value = 1f; // Completa visualmente a barra
                }

                // Aguarda um tempo para que o usuário veja a barra completa
                yield return new WaitForSecondsRealtime(1.0f);

                // Permite a ativação da cena
                operation.allowSceneActivation = true;
            }
        }
    }
}
