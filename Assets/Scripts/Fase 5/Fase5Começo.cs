using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_5
{
    public class Fase5Comeco : MonoBehaviour
    {
        private static bool _firstOpen = false;
        public static List<bool> Repescagens = new List<bool>{ false, false, false, false, false};
        public static List<niveisButton> nivelButtons = new List<niveisButton>(3);
        [SerializeField] private List<Image> imagensbutton = new List<Image>(3);
        [SerializeField] private GameObject Loadpage;
        
        [Header("fases Opções")]
        [SerializeField] private TextMeshProUGUI credencial;
        [SerializeField] private TextMeshProUGUI nome;
        private void Awake()
        {
            
             _firstOpen = PlayerPrefs.GetInt("repescagem", 0) == 1;
             LoadName();
             LoadRepescagem();
            InicializarBotoes();
            if (!_firstOpen)
                LoadSave();
            HabilitarFases();
            if (!RepescagemManager.LoadingPagePrefab)
            {
                RepescagemManager.SetLoadingPagePrefab(Loadpage);
            }
        }
        private void Start()
        {

        }

        public void CheckAllRepescagensComplete()
        {
            bool allComplete = true;
    
            // Verificamos das fases 1 a 4 (excluindo a 0 e a própria 5)
            for (int i = 1; i < 5; i++)
            {
                // Se a fase precisa de repescagem e ainda não foi concluída
                if (Repescagens[i] == false)
                {
                    allComplete = false;
                    break;
                }
            }
    
            if (allComplete)
            {
                // Todas as repescagens foram completadas, carregar a fase 5
                StartCoroutine(LoadFase5());
            }
            else
            {
                // Ainda há repescagens pendentes, voltar para a tela de repescagem
                StartCoroutine(ReturnToRepescagemScreen());
            }
        }

        private IEnumerator LoadFase5()
        {
            var loadI = Instantiate(Loadpage);
            loadI.SetActive(true);
            var progress = SceneManager.LoadSceneAsync("fase5");
    
            while (progress is { isDone: false })
            {
                loadI.GetComponent<Slider>().value = progress.progress;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(1.0f);
        }

        private IEnumerator ReturnToRepescagemScreen()
        {
            var loadI = Instantiate(Loadpage);
            loadI.SetActive(true);
            var progress = SceneManager.LoadSceneAsync("faserepescagem");
    
            while (progress is { isDone: false })
            {
                loadI.GetComponent<Slider>().value = progress.progress;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(1.0f);
        }
        private void LoadName()
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            EmboscadaController.gameData.classificacao = 
                (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            EmboscadaController.gameData.currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
            EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName");
            nome.text = EmboscadaController.gameData.playerName;
            EmboscadaController.gameData.selectedCharacterId = PlayerPrefs.GetInt("selectedCharacterId");
        }

        private static void LoadSave()
        {
            
            for(var i = 0; i < EmboscadaController.gameData.niveisganhos.Length; i++)
            {
                var ganhou = PlayerPrefs.GetInt($"nivel{i}", 0) == 1;
                EmboscadaController.gameData.niveisganhos[i] = ganhou;
                Repescagens.Add(!ganhou);
                EmboscadaController.gameData.niveisRepescagem[i] = !ganhou;
                PlayerPrefs.SetInt($"repescagem{i}", ganhou ? 1 : 0);
            }
            _firstOpen = true;
            PlayerPrefs.SetInt("repescagem", 1);
            PlayerPrefs.SetInt("currentLevel", 50);
        }

        private void InicializarBotoes()
        {
            foreach (var button in nivelButtons)
            {
                button.button.interactable = false;
                button.button.gameObject.SetActive(false);
            }
        }
        

        private void HabilitarFases()
        {
            for (var i = 1; i < nivelButtons.Count; i++)
            {
                nivelButtons[i].button.gameObject.SetActive(Repescagens[i+1]);
                nivelButtons[i].button.interactable = Repescagens[i+1];
                nivelButtons[i].status = Repescagens[i+1];
                if ( nivelButtons[i].status)
                    imagensbutton[i].gameObject.SetActive(false);
            }
        }

    
        public void OnClickNivel(int nivel)
        {
            StartCoroutine(StartFaseRepescagem(nivel));
        }

        private IEnumerator StartFaseRepescagem(int nivel)
        {
            var loadI = Instantiate(Loadpage);
            loadI.SetActive(true);
            AsyncOperation progress = new AsyncOperation();
            switch (nivel)
            {
                case 1:
                   progress =  SceneManager.LoadSceneAsync("2 repescagem fase 2");
                    break;
                case 2:
                    progress = SceneManager.LoadSceneAsync("3 repescagem fase 3");
                    break;
                case 3:
                    progress =   SceneManager.LoadSceneAsync("4 repescagem fase 4");
                    break;
            }
            while (progress is { isDone: false })
            {
                loadI.GetComponent<Slider>().value = progress.progress;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(1.0f);
        }
        

        private void LoadRepescagem()
        {
            for (var i = 0; i < 5; i++)
            {
                Repescagens[i] = PlayerPrefs.GetInt("repescagem"+i, 0) == 1;
            }
        }


    
    }
}