using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class playScript : MonoBehaviour{
    public GameObject main;
    public GameObject loading;


    private void Awake()
    {

       DontDestroyOnLoad(loading);
       DontDestroyOnLoad(main);
    }
    public void playGame(){
    Debug.Log("Tentando passar para a main");
    mainManager.indiceCanvainicial = 0;
    SceneManager.LoadScene("main");
        // Debug.Log("enviado");
    }

public void quitGame(){
    Console.WriteLine("Quit Game");
    Application.Quit();
    }
}
