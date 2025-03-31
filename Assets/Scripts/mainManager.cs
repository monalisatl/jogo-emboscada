using System.Collections.Generic;
using UnityEngine;

public class mainManager : MonoBehaviour
{
   [SerializeField] public List<GameObject> prefabs;
    private GameObject canva;
    public int indiceCanvainicial = 0;
    private int indice = 0;
    public static mainManager main;

    void Start()
    {
        InstanciarCanva(indiceCanvainicial);
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstanciarCanva(int id){
        if(id < 0 || id >= prefabs.Count){
            Debug.LogError("Indice Inválido: " + id);
            return;
        }
        if( prefabs[id].Equals(canva)){
            Debug.LogError("O Prefab escolhido é igual ao atual");
            return;
        }
        if( canva != null){
            Destroy(canva);

        }

        canva = Instantiate(prefabs[id]);
        indice = id;
    }

    public void ProximoCanvas()
    {
        int novoIndice = indice + 1;
        if (novoIndice >= prefabs.Count)
        {
            novoIndice = 0; 
        }
        InstanciarCanva(novoIndice);
    }

    public void CanvasAnterior()
    {
        int novoIndice = indice - 1;
        if (novoIndice < 0)
        {
            novoIndice = prefabs.Count - 1;
        }
        InstanciarCanva(novoIndice);
    }

    
    public void IrParaCanvas(int indice)
    {   
        InstanciarCanva(indice);
    
    }
}
