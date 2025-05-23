using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Fase_2;
using Fase_3;
using Fase_4;
namespace Fase_5
{
    public class Fase5Comeco : MonoBehaviour
    {
        public static bool[] Repescagens = {false, false, false};
        [SerializeField] private GameObject pistas;
        [SerializeField] private GameObject[] statusFase = new GameObject[3];
        [SerializeField] private Button[] nivelButtons = new Button[3];
        [SerializeField] private Image[] imagensbutton = new Image[3];
        [FormerlySerializedAs("Loadpage")] [SerializeField] private GameObject loadpage;
        
        [Header("Informações do Jogador")] [SerializeField]
        private TextMeshProUGUI credencial;

        [SerializeField] private TextMeshProUGUI nome;

        private void Awake()
        {
            Debug.Log($"Fase 2 (nivel1): {PlayerPrefs.GetInt("nivel1", 0)}");
            Debug.Log($"Fase 3 (nivel2): {PlayerPrefs.GetInt("nivel2", 0)}");
            Debug.Log($"Fase 4 (nivel3): {PlayerPrefs.GetInt("nivel3", 0)}");
            // Carrega dados do jogador
            LoadName();
            VerificarPistas();
            // Carrega estado das repescagens
            LoadRepescagemStatus();
            ClosePistas();
            // Configura os botões baseado nas repescagens necessárias
            AtualizarBotoesRepescagem();

            // Configura o prefab de loading, se necessário
            if (!RepescagemManager.LoadingPagePrefab && loadpage)
            {
                RepescagemManager.SetLoadingPagePrefab(loadpage);
            }

            CheckAllRepescagensComplete();
        }
        public void CheckAllRepescagensComplete()
        {
            bool todasCompletadas = true;
            
            for (int i = 0; i < 3; i++)
            {
                if (Repescagens[i])
                {
                    todasCompletadas = false;
                    break;
                }
            }

            if (todasCompletadas)
            {
                StartCoroutine(LoadFase5());
            }
        }

        private IEnumerator LoadFase5()
        {
            // Verifica se o prefab de loading existe
            if (loadpage == null)
            {
                Debug.LogError("Prefab de Loading não configurado!");
                SceneManager.LoadScene("fase5");
                yield break;
            }

            // Cria a tela de loading
            GameObject loadI = Instantiate(loadpage);
            loadI.SetActive(true);

            // Busca o slider para atualizar o progresso
            Slider progressBar = loadI.GetComponentInChildren<Slider>();

            // Inicia o carregamento da fase 5
            AsyncOperation progress = SceneManager.LoadSceneAsync("fase5");

            // Atualiza a barra de progresso enquanto carrega
            while (progress is { isDone: false })
            {
                if (progressBar != null)
                {
                    progressBar.value = progress.progress;
                }

                yield return null;
            }

            // Pequeno delay para garantir que a animação de loading seja vista
            yield return new WaitForSecondsRealtime(1.0f);

            // Limpa o objeto de loading
            if (loadI != null)
            {
                Destroy(loadI);
            }
        }

        private IEnumerator ReturnToRepescagemScreen()
        {
            
            if (loadpage == null)
            {
                Debug.LogError("Prefab de Loading não configurado!");
                SceneManager.LoadScene("faserepescagem");
                yield break;
            }

            // Cria a tela de loading
            GameObject loadI = Instantiate(loadpage);
            loadI.SetActive(true);

            // Busca o slider para atualizar o progresso
            Slider progressBar = loadI.GetComponentInChildren<Slider>();

            // Inicia o carregamento da tela de repescagem
            AsyncOperation progress = SceneManager.LoadSceneAsync("faserepescagem");

            // Atualiza a barra de progresso enquanto carrega
            while (!progress.isDone)
            {
                if (progressBar != null)
                {
                    progressBar.value = progress.progress;
                }

                yield return null;
            }

            // Pequeno delay para garantir que a animação de loading seja vista
            yield return new WaitForSecondsRealtime(1.0f);

            // Limpa o objeto de loading
            if (loadI != null)
            {
                Destroy(loadI);
            }
        }

        private void LoadName()
        {
            // Inicializa os dados do jogador se necessário
            EmboscadaController.gameData ??= new EmboscadaController.GameData();

            // Carrega a classificação
            EmboscadaController.gameData.classificacao =
                (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);

            // Atualiza o texto da credencial
            if (credencial != null)
            {
                credencial.text = EmboscadaController.gameData.classificacao.ToString();
            }

            // Carrega o nível atual
            EmboscadaController.gameData.currentLevel = PlayerPrefs.GetInt("currentLevel", 0);

            // Carrega o nome do jogador
            EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Detetive");

            // Atualiza o texto do nome
            if (nome != null)
            {
                nome.text = EmboscadaController.gameData.playerName;
            }

            // Carrega o ID do personagem selecionado
            EmboscadaController.gameData.selectedCharacterId = PlayerPrefs.GetInt("selectedCharacterId", 0);
        }

