using UnityEngine;
using UnityEngine.UI;
using System;

public class EnigmaUI : MonoBehaviour
{
    public Text txtPergunta;
    public Button[] botoesAlt;
    public GameObject painelExplic;
    public Text txtExplicacao;
    public Color corCerta = Color.green;
    public Color corErrada = Color.red;

    private Action<bool> callback;
    private int indiceCorreto;
    private bool respondido = false;

    public void Inicializar(Enigma e, Action<bool> onFinish)
    {
        txtPergunta.text = e.enigma;
        indiceCorreto = e.idCorreto;
        callback = onFinish;
        txtExplicacao.text = e.explicacao;
        painelExplic.SetActive(false);

        for (int i = 0; i < botoesAlt.Length; i++)
        {
            int idx = i;
            botoesAlt[i].transform.GetChild(0).GetComponent<Text>().text = e.alternativas[i];
            botoesAlt[i].onClick.RemoveAllListeners();
            botoesAlt[i].onClick.AddListener(() => OnEscolheu(idx));
        }
    }

    private void OnEscolheu(int idxEscolhido)
    {
        if (respondido) return;
        respondido = true;
        for (int i = 0; i < botoesAlt.Length; i++)
        {
            var colors = botoesAlt[i].colors;
            if (i == indiceCorreto) colors.normalColor = corCerta;
            else if (i == idxEscolhido) colors.normalColor = corErrada;
            botoesAlt[i].colors = colors;
            botoesAlt[i].interactable = false;
        }
        Invoke(nameof(MostrarExplicacao), 1f);
    }

    private void MostrarExplicacao()
    {
        painelExplic.SetActive(true);
        var btn = painelExplic.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            callback( /* acertou? */ respondido && (Array.IndexOf(botoesAlt, botoesAlt[indiceCorreto]) == indiceCorreto) );
            Destroy(gameObject);
        });
    }
}
