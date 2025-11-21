using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoSceneVideoQuit : MonoBehaviour
{
    [Header("Asignar VideoPlayer en la escena")]
    public VideoPlayer videoPlayer;

    [Header("Opcional: iniciar después de unos segundos")]
    public float delayBeforeStart = 0f;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("❌ No hay VideoPlayer asignado en AutoSceneVideoQuit.");
            return;
        }

        // Cuando el video termine → cerrar juego
        videoPlayer.loopPointReached += OnVideoEnd;

        // Empezar el video
        StartCoroutine(PlayVideoAfterDelay());
    }

    IEnumerator PlayVideoAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        QuitGame();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // parar play mode
#else
        Application.Quit();  // cerrar build real
#endif
    }
}
