using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InicioJogo : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject firstPopupPrefab;
    [SerializeField] private GameObject secondPopupPrefab;

    private GameObject currentPopup;
    private int noClickCount = 0;
    
    
    void Start()
    {
        mainCanvas = GameObject.Find("");
    }

    public void OnYesClicked()
    {
        Debug.Log("Jogador escolheu SIM! Continuando o jogo...");
        mainManager.main.ProximoCanvas();
    }
    
    public void OnNoClicked()
    {
        noClickCount++;
        
        if (noClickCount == 1)
        {
            ShowPopup(firstPopupPrefab);
        }
        else if (noClickCount == 2)
        {
            ShowPopup(secondPopupPrefab);
        }
        else
        {
            noClickCount = 0;
            ClosePopup();
        }
    }
    
    private void ShowPopup(GameObject popupPrefab)
    {
        // ClosePopup();
        // currentPopup = Instantiate(popupPrefab, canvasParent);
        // Button buttons = currentPopup.GetComponentInChildren<Button>();
        // foreach (Button button in buttons)
        // {
        //     if (button.name.Contains("Sim"))
        //         button.onClick.AddListener(OnYesClicked);
        //     else if (button.name.Contains("Nao") || button.name.Contains("Não"))
        //         button.onClick.AddListener(OnNoClicked);
        // }
    }
    
    private void ClosePopup()
    {
        if (currentPopup != null)
            Destroy(currentPopup);
    }


}

