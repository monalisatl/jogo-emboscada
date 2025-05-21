using System.Collections.Generic;
using UnityEngine;

namespace Fase_2
{
    public class Fase2Manager : MonoBehaviour
    {
        public static Fase2Manager instance;

        [Header("Pool de notícias")]
        public List<Noticia> poolNoticias;
        public int quantidadePerguntas = 4;
        public static float statusFase2 = 0;
        public static List<Noticia> perguntasSelecionadas;
        public static bool isRepescagemMode = false;
        private int currentIndex = 0;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Verificar se é modo repescagem
                isRepescagemMode = PlayerPrefs.GetInt("repescagem1", 0) == 1;
                
                ResetState();
                SelecionarNoticias();
                
                Debug.Log($"Fase2Manager inicializado. Modo repescagem: {isRepescagemMode}");
            }
            else
            {
                if (isRepescagemMode)
                {
                    if (perguntasSelecionadas == null || perguntasSelecionadas.Count == 0)
                    {
                        instance.ResetState();
                        instance.SelecionarNoticias();
                        Debug.Log("Recriadas perguntas para repescagem");
                    }
                    
                }
                Destroy(gameObject);
            }
        }
        
        private void ResetState()
        {
            statusFase2 = 0f;
            perguntasSelecionadas = null;
            currentIndex = 0;
        }
        public static void ResetRepescagem()
        {
            perguntasSelecionadas = null;
            statusFase2 = 0;
            isRepescagemMode = true;
            PlayerPrefs.SetInt("repescagem1", 1);
            PlayerPrefs.Save();
        }
        // Sorteia sem repetição
        private void SelecionarNoticias()
        {
            // Verificação de segurança
            if (poolNoticias == null || poolNoticias.Count == 0)
            {
                Debug.LogError("Pool de notícias está vazio ou não foi configurado!");
                return;
            }
            
            var disponiveis = new List<Noticia>(poolNoticias);
            perguntasSelecionadas = new List<Noticia>();
            int qtdReal = Mathf.Min(quantidadePerguntas, disponiveis.Count);
            
            for (int i = 0; i < qtdReal; i++)
            {
                int idx = Random.Range(0, disponiveis.Count);
                perguntasSelecionadas.Add(disponiveis[idx]);
                disponiveis.RemoveAt(idx);
            }
            
            Debug.Log($"Selecionadas {perguntasSelecionadas.Count} notícias de {poolNoticias.Count}");
        }
    
        public Noticia GetNextQuestion()
        {
            if (perguntasSelecionadas == null || currentIndex >= perguntasSelecionadas.Count)
                return null;
                
            return perguntasSelecionadas[currentIndex++];
        }

        public int TotalPerguntas => perguntasSelecionadas?.Count ?? 0;
    }
}
