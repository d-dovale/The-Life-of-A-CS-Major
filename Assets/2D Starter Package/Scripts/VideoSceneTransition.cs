using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneTransition : MonoBehaviour
{
    public int nextSceneIndex;
    VideoPlayer videoPlayer;
    
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    
    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}