        // Carrega o status de repescagem para os níveis 2, 3 e 4
        private void LoadRepescagemStatus()
        {   Debug.Log($"Tamanho de repescagem: {Repescagens.Length}");
            for (int i = 0; i < 3; i++)
            {
                int numeroNivel = i + 1;
                
                bool ganhouFase = PlayerPrefs.GetInt($"nivel{numeroNivel}", 0) == 1;
                Repescagens[i] = !ganhouFase;
                if (PlayerPrefs.GetInt("repescagem", 0) == 0)
                {
                    PlayerPrefs.SetInt($"repescagem{numeroNivel}", !ganhouFase ? 1 : 0);
                    if (EmboscadaController.gameData.niveisRepescagem.Length > numeroNivel)
                    {
                        EmboscadaController.gameData.niveisRepescagem[numeroNivel] = !ganhouFase;
                    }
                }
                else
                {
                    // Se não for o primeiro carregamento, carrega o status salvo
                    Repescagens[i] = PlayerPrefs.GetInt($"repescagem{numeroNivel}", 0) == 1;
                }

                Debug.Log($"Nível {numeroNivel}: Precisa repescagem = {Repescagens[i]}");
            }

            // Marca como inicializado
            PlayerPrefs.SetInt("repescagem", 1);
        }

        // Atualiza os botões baseado no status de repescagem
        private void AtualizarBotoesRepescagem()
        {
            for (int i = 0; i < 3; i++)
            {
                // Verifica se o botão está configurado
                if (nivelButtons[i] != null )
                {
                    // Ativa o botão apenas se precisar de repescagem
                    //nivelButtons[i].gameObject.SetActive(Repescagens[i]);
                    nivelButtons[i].interactable = Repescagens[i];
                    // Atualiza a imagem status relacionada (se existir)
                    if (i < imagensbutton.Length && imagensbutton[i] != null)
                    {
                        imagensbutton[i].gameObject.SetActive(!Repescagens[i]);
                    }
                }
                else
                {
                    Debug.LogWarning($"Botão para nível {i + 1} não configurado!");
                }
            }
        }

        // Método para ser chamado pelo botão (1 = nível 2, 2 = nível 3, 3 = nível 4)
        public void OnClickNivel(int nivelButton)
        {
            Debug.Log($"Clicou no botão: {nivelButton} para nível {nivelButton + 1}");

            // Verifica se o botão clicado é válido
            if (nivelButton >= 1 && nivelButton <= 3)
            {
                int nivel = nivelButton + 1;
                RepescagemManager.StartRepescagem(nivel);
                StartCoroutine(StartFaseRepescagem(nivelButton));
            }
            else
            {
                Debug.LogError($"Botão inválido: {nivelButton}");
            }
        }

        private IEnumerator StartFaseRepescagem(int nivelButton)
        {
            var name = "";
            switch (nivelButton)
            {
                case 1:
                    name = "18_fase2_minigame";
                    ChronoUIController._isRepescagem = true;
                    Fase2Manager.ResetRepescagem();
                    break;
                case 2:
                    name = "fase3";
                    Fase3Manager._isRepescagemMode = true;
                    break;
                case 3:
                    name = "fase4.1";
                    EnigmaScript.isRepescagemMode = true;
                    break;
            }
            if (loadpage == null)
            {
                Debug.LogError("Prefab de Loading não configurado!");
                SceneManager.LoadScene($"{name}");
                yield break;
            }

            // Cria a tela de loading
            GameObject loadI = Instantiate(loadpage);
            loadI.SetActive(true);

            // Busca o slider para atualizar o progresso
            Slider progressBar = loadI.GetComponentInChildren<Slider>();

            // Define o nome da cena - formato principal
            string sceneName = $"{name}";
            Debug.Log($"Tentando carregar cena: {sceneName}");

            // Verifica se a cena existe no build
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);

            // Se não existe, tenta o formato alternativo
            if (sceneIndex < 0)
            {
                sceneName = $"{nivelButton} repescagem fase {nivelButton + 1}";
                Debug.Log($"Tentando formato alternativo: {sceneName}");
                sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
                
                if (sceneIndex < 0)
                {
                    sceneName = $"{name}";
                    Debug.Log($"Tentando último formato: {sceneName}");
                    sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);

                    // Se nenhum formato funcionar
                    if (sceneIndex < 0)
                    {
                        Debug.LogError(
                            $"Nenhum formato de nome de cena válido encontrado para nível {nivelButton + 1}");
                        Destroy(loadI);
                        yield break;
                    }
                }
            }

            // Carrega a cena pelo índice encontrado
            AsyncOperation progress = SceneManager.LoadSceneAsync(sceneIndex);

            // Atualiza a barra de progresso enquanto carrega
            while (!progress.isDone)
            {
                if (progressBar != null)
                {
                    progressBar.value = progress.progress;
                }

                yield return null;
            }

            // Pequeno delay para garantir que a animação de loading seja vista
            yield return new WaitForSecondsRealtime(1.0f);

            // Limpa o objeto de loading
            if (loadI != null)
            {
                Destroy(loadI);
            }
        }

        public void OpenPistas()
        {
            pistas.gameObject.SetActive(true);
        }
        
        public void ClosePistas()
        {
            pistas.gameObject.SetActive(false);
        }

        private void VerificarPistas()
        {
            for (int i = 1; i < 4; i++)
            {
                if (PlayerPrefs.GetInt($"nivel{i}", 0) == 1)
                {
                    statusFase[i-1].gameObject.SetActive(false);
                }
            }
        }
    }
    
}
