using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fase_2
{
    public class fase2niveis : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credencial;
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private GameObject statusFase1;
        [SerializeField] private Image[] status;
        [SerializeField] private GameObject dicas;

        private void Start()
        {
            dicas.SetActive(false);
            StartCoroutine(SaveFase());
            StartCoroutine(MarcarFase());
        }

        private IEnumerator SaveFase()
        {

            if (PlayerPrefs.HasKey("playerName"))
            {
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName", "Jogador");
            }
            else
            {
                Debug.LogError("Nenhum nome de jogador encontrado. Usando 'Jogador' como padrão.");
                EmboscadaController.gameData.playerName = "Jogador";
            }

            if (PlayerPrefs.HasKey("classificacao"))
            {
                EmboscadaController.gameData.classificacao = (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            }
            else
            {
                Debug.LogError("Nenhuma classificação encontrada. Usando 'NENHUMA' como padrão.");
                EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Amador;
            }
                
            if (credencial == null || playerName == null) {
                Debug.LogError("Refs de UI não atribuídas!");
                yield break;
            }
            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            playerName.text = EmboscadaController.gameData.playerName;
            PlayerPrefs.SetInt("currentLevel", 16);
            PlayerPrefs.Save();
        }

        IEnumerator MarcarFase()
        {   
            if(status.Length < 2)
            {
                Debug.LogError("Status array não preenchido!");
                yield break;
            }
            if (EmboscadaController.gameData.niveisganhos[0])
            {
                statusFase1.GetComponent<Image>().sprite = status[0].sprite;
            
            }
            else
            {
                statusFase1.GetComponent<Image>().sprite = status[1].sprite;
            }

            yield return null;
        }

        public void OnDicasClose()
        {
            dicas.SetActive(false);
        }

        public void OnDicasOpen()
        {
            dicas.SetActive(true);
        }
    }
}
