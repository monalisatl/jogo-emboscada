using UnityEngine;

namespace Fase_2
{
    public class GerenciarNotica : MonoBehaviour
    {   
        [SerializeField] private GameObject noticia;

        void Start()
        {
            noticia = GameObject.Find("noticia_aberta");
        }
        public void FecharNoticia()
        {   
            noticia.SetActive(false);

        }

        public void AbrirNoticia()
        {
            noticia.SetActive(true);
        }
    }
}
