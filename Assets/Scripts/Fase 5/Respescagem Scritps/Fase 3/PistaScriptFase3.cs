using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_5.Respescagem_Scritps.Fase_3
{
    public class Fase3VitoriaScript : MonoBehaviour
    {
        [SerializeField] private GameObject pista;
        [SerializeField] private GameObject load;

        private void Awake()
        {
            pista.SetActive(false);
        }

        void Start()
        {
            OnSaveVitoria();
        }
        
        void OnSaveVitoria()
        {
            // Salva o progresso da fase 3
            Fase5Comeco.Repescagens[2] = true;
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.niveisRepescagem[2] = true;
            PlayerPrefs.SetInt("repescagem2", 1);
            PlayerPrefs.SetInt("currentLevel", 50);
            
            // Configura o prefab de loading para o RepescagemManager
            // RepescagemManager.SetLoadingPagePrefab(load);
            // Verifica se todas as repescagens foram completadas
            // RepescagemManager.CheckAllRepescagensComplete();
        }

        public void OpenPista()
        {
            pista.SetActive(true);
        }

        public void ClosePistaAndLoadNextScene()
        {
            // Usa o RepescagemManager para carregar a próxima cena
            RepescagemManager.SetLoadingPagePrefab(load);
            RepescagemManager.CheckAllRepescagensComplete();
            
            // Ou se preferir manter sua própria lógica:
            // StartCoroutine(loadRepescagemMain());
        }

        // Você pode manter este método como backup ou removê-lo se decidir usar apenas o RepescagemManager
        // private IEnumerator loadRepescagemMain()
        // {
        //     var loadI = Instantiate(load);
        //     loadI.SetActive(true);
        //     var progress = SceneManager.LoadSceneAsync("faserepescagem");
        //
        //     while (progress is { isDone: false })
        //     {
        //         loadI.GetComponent<Slider>().value = progress.progress;
        //         yield return null;
        //     }
        //     yield return new WaitForSecondsRealtime(1.0f);
        // }
    }
}
