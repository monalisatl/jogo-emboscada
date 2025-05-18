using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RepescagemManager : MonoBehaviour
{
    [Header("Arraste aqui os 4 Toggles (nível 1..4)")]
    public List<Toggle> nivelToggles;

    private Queue<int> filaDeNiveis = new Queue<int>();
    [SerializeField] private List<Scene> scenasfase = new List<Scene>();
    private void Start()
    {
        for (int i = 0; i < nivelToggles.Count; i++)
            nivelToggles[i].isOn = Fase5Comeco.Repescagens[i];
    }


    //
    // private IEnumerator ProcessarRepescagem()
    // {
    //     while (filaDeNiveis.Count > 0)
    //     {
    //         var nivel = filaDeNiveis.Dequeue();
    //         var terminou = false;
    //         var venceu = false;
    //         
    //         void OnLevelEnd(bool win)
    //         {
    //             terminou = true;
    //             venceu = win;
    //         }
    //         EmboscadaController.OnLevelEnd += OnLevelEnd;
    //     
    //         // Carrega a cena (por exemplo "Level1", "Level2", etc)
    //         SceneManager.LoadScene($"Level{nivel}");
    //     
    //         // espera até o player terminar a fase
    //         yield return new WaitUntil(() => terminou);
    //     
    //         EmboscadaController.OnLevelEnd -= OnLevelEnd;
    //     
    //         if (!venceu)
    //         {
    //             // reenfileira o mesmo nível pra jogar de novo
    //             filaDeNiveis.Enqueue(nivel);
    //         }
    //         // se venceu, ele segue naturalmente para o próximo da fila
    //     }
    //     
    //     // quando a fila zerar, acabou a repescagem
    //     Debug.Log("Repescagem concluída!");
    //     // aqui você pode, por exemplo, voltar ao menu ou avançar
    //     SceneManager.LoadScene("MainMenu");
    // }
}
