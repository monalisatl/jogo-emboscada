using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class playScript : MonoBehaviour{

public void playGame(){
    SceneManager.LoadScene("Game");

    }

public void quitGame(){
    Console.WriteLine("Quit Game");
    Application.Quit();
    }
}
