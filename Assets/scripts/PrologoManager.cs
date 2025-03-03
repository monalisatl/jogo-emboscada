using TMPro;
using UnityEngine;

public class PrologoManager : MonoBehaviour
{
    public GameObject prologo;
    private TextMeshProUGUI textMesh;
    public string texto;

    void Start()
    {
        // Obtém o componente TextMeshProUGUI corretamente
        textMesh = prologo.GetComponent<TextMeshProUGUI>();

        // Define o texto inicial
        AtualizarTexto(texto);
    }

    // Método público para atualizar o texto dinamicamente
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
