using UnityEngine;

namespace Fase_5.Respescagem_Scritps.Fase_1
{
    public class Fase1Repescagemonrganizacao : MonoBehaviour
    {   
        public static Fase1Repescagemonrganizacao Instance;
        public GameObject tutorial;
        public GameObject game;
    
        void Start()
        {
            #region  carregar objetos
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        
            if (tutorial == null)
            {
                tutorial = GameObject.Find("12_fase1_2instruçao");
                if (tutorial == null)
                {
                    Debug.LogError("Tutorial não encontrado na cena.");
                }
            }
        
            if (game == null)
            {
                game = GameObject.Find("13_fase1_2minigame");
                if (game == null)
                {
                    Debug.LogError("Game não encontrado na cena.");
                }
            }
            #endregion
            if (tutorial != null && game != null)
            {
                tutorial.SetActive(true);
                game.SetActive(false);
            }
            else
            {
                Debug.LogError("Um ou mais objetos não foram encontrados na cena.");
            }

        }

        void FixedUpdate()
        {
            if (PassarinstrucaoRepescagem.isTutorialActive == false){
                Destroy(tutorial);
                Debug.Log("Tutorial concluído. Iniciando o jogo.");
                PassarinstrucaoRepescagem.isTutorialActive = true;
            
            }
        }
    }
}
