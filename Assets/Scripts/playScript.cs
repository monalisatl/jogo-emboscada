using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class playScript : MonoBehaviour{
    public GameObject main;
    public GameObject loading;


    private void Awake()
    {
    }
    public void playGame(){
    Debug.Log("Tentando passar para a main");
    MainManager.indiceCanvainicial = 0;
    SceneManager.LoadSceneAsync("main");
        // Debug.Log("enviado");
    }

public void quitGame(){
    Console.WriteLine("Quit Game");
    Application.Quit();
    }
}
