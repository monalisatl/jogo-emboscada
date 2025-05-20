using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_4
{
    public class Fase4EndManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadingPrefab;
        [SerializeField] private string faseName;
        
        public void CarregarTela()
        {
            StartCoroutine(CarregarCena(faseName));
        }
        
        private System.Collections.IEnumerator CarregarCena(string nomeCena)
        {
            var loadI = Instantiate(loadingPrefab);
            loadI.SetActive(true);
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(nomeCena);

            while (operation is { isDone: false })
            {
                var progressBar = loadI.GetComponent<Slider>();
                if (progressBar is not null)
                {
                    progressBar.value = operation.progress;
                }
                yield return null;
            }
            
            yield return new WaitForSecondsRealtime(1.0f);
        }
    }
}
