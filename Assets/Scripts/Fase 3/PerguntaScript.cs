using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fase_3
{
    public class PerguntaScript : MonoBehaviour
    {
        [SerializeField] private Button[] opcoes;
        [SerializeField] private int indiceCorreto;
        public event Action<bool> OnAnswered;

        void Start()
        {
            for (int i = 0; i < opcoes.Length; i++)
            {
                int idx = i;
                opcoes[i].onClick.AddListener(() =>
                {
                    bool acertou = idx == indiceCorreto;
                    OnAnswered?.Invoke(acertou);
                    Destroy(gameObject);
                });
            }
        }

        public void ForceFinish(bool correto)
        {
            if (OnAnswered != null)
            {
                OnAnswered.Invoke(correto);
            }
        
            Button[] botoes = GetComponentsInChildren<Button>();
            foreach (var botao in botoes)
            {
                botao.interactable = false;
            }
            Transform tempoEsgotadoText = transform.Find("TempoEsgotadoText");
            if (tempoEsgotadoText != null)
            {
                tempoEsgotadoText.gameObject.SetActive(true);
            }
        }

    }
}