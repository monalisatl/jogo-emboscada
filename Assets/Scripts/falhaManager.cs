using UnityEngine;
using UnityEngine.Video;

public class falhaManager : MonoBehaviour
{   
    private GameObject videoMg;
    private VideoPlayer player;
    void Start()
    {   videoMg = GameObject.Find("Video Player");
        player = videoMg.GetComponent<VideoPlayer>();
        player.Play();
    }

    void Update()
    {
        if(!player.isPlaying){
            mainManager.main.ProximoCanvas();
        }
    }


}
