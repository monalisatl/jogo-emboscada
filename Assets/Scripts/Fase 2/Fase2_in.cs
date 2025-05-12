using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fase2_in : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void onclickFase2In()
    {
        SceneManager.LoadSceneAsync("18_fase2_minigame");
    }
}
