using System;
using UnityEngine;
using UnityEngine.UI;

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
}