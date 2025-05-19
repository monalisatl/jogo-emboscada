using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fase_5
{
    public class Fase5EntryManager : MonoBehaviour
    {
        [SerializeField] private GameObject botaoRepescagem;
        [SerializeField] private GameObject loadingPrefab;
        [SerializeField] private Button botaogeral;

        [Header("Configuração de Repescagem")]
        [SerializeField] private int minimoFasesAprovadas = 3;
        
        private void Start()
        {
            botaoRepescagem.SetActive(false);
            botaogeral.onClick.AddListener(VerificarEMostrarBotoes);
        }
        
        public void VerificarEMostrarBotoes()
        {
            bool precisaRepescagem = VerificarNecessidadeRepescagem();
            if (precisaRepescagem)
            {
                botaoRepescagem.SetActive(true);
                ConfigurarBotoes();
            }
            else
            {
                CarregarTelaDireta();
            }
        }
        
        private bool VerificarNecessidadeRepescagem()
        {
            int fasesAprovadas = 0;
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            for (int i = 0; i < 5; i++)
            {
                EmboscadaController.gameData.niveisganhos[i] = PlayerPrefs.GetInt($"nivel{i}", 0) == 1;
                if (EmboscadaController.gameData.niveisganhos[i])
                {
                    fasesAprovadas++;
                }
            }
            bool fezRepescagemCompleta = VerificarRepescagensCompletas();
            if (fezRepescagemCompleta)
                return false;
            return fasesAprovadas < minimoFasesAprovadas;
        }
        
        private bool VerificarRepescagensCompletas()
        {
            for (int i = 1; i < 4; i++)
            {
                if (PlayerPrefs.GetInt($"repescagem{i}", 0) != 1)
                {
                    return false;
                }
            }
            return true;
        }
        
        private void ConfigurarBotoes()
        {
            Button btnRepescagem = botaoRepescagem.GetComponent<Button>();
            if (btnRepescagem)
            {
                btnRepescagem.onClick.RemoveAllListeners();
                btnRepescagem.onClick.AddListener(CarregarTelaRepescagem);
            }
        }
        
        public void CarregarTelaDireta()
        {
            StartCoroutine(CarregarCena("fase5_video"));
        }
        
        public void CarregarTelaRepescagem()
        {
            StartCoroutine(CarregarCena("faserepescagem"));
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
