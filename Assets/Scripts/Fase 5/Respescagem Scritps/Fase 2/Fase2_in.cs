using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fase_5.Respescagem_Scritps.Fase_2
{
    public class Fase2InRepescagem : MonoBehaviour
    {
        public void onclickFase2In()
        {
            SceneManager.LoadSceneAsync("18_fase2_minigame");
        }
    }
}
