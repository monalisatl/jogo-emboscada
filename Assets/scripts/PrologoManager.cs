using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class PrologoManager : MonoBehaviour
{
    public GameObject prologo;
    private TextMeshProUGUI textMesh;
    public string texto;

    void Start()
    {
        textMesh = prologo.GetComponent<TextMeshProUGUI>();

        AtualizarTexto(texto);
    }

    public void AtualizarTexto(string novoTexto)
    {
        if (textMesh != null)
        {
            textMesh.text = novoTexto;
        }
        else
        {
            Debug.LogWarning("O componente TextMeshProUGUI não foi encontrado.");
        }
    }

    
}
