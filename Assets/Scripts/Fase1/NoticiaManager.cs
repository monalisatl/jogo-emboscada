using System.Collections;
using UnityEngine;

public class NoticiaManager : MonoBehaviour
{

    public static NoticiaManager instance;
    [SerializeField] private Noticia noticiaAtual;
    [SerializeField] private GameObject[] noticiaObj = new GameObject[2];

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
        foreach (GameObject obj in noticiaObj)
        {
            GameObject titulo_noticia = GameObject.Find("titulo_noticia").gameObject;
            GameObject data_noticia = GameObject.Find("data_noticia").gameObject;
            GameObject conteudo_noticia = GameObject.Find("conteudo_noticia").gameObject;
            GameObject linkFonte_noticia = GameObject.Find("linkFonte_noticia").gameObject;
            GameObject opcoes_resposta =GameObject.Find("opcoes_resposta").gameObject;
            titulo_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.titulo;
            data_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.data;
            conteudo_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.conteudo;
            linkFonte_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.linkFonte;
            GameObject[] botoes = new GameObject[noticiaAtual.opcoesResposta.Count];
            if(obj.name == "noticia"){
                for (int i = 0; i < noticiaAtual.opcoesResposta.Count; i++)
                {     botoes[i] = GameObject.Find("botao "+ (i+1));
                    botoes[i].GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.opcoesResposta[i].texto;
                    if (noticiaAtual.opcoesResposta[i].isCorreto)
                    {
                        botoes[i].GetComponent<BotaoOpcao>().isCorreto = true;
                    }
                }
            }
        }   
    }
    public void CarregarProximaNoticia()
    {
        noticiaAtual = Fase1Manager.instance.MostrarPerguntaAtual();
        
        if(noticiaAtual != null)
        {
                   foreach (GameObject obj in noticiaObj)
        {
            GameObject titulo_noticia = GameObject.Find("titulo_noticia").gameObject;
            GameObject data_noticia = GameObject.Find("data_noticia").gameObject;
            GameObject conteudo_noticia = GameObject.Find("conteudo_noticia").gameObject;
            GameObject linkFonte_noticia = GameObject.Find("linkFonte_noticia").gameObject;
            GameObject opcoes_resposta = GameObject.Find("opcoes_resposta").gameObject;
            titulo_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.titulo;
            data_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.data;
            conteudo_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.conteudo;
            linkFonte_noticia.GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.linkFonte;
            GameObject[] botoes = new GameObject[noticiaAtual.opcoesResposta.Count];
            if(obj.name == "noticia"){
                for (int i = 0; i < noticiaAtual.opcoesResposta.Count; i++)
                {     botoes[i] = GameObject.Find("botao "+ (i+1));
                    botoes[i].GetComponent<TMPro.TextMeshProUGUI>().text = noticiaAtual.opcoesResposta[i].texto;
                    if (noticiaAtual.opcoesResposta[i].isCorreto)
                    {
                        botoes[i].GetComponent<BotaoOpcao>().isCorreto = true;
                    }
                }
            }
        }  
        }
        else
        {
            Debug.Log("Fim das notícias!");
        }
    }
}
