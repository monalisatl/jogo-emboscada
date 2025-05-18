using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fase_5.Respescagem_Scritps.Fase_1
{
    public class EntrarRepescagemfase1 : MonoBehaviour
    {
        public static EntrarRepescagemfase1 Instance;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        
        }
    

        public void abrir_fase1()
        {
            SceneManager.LoadSceneAsync("11_fase1_minigame");
        }

    }
}
