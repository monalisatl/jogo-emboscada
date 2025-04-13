using UnityEngine;

public class fase_1_onrganizacao : MonoBehaviour
{   
    public static fase_1_onrganizacao instance;
    public GameObject tutorial;
    public GameObject game;
    
    void Start()
    {
    #region  carregar objetos
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
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
        if (passarinstrucao.isTutorialActive == false){
            Destroy(tutorial);
            Debug.Log("Tutorial concluído. Iniciando o jogo.");
            passarinstrucao.isTutorialActive = true;
            
        }
    }
}
