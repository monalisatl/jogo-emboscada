using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fase_2
{
    public class Fase2_in : MonoBehaviour
    {
        public void OnclickFase2In()
        {
            SceneManager.LoadSceneAsync("18_fase2_minigame");
        }
    }
}
