using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_3
{
    public class niveisFase3 : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credencial;
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private GameObject statusFase1;
        [SerializeField] private GameObject statusFase2;
        [SerializeField] private Image[] status;  // 0 = sprite de sucesso, 1 = sprite de falha

        private void Start()
        {
            StartCoroutine(SaveFase());
            StartCoroutine(MarcarFases());
        }

        private IEnumerator SaveFase()
        {
            // Carrega nome do jogador
            if (PlayerPrefs.HasKey("playerName"))
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
            else
            {
                Debug.LogError("Nenhum nome de jogador encontrado. Usando 'Jogador' como padrão.");
                EmboscadaController.gameData.playerName = "Jogador";
            }

            // Carrega classificação
            if (PlayerPrefs.HasKey("classificacao"))
                EmboscadaController.gameData.classificacao =
                    (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            else
            {
                Debug.LogError("Nenhuma classificação encontrada. Usando 'Amador' como padrão.");
                EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Amador;
            }

            // Verifica referências de UI
            if (!credencial || !playerName)
            {
                Debug.LogError("Refs de UI (credencial/playerName) não atribuídas!");
                yield break;
            }

            // Atualiza texto
            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            playerName.text = EmboscadaController.gameData.playerName;

            // Atualiza level atual
            PlayerPrefs.SetInt("currentLevel", 23);
            PlayerPrefs.Save();

            yield return null;
        }

        private IEnumerator MarcarFases()
        {
            // Precisa de ao menos 2 sprites (sucesso/falha)
            if (status == null || status.Length < 2)
            {
                Debug.LogError("Array de status não configurado (esperava 2 sprites)!");
                yield break;
            }

            // Verifica referências aos GameObjects de status
            if (statusFase1 == null || statusFase2 == null)
            {
                Debug.LogError("GameObjects statusFase1 ou statusFase2 não atribuídos!");
                yield break;
            }

            // Marca fase 1
            var img1 = statusFase1.GetComponent<Image>();
            if (img1 != null)
                img1.sprite = EmboscadaController.gameData.niveisganhos[0]
                    ? status[0].sprite
                    : status[1].sprite;

            // Marca fase 2
            var img2 = statusFase2.GetComponent<Image>();
            if (img2 != null)
                img2.sprite = EmboscadaController.gameData.niveisganhos[1]
                    ? status[0].sprite
                    : status[1].sprite;

            yield return null;
        }

        public void AbrirFase3()
        {
            SceneManager.LoadSceneAsync("fase3");
        }
    }
}

