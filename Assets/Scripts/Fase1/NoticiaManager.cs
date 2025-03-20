using UnityEngine;

public class NoticiaManager : MonoBehaviour
{

    public static NoticiaManager instance;
    [SerializeField] private Noticia noticiaAtual;
    [SerializeField] private GameObject[] noticiaObj;

    void Awake()
    {
        noticiaObj = new GameObject[2];
        noticiaObj[0] = GameObject.Find("noticia");
        noticiaObj[1] = GameObject.Find("noticia_aberta");
        noticiaAtual = Fase1Manager.instance.MostrarPerguntaAtual();
        foreach (GameObject obj in noticiaObj)
        {
            GameObject titulo_noticia = obj.transform.Find("titulo_noticia").gameObject;
            GameObject data_noticia = obj.transform.Find("data_noticia").gameObject;
            GameObject conteudo_noticia = obj.transform.Find("conteudo_noticia").gameObject;
            GameObject linkFonte_noticia = obj.transform.Find("linkFonte_noticia").gameObject;
            GameObject opcoes_resposta = obj.transform.Find("opcoes_resposta").gameObject;
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
