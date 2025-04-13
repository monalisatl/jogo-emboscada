using System;
using TMPro;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject maintext;
    private TextMeshProUGUI textMesh;
    private string texto;
    private GameObject popUp;

void Start()
{
    popUp = GameObject.Find("PopUp");
}


public void closePopUp()
{
    popUp.SetActive(false);
}

}
