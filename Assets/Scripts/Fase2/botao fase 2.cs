using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class botaofase1 : MonoBehaviour
{
    public Color corNormal;
    public GameObject button;
    private Image image;
    private bool isPressed = false;

    void Start()
    {
        image = GetComponent<Image>();
        corNormal = image.color;
        Debug.Log("Cor Normal: " + corNormal);
    }

    public void OnPres(){


        if (isPressed)
        {
            image.color = new Color(0.525f, 0f, 0.624f, 1f);
            isPressed = false;
        }
        else
        {
            image.color = new Color(0f, 1f, 0.13f,1f);
            isPressed = true;
        }

    }

}
