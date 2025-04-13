using UnityEngine;
using UnityEngine.UI;

public class passarinstrucao : MonoBehaviour
{

    public GameObject button;
    public static passarinstrucao instance;

    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject game;

    public static bool isTutorialActive = true;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if (button == null)
        {
            button = GameObject.Find("next");
            if (button == null)
            {
                Debug.LogError("Botão 'next' não encontrado na cena.");
            }
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

    }

    public void OnPressSeguir()
    {
        if (!isTutorialActive)
        {
            Debug.Log("O tutorial já foi concluído. Não é possível prosseguir.");
            return;
        }
        if (button != null)
        {
            game.SetActive(true);
            tutorial.SetActive(false);
            Debug.Log("Botão 'next' pressionado.");
            isTutorialActive = false;
        }
        else
        {
            Debug.LogError("Botão 'next' não encontrado. Não é possível prosseguir.");
        }
    }
}
