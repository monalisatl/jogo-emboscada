using System.Collections.Generic;
using UnityEngine;

public class Fase2Manager : MonoBehaviour
{
    public static Fase2Manager instance;

    [Header("Pool de notícias")]
    public List<Noticia> poolNoticias;
    public int quantidadePerguntas = 4;
    public static float statusFase2 =0;
    public static List<Noticia> perguntasSelecionadas;
    private int currentIndex = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SelecionarNoticias();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Sorteia sem repetição
    private void SelecionarNoticias()
    {
        var disponiveis = new List<Noticia>(poolNoticias);
        perguntasSelecionadas = new List<Noticia>();
        for (int i = 0; i < quantidadePerguntas && disponiveis.Count > 0; i++)
        {
            int idx = Random.Range(0, disponiveis.Count);
            perguntasSelecionadas.Add(disponiveis[idx]);
            disponiveis.RemoveAt(idx);
        }
    }
    
    public Noticia GetNextQuestion()
    {
        if (currentIndex < perguntasSelecionadas.Count)
            return perguntasSelecionadas[currentIndex++];
        return null;
    }

    public int TotalPerguntas => perguntasSelecionadas.Count;
}