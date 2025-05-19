using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fase_4
{
    public class EnigmaUI : MonoBehaviour
    {
        public GameObject UiGameObject;
        public TextMeshProUGUI txtPergunta;
        public Button[] botoesAlt;
        public GameObject painelExplic;
        public TextMeshProUGUI txtExplicacao;
        public Color corCerta = Color.green;
        public Color corErrada = Color.red;
        private Action<bool> callback;
        private Button origemButton;
        public  Button fecharButton;
        private int indiceCorreto;
        private bool respondido = false;
        private bool acertou;


        public void Inicializar(Enigma e,
            Action<bool> onFinish,
            Button origemBtn)
        {
            callback = onFinish;
            origemButton = origemBtn;
            indiceCorreto = e.idCorreto;
            txtPergunta.text = e.enigma;
            txtExplicacao.text = e.explicacao;
            painelExplic.SetActive(false);

            for (int i = 0; i < botoesAlt.Length; i++)
            {
                int idx = i;
                var txt = botoesAlt[i].GetComponentInChildren<TextMeshProUGUI>();
                txt.text = e.alternativas[i];
                botoesAlt[i].onClick.RemoveAllListeners();
                botoesAlt[i].onClick.AddListener(() => OnEscolheu(idx));
            }
        }

        private void OnEscolheu(int idxEscolhido)
        {
            if (respondido) return;
            respondido = true;
            acertou = (idxEscolhido == indiceCorreto);

            for (int i = 0; i < botoesAlt.Length; i++)
            {
           
                botoesAlt[i].image.color = corErrada;
                Image cols = botoesAlt[i].GetComponent<Image>();
                if (i == indiceCorreto) cols.color = corCerta;
                else if (i == idxEscolhido) cols.color = corErrada;
                Debug.Log($"botao {i} - {cols.color}");
                botoesAlt[i].image = cols;
                botoesAlt[i].interactable = false;
            }

            Invoke(nameof(MostrarExplicacao), 1f);
        }

        private void MostrarExplicacao()
        {
            if (UiGameObject == null)
            {
                UiGameObject = GameObject.FindWithTag("EnigmaPainel");
                
                if (UiGameObject == null)
                {
                    Debug.LogError("UiGameObject n�o encontrado!");
                    return;
                }
                
            }
            painelExplic.SetActive(true);
            EnigmaScript.instance.PausarTimer();
            if (fecharButton == null)
            {
                fecharButton = painelExplic.GetComponentInChildren<Button>();
            }
            fecharButton.onClick.RemoveAllListeners();
            fecharButton.onClick.AddListener(() =>
            {
                Image cols = origemButton.GetComponent<Image>();
                cols.color = acertou ? corCerta : corErrada;
                origemButton.image = cols;
                origemButton.interactable = false;
                EnigmaScript.instance.LigarTimer();
                callback(acertou);
                Destroy(UiGameObject);
            }); 
        }
    }
}
