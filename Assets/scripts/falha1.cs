using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class falha1 : MonoBehaviour
{ 
    private AudioSource player;
   [SerializeField] private int fase;
    private IEnumerator Start()
    {
        var audioGO = GameObject.Find("Audio");
        if (audioGO == null)
        {
            Debug.LogError("FalhaManager: não encontrou GameObject 'Audio'");
            yield break;
        }
        
        player = audioGO.GetComponent<AudioSource>();
        if (player == null)
        {
            Debug.LogError("FalhaManager: AudioSource não encontrado em 'Audio'");
            yield break;
        }
        
        player.pitch = 2f;
        player.Play();
        
        yield return new WaitUntil(() => !player.isPlaying);
        if (MainManager.main != null)
        {
           
            MainManager.main.IrParaCanvas(fase);
        }
        else
        {
            Debug.LogWarning("FalhaManager: mainManager.main não inicializado.");
        }
    }
}
