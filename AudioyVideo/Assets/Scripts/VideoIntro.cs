using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class VideoIntro : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "F1 Conocimiento"; // Nombre de la escena a cargar después del video

    private void Start()
    {
        StartCoroutine(PlayVideoAndContinue());
    }

    IEnumerator PlayVideoAndContinue()
    {
        // Esperar a que cargue el primer frame
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();

        // Esperar a que termine el video
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // Cargar la escena principal
        SceneManager.LoadScene(nextSceneName);
    }
}
