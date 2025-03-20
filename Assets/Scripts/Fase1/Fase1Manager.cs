using System.Collections.Generic;
using UnityEngine;

public class Fase1Manager : MonoBehaviour {
    [SerializeField] private List<Noticia> poolNoticias = new List<Noticia>(); 
    [SerializeField] private int quantidadePerguntas = 4;
    public static List<Noticia> noticiasSelecionadas { get; private set; }
    public static int perguntaAtualIndex = 0;
    public static Fase1Manager instance;
    private bool inicializado = false;
     void Awake()
    {
        if (instance == null)
        {
            instance = this;
            noticiasSelecionadas = new List<Noticia>();
            DontDestroyOnLoad(gameObject);
            InicializarFase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InicializarFase()
    {
        SelecionarNoticiasAleatorias();
        inicializado = true;
    }

    public bool EstaInicializado() => inicializado;
    private void SelecionarNoticiasAleatorias() {
        List<Noticia> noticiasDisponiveis = new List<Noticia>(poolNoticias);
        noticiasSelecionadas.Clear();
        Debug.Log("Selecionando notícias aleatórias...");
        Debug.Log("Notícias disponíveis: " + noticiasDisponiveis.Count);
        Debug.Log("Quantidade de perguntas: " + poolNoticias.Count);

        for (int i = 0; i < quantidadePerguntas; i++) {
            if (noticiasDisponiveis.Count == 0) break;

            int index = UnityEngine.Random.Range(0, noticiasDisponiveis.Count);
            noticiasSelecionadas.Add(noticiasDisponiveis[index]);
            noticiasDisponiveis.RemoveAt(index);
        }
    }

    public Noticia MostrarPerguntaAtual() {
        if (perguntaAtualIndex >= noticiasSelecionadas.Count) {
            Debug.Log("Fim do quiz!");
            return null;
        }

        Noticia noticia = noticiasSelecionadas[perguntaAtualIndex];
        perguntaAtualIndex++;
        return noticia;
    }


}