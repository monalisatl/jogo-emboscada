using System.Collections;
using UnityEngine;

public class NoticiaManager : MonoBehaviour
{

    public static NoticiaManager instance;
    [SerializeField] private Noticia noticiaAtual;
    [SerializeField] public GameObject[] noticiaObj = new GameObject[2];

    [Header("Noticia aberta")]
    [SerializeField] private GameObject titulo_noticia_aberta;
    [SerializeField] private GameObject data_noticia_aberta;
    [SerializeField] private GameObject conteudo_noticia_aberta;
    [SerializeField] private GameObject linkFonte_noticia_aberta;
    [Header("Noticia")]
    [SerializeField] private GameObject titulo_noticia;
    [SerializeField] private GameObject data_noticia;
    [SerializeField] private GameObject conteudo_noticia;
    [SerializeField] private GameObject linkFonte_noticia;
    [Header("Botões")]
    [SerializeField] private GameObject[] botoes = new GameObject[4];

    private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
    private IEnumerator Start()
    {
        // Espera até o Fase1Manager estar totalmente inicializado
        yield return new WaitUntil(() => Fase1Manager.instance != null && Fase1Manager.instance.EstaInicializado());
        
        InicializarNoticia();
    }

    
    void InicializarNoticia() {

        if(noticiaObj[0] == null || noticiaObj[1] == null) {
        Debug.LogError("Objetos da notícia não encontrados!");
        return;
    }
        noticiaAtual = Fase1Manager.instance.MostrarPerguntaAtual();
        if (noticiaAtual != null)
        {
            carregarNoticia(noticiaAtual);
        }
    }
    public void CarregarProximaNoticia(){
        
        noticiaAtual = Fase1Manager.instance.MostrarPerguntaAtual();
        if (noticiaAtual != null)
        {
            carregarNoticia(noticiaAtual);
        }
        else
        {
            Debug.Log("Fim do quiz!");
            
        }
    }


    void carregarNoticia(Noticia noticiaAtual)
    {       

            // titulo_noticia_aberta = GameObject.Find("titulo_noticia_aberta").gameObject;
            // data_noticia_aberta = GameObject.Find("data_noticia_aberta").gameObject;
            // conteudo_noticia_aberta = GameObject.Find("conteudo_noticia_aberta").gameObject;
            // linkFonte_noticia_aberta = GameObject.Find("linkFonte_noticia_aberta").gameObject;
            // titulo_noticia = GameObject.Find("titulo_noticia").gameObject;
            // data_noticia = GameObject.Find("data_noticia").gameObject;
            // conteudo_noticia = GameObject.Find("conteudo_noticia").gameObject;
            // linkFonte_noticia = GameObject.Find("linkFonte_noticia").gameObject;
            data_noticia_aberta.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.data;
            titulo_noticia_aberta.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.titulo;
            conteudo_noticia_aberta.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.conteudo;
            linkFonte_noticia_aberta.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.linkFonte;
            titulo_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.titulo;
            data_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.data;
            conteudo_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.conteudo;
            linkFonte_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.linkFonte;
                for (int i = 0; i < noticiaAtual.opcoesResposta.Count; i++)
                { 
                    botoes[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = noticiaAtual.opcoesResposta[i].texto;
                    if (noticiaAtual.opcoesResposta[i].isCorreto)
                    {
                        botoes[i].GetComponent<BotaoOpcao>().isCorreto = true;
                    }
                }
        }
    }

