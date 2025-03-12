using UnityEngine;

public class plantaoManager : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject config;
    void Start()
    {   
        config = GameObject.Find("Audio Source");
        audioSource = config.GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying){
            mainManager.main.ProximoCanvas();
        }
    }

}
