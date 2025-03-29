using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InicioJogo : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject popUp;
    [SerializeField] private GameObject popUpText;
    [SerializeField] private TextMeshProUGUI popUpTextMesh;
    private int noClickCount = 0;
    
    
    void Start()
    {
        mainCanvas = GameObject.Find("4_inicio_jogo");
        popUp = GameObject.Find("PopUp");
        popUpText = GameObject.Find("mainText");
        popUpTextMesh = popUpText.GetComponent<TextMeshProUGUI>();
        popUp.SetActive(false);
    }


    public void OnNoClicked()
    {
        noClickCount++;
        
        if (noClickCount == 1)
        {
            ShowPopup( "Entendo, você não está interessado realmente? \n pense mais um pouco");
        }
        else if (noClickCount >=2)
        {
            ShowPopup( "Por favor, só aperte SIM, é muito importante para mim.");
        }
        else
        {
            noClickCount = 0;
            ClosePopup();
        }
    }
    
    private void ShowPopup( string text)
    {
        popUpTextMesh.text = text;
        popUp.SetActive(true);
        
    }
    
    private void ClosePopup()
    {
        popUp.SetActive(false);
    }



}

