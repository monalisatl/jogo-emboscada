using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class PlantaoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;     private GameObject config;

    void Start(){
        config = GameObject.Find("Video Player");
        if (config != null)
        {
            videoPlayer = config.GetComponent<VideoPlayer>();
        }
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer não encontrado no objeto 'Video Player'. Certifique-se de que o objeto existe e contém o componente VideoPlayer.");
            return;
        }
        videoPlayer.Play();

        // Inicia a coroutine para verificar periodicamente o estado de reprodução
        StartCoroutine(CheckAndPlayAudio());
    }

    private IEnumerator CheckAndPlayAudio()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!videoPlayer.isPlaying && videoPlayer != null)
            {
                mainManager.main.ProximoCanvas();
            }
        }
    }

    void Update()
    {

    }
}