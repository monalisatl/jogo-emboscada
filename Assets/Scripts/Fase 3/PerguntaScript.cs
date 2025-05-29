using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fase_3
{
    public class PerguntaScript : MonoBehaviour
    {
        [Header("Opções de Resposta")]
        [SerializeField] private Button[] opcoes;
        [SerializeField] private int indiceCorreto;

        [Header("Falha por Tempo")]
        [SerializeField] private GameObject falhaTempoPanel;
        [SerializeField] private Button falhaNextButton;

        public event Action<bool> OnAnswered;

        private bool _falhaNextClicked;

        void Start()
        {
            // garante que o painel comece desativado
            falhaTempoPanel.SetActive(false);

            // configura cada botão de opção
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

        /// <summary>
        /// Chamado pelo Fase3Manager quando o tempo esgota.
        /// Exibe o painel de falha e prepara o Next button.
        /// </summary>
        public void ShowFalhaTempo()
        {
            // mostra painel
            falhaTempoPanel.SetActive(true);
            _falhaNextClicked = false;

            // desabilita as opções
            foreach (var botao in opcoes)
                botao.interactable = false;
            falhaNextButton.onClick.RemoveAllListeners();
            falhaNextButton.onClick.AddListener(() =>
            {
                _falhaNextClicked = true;
            });
        }

        /// <summary>
        /// Deve ser usado em coroutine: espera até o jogador clicar em Next.
        /// </summary>
        public IEnumerator WaitForFalhaNext()
        {
            while (!_falhaNextClicked)
                yield return null;
            falhaTempoPanel.SetActive(false);
            Fase3Manager.instance.destroyPergunta();
                
        }
        
        public void ForceFinish(bool correto)
        {
            foreach (var botao in opcoes)
                botao.interactable = false;

        }
    }
}
