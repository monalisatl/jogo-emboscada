using System.Data.Common;
using UnityEngine;
using UnityEngine.UIElements;

public class botaofase1 : MonoBehaviour
{

    public GameObject button;
    private Image image;
    private bool isPressed = false;

    void Start()
    {
        image = button.GetComponent<Image>();
    }

    void OnPres(){
        if (isPressed)
        {
            image.tintColor = new Color(134f, 0f, 159f, 255f);
            isPressed = false;
        }
        else
        {
            image.tintColor = new Color(134f, 0f, 159f, 255f);
            isPressed = true;
        }

    }

}
