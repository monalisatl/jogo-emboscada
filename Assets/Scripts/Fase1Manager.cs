using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Fase1Manager : MonoBehaviour
{

    [SerializeField] private GameObject noticiaField;
    [SerializeField] private TextMeshProUGUI noticiaText;
    [SerializeField] private string noticia;

    void Start()
    {
        noticiaField = GameObject.FindGameObjectWithTag("noticia");
        noticiaText = noticiaField.GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   internal class Noticia {
        public string noticia;
        public Image imagem;
        public Noticia(string noticia, Image imagem) {
            this.noticia = noticia;
            this.imagem = imagem;
        }
    }
}
