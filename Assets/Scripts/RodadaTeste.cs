using System.Collections;
using TMPro;
using UnityEngine;

public class RodadaTeste : MonoBehaviour
{
    [SerializeField] private int id = 0;
     private string texto;
    [SerializeField] private GameObject TextMash;
    private TextMeshProUGUI textMeshFild;
    void Start()
    {   
        if (id == 0){
            texto = "Rodada teste";
        }else if (id == 1){
            texto = "Rodada Valendo";
        }
        if (TextMash != null)
        {
            textMeshFild = TextMash.GetComponent<TextMeshProUGUI>();
            textMeshFild.text = texto;
        }

        StartCoroutine(TimerCoroutine());
    }

     IEnumerator TimerCoroutine(){
        yield return new WaitForSeconds(3f);
        next();

     }

     void next(){
            if (MainManager.main != null)
            {
                {
                    MainManager.main.IrParaCanvas(13);
                }

            }
            else
            {
                Debug.LogWarning("mainManager.main is not initialized.");
            }
     }
}
