using UnityEngine;
using UnityEngine.SceneManagement;

public class entrar_fase1 : MonoBehaviour
{
    public static entrar_fase1 instance;

    void Start()
    {
       if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void abrir_fase1()
    {
        SceneManager.LoadScene("11_fase1_minigame");
    }

}
