using UnityEngine;

public class FalhaManager : MonoBehaviour
{
    private GameObject audioPlay;
    private AudioSource player;
    private static int cout = 0;
    void Start()
    {
        audioPlay = GameObject.Find("Audio");
        player = audioPlay.GetComponent<AudioSource>();
        if (player != null && player.gameObject.activeInHierarchy && player.enabled)
        {
            player.pitch = 2.0f;
            player.Play();
        }
    }

    void Update()
    {
        if (player != null && !player.isPlaying)
        {

            if (mainManager.main != null)
            {
                {
                    mainManager.main.ProximoCanvas();
                }

            }
            else
            {
                Debug.LogWarning("mainManager.main is not initialized.");
            }
        }
    }
}
