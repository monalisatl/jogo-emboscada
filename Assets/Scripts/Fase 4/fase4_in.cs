using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fase_4
{
    public class NiveisFase4 : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI credencial;
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private GameObject statusFase1;
        [SerializeField] private GameObject statusFase2;
        [SerializeField] private GameObject statusFase3;
        [SerializeField] private Image[] status;  // 0 = sucesso, 1 = falha
        [SerializeField] private GameObject dica;
        [SerializeField] private GameObject[] bloqueios;
        private void Start()
        {   ConfigurarBloqueios();
            StartCoroutine(SaveFase());
            StartCoroutine(MarcarFases());
        }



        private IEnumerator SaveFase()
        {
            EmboscadaController.gameData ??= new EmboscadaController.GameData();
            // --- Carrega dados do jogador ---
            if (PlayerPrefs.HasKey("playerName"))
                EmboscadaController.gameData.playerName = PlayerPrefs.GetString("playerName");
            else
            {
                Debug.LogError("Nenhum nome salvo: usando 'Jogador'");
                EmboscadaController.gameData.playerName = "Jogador";
            }

            if (PlayerPrefs.HasKey("classificacao"))
                EmboscadaController.gameData.classificacao =
                    (EmboscadaController.Classificacao)PlayerPrefs.GetInt("classificacao", 0);
            else
            {
                Debug.LogError("Nenhuma classificação salva: usando Amador");
                EmboscadaController.gameData.classificacao = EmboscadaController.Classificacao.Amador;
            }
            
            VerificarDicas();
            if (!credencial || !playerName)
            {
                Debug.LogError("Campos de UI não atribuídos!");
                yield break;
            }

            credencial.text = EmboscadaController.gameData.classificacao.ToString();
            playerName.text = EmboscadaController.gameData.playerName;
            PlayerPrefs.SetInt("currentLevel", 31);
            PlayerPrefs.Save();

            yield return null;
        }

        private IEnumerator MarcarFases()
        {
            // valida array de sprites
            if (status == null || status.Length < 2)
            {
                Debug.LogError("Array status não configurado (esperava 2 sprites)");
                yield break;
            }
            // valida GameObjects
            if (!statusFase1 || !statusFase2 || !statusFase3)
            {
                Debug.LogError("statusFaseX não atribuído no Inspector");
                yield break;
            }

            // fase 1
            var img1 = statusFase1.GetComponent<Image>();
            if (img1 != null)
                img1.sprite = EmboscadaController.gameData.niveisganhos[0]
                    ? status[0].sprite
                    : status[1].sprite;

            // fase 2
            var img2 = statusFase2.GetComponent<Image>();
            if (img2 != null)
                img2.sprite = EmboscadaController.gameData.niveisganhos[1]
                    ? status[0].sprite
                    : status[1].sprite;

            // fase 3
            var img3 = statusFase3.GetComponent<Image>();
            if (img3 != null)
                img3.sprite = EmboscadaController.gameData.niveisganhos[2]
                    ? status[0].sprite
                    : status[1].sprite;

            yield return null;
        }

        public void OnCameraPress()
        {
            Invoke("OpenFase4", 1.0f);
        }

        private void OpenFase4()
        {
            SceneManager.LoadSceneAsync("fase4.1");
        }
        
        private void ConfigurarBloqueios()
        {
            dica.SetActive(false);
            foreach (var bloqueio in bloqueios)
            {
                bloqueio.SetActive(true);
            }

        }

        private void VerificarDicas()
        {
            for (int i = 1; i < 3; i++)
            {
                if (EmboscadaController.gameData.niveisganhos[i] = (PlayerPrefs.GetInt("nivel" + i, 0) == 1))
                {
                    bloqueios[i-1].SetActive(false);
                }
            }
        }
        
        public void OnDicasOpen()
        {
            dica.SetActive(true);
        }

        public void OnDicasClose()
        {
            dica.SetActive(false);
        }
        
    }
}
