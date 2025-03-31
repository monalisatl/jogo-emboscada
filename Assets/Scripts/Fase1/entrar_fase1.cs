using UnityEngine;

public class entrar_fase1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void abrir_fase1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("11_fase1_minigame");
    }
}
