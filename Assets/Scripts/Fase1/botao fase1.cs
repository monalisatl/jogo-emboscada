using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class botaofase1 : MonoBehaviour
{

    public GameObject button;
    private Image image;
    private bool isPressed = false;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void OnPres(){
        if (isPressed)
        {
            image.color = new Color(134f, 0f, 159f, 255f);
            isPressed = false;
        }
        else
        {
            image.color = new Color(134f, 0f, 159f, 255f);
            isPressed = true;
        }

    }

}
