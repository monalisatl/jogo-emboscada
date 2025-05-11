using System.Collections;
using UnityEngine;

public class Fase3Manager : MonoBehaviour
{
    [Header("Prefabs de Etapas")]
    [SerializeField] private Fase3InstrucaoScript instrucaoPrefab;
    [SerializeField] private secretarioVideoConfig videoPrefab;
    [SerializeField] private PerguntaScript pergunta1Prefab;
    [SerializeField] private PerguntaScript pergunta2Prefab;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject vitoriaPrefab;
    [SerializeField] private GameObject derrotaPrefab;

    private bool _resp1, _resp2;

    void Start()
    {
        StartCoroutine(RunFase());
    }

    private IEnumerator RunFase()
    {
        var instr = Instantiate(instrucaoPrefab);
        bool doneInstr = false;
        instr.OnComplete += () => doneInstr = true;
        yield return new WaitUntil(() => doneInstr);
        
        yield return ShowLoading();
        var vid1 = Instantiate(videoPrefab);
        bool doneVid1 = false;
        vid1.OnVideoEnd += () => doneVid1 = true;
        yield return new WaitUntil(() => doneVid1);
        
        var p1 = Instantiate(pergunta1Prefab);
        bool answered1 = false;
        p1.OnAnswered += (certo) => { _resp1 = certo; answered1 = true; };
        yield return new WaitUntil(() => answered1);
        
        yield return ShowLoading();
        var vid2 = Instantiate(videoPrefab);
        bool doneVid2 = false;
        vid2.OnVideoEnd += () => doneVid2 = true;
        yield return new WaitUntil(() => doneVid2);
        
        var p2 = Instantiate(pergunta2Prefab);
        bool answered2 = false;
        p2.OnAnswered += (certo) => { _resp2 = certo; answered2 = true; };
        yield return new WaitUntil(() => answered2);

        // 6) Vitória ou Derrota
        if (_resp1 && _resp2)
            Instantiate(vitoriaPrefab);
        else
            Instantiate(derrotaPrefab);
    }

    private IEnumerator ShowLoading()
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        loadingScreen.SetActive(false);
    }
}
