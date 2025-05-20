using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private GameObject loadingScreen;
    private GameObject canva;
    public static int indiceCanvainicial = 0;
    private int indice = 0;
    public static MainManager main;

    void Awake()
    {
        if (main == null) { main = this;}
        else { Destroy(gameObject); return; }
        
        if (LoadingScreenController.Instance == null && loadingScreen != null)
        {
            Instantiate(loadingScreen);
            DontDestroyOnLoad(LoadingScreenController.Instance);
        }
    }


    void Start()
    {
        StartCoroutine(InstanciarCanvaAsync(indiceCanvainicial));
    }
    public IEnumerator InstanciarCanvaAsync(int id)
    {
        // validações
        if (id < 0 || id >= prefabs.Count)
        {
            Debug.LogError("Indice Inválido: " + id);
            yield break;
        }
        if (prefabs[id] == canva)
        {
            Debug.LogError("O Prefab escolhido é igual ao atual");
            yield break;
        }
        
        yield return LoadingScreenController.Instance.ShowLoading(new List<Func<IEnumerator>> {
            () => PrepareSwap(id)
        });
    }
    
    private IEnumerator PrepareSwap(int id)
    {
        if (canva != null)
            Destroy(canva);
        
        yield return null;
        canva = Instantiate(prefabs[id]);
        indice = id;

        yield return null; 
    }


    public void ProximoCanvas()
    {
        int novoIndice = (indice + 1) % prefabs.Count;
        StartCoroutine(InstanciarCanvaAsync(novoIndice));
    }

    public void CanvasAnterior()
    {
        int novoIndice = (indice - 1 + prefabs.Count) % prefabs.Count;
        StartCoroutine(InstanciarCanvaAsync(novoIndice));
    }

    public void IrParaCanvas(int id)
    {
        StartCoroutine(InstanciarCanvaAsync(id));
    }
}
