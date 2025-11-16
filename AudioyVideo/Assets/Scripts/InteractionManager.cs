using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonAction
    {
        [Header("Nombre del botón (único dentro del objeto)")]
        public string buttonName;

        [Header("Acciones de visibilidad")]
        public List<GameObject> objectsToEnable;
        public List<GameObject> objectsToDisable;

        [Header("Video (opcional)")]
        public VideoClip videoClip;
        public bool playFullscreen = false;
        public bool stopOtherVideos = false;

        [Header("Mensaje (opcional)")]
        [TextArea] public string message;

        [Header("Notificación (opcional)")]
        public bool showNotification = false;
        [TextArea] public string notificationText;

        [Header("Final del juego (opcional)")]
        public bool isEnding = false;
    }

    [System.Serializable]
    public class InteractionEvent
    {
        [Header("Objeto interactuable")]
        public GameObject targetObject;

        [Header("Botones disponibles para este objeto")]
        public List<ButtonAction> buttonActions = new List<ButtonAction>();
    }

    [Header("Configuración general")]
    public List<InteractionEvent> interactionEvents = new List<InteractionEvent>();

    [Header("Opciones de video globales")]
    public RawImage fullscreenVideoUI;
    public VideoPlayer fullscreenVideoPlayer;
    public GameObject playerObject;

    private bool isPlayingVideo = false;
    private bool shouldQuitAfterVideo = false;

    // ⭐ Fade
    [Header("Fade UI (Panel negro con CanvasGroup)")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 1.2f;

    // ⭐ Notificación lateral
    [Header("Notificación UI")]
    public CanvasGroup notificationPanel;
    public TextMeshProUGUI notificationTextUI;
    public float notificationDuration = 2f;
    public float notificationFade = 0.4f;

    // 🎵 AUDIO AMBIENTAL
    [Header("Audio Ambiental")]
    public AudioSource ambientMusic;
    public float musicFadeDuration = 1.5f;



    void Start()
    {
        Debug.Log($"📦 InteractionManager iniciado con {interactionEvents.Count} objetos configurados.");

        if (fullscreenVideoUI != null)
            fullscreenVideoUI.gameObject.SetActive(false);

        if (fadePanel != null)
            fadePanel.alpha = 0f;

        if (notificationPanel != null)
            notificationPanel.alpha = 0f;

        // 🎵 Música empieza suave
        if (ambientMusic != null)
        {
            ambientMusic.volume = 0f;
            ambientMusic.loop = true;
            ambientMusic.Play();
            StartCoroutine(FadeMusic(0f, 1f));
        }
    }



    // 🔹 Ejecuta la acción asociada a un objeto
    public void TriggerEvent(GameObject interactedObject, string buttonName)
    {
        if (isPlayingVideo) return;

        var ev = interactionEvents.Find(e => e.targetObject == interactedObject);
        if (ev == null)
        {
            Debug.LogWarning($"⚠️ No se encontró evento para: {interactedObject.name}");
            return;
        }

        var button = ev.buttonActions.Find(b => b.buttonName == buttonName);
        if (button == null)
        {
            Debug.LogWarning($"⚠️ El objeto {interactedObject.name} no tiene una acción llamada '{buttonName}'.");
            return;
        }

        Debug.Log($"🔘 Ejecutando acción '{buttonName}' en {interactedObject.name}");

        foreach (var obj in button.objectsToEnable)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in button.objectsToDisable)
            if (obj != null) obj.SetActive(false);


        // Reproducir video
        if (button.videoClip != null)
        {
            if (button.stopOtherVideos)
            {
                foreach (var vp in FindObjectsOfType<VideoPlayer>())
                    vp.Stop();
            }

            if (button.playFullscreen)
            {
                shouldQuitAfterVideo = button.isEnding;

                // 🎵 Cuando inicia un video → bajar música
                if (ambientMusic != null)
                    StartCoroutine(FadeMusic(ambientMusic.volume, 0f));

                PlayFullscreenVideo(button.videoClip);
                return;
            }
        }

        if (!string.IsNullOrEmpty(button.message))
            Debug.Log($"💬 {button.message}");

        if (button.showNotification && !string.IsNullOrEmpty(button.notificationText))
            ShowNotification(button.notificationText);

        if (button.isEnding && button.videoClip == null)
        {
            QuitGame();
        }
    }




    // 🎥 Control de video fullscreen
    private void PlayFullscreenVideo(VideoClip clip)
    {
        if (fullscreenVideoPlayer == null || fullscreenVideoUI == null)
        {
            Debug.LogError("❌ Falta asignar VideoPlayer o RawImage para fullscreen.");
            return;
        }

        StartCoroutine(PlayVideoSequence(clip));
    }




    // ⭐ SECUENCIA COMPLETA DEL VIDEO + AUDIO
    private IEnumerator PlayVideoSequence(VideoClip clip)
    {
        isPlayingVideo = true;

        yield return StartCoroutine(Fade(0f, 1f));

        fullscreenVideoUI.gameObject.SetActive(true);
        fullscreenVideoPlayer.clip = clip;
        fullscreenVideoPlayer.Prepare();

        if (playerObject != null)
        {
            foreach (var script in playerObject.GetComponents<MonoBehaviour>())
            {
                if (script.enabled && script != this)
                    script.enabled = false;
            }
        }

        while (!fullscreenVideoPlayer.isPrepared)
            yield return null;

        fullscreenVideoPlayer.Play();

        yield return StartCoroutine(Fade(1f, 0f));

        // Esperar hasta casi el final del video
        while (fullscreenVideoPlayer.time < fullscreenVideoPlayer.length - fadeDuration - 0.1f)
            yield return null;

        yield return StartCoroutine(Fade(0f, 1f));

        fullscreenVideoPlayer.Stop();
        fullscreenVideoUI.gameObject.SetActive(false);

        isPlayingVideo = false;

        if (playerObject != null)
        {
            foreach (var script in playerObject.GetComponents<MonoBehaviour>())
            {
                if (!script.enabled)
                    script.enabled = true;
            }
        }

        if (shouldQuitAfterVideo)
        {
            QuitGame();
            yield break;
        }

        yield return StartCoroutine(Fade(1f, 0f));

        // 🎵 Cuando termina el video → volver a subir música
        if (ambientMusic != null)
            StartCoroutine(FadeMusic(0f, 1f));
    }



    // ⭐ FUNCIÓN DE FADE UI
    private IEnumerator Fade(float start, float end)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = end;
    }



    // 🎵 FADE Música
    private IEnumerator FadeMusic(float from, float to)
    {
        float t = 0f;

        while (t < musicFadeDuration)
        {
            t += Time.deltaTime;
            if (ambientMusic != null)
                ambientMusic.volume = Mathf.Lerp(from, to, t / musicFadeDuration);
            yield return null;
        }

        if (ambientMusic != null)
            ambientMusic.volume = to;
    }



    // 🔔 Notificación lateral
    public void ShowNotification(string text)
    {
        StartCoroutine(ShowNotificationRoutine(text));
    }

    private IEnumerator ShowNotificationRoutine(string text)
    {
        if (notificationPanel == null || notificationTextUI == null)
        {
            Debug.LogWarning("⚠️ No se asignó la UI de notificación.");
            yield break;
        }

        notificationTextUI.text = text;

        float t = 0f;

        while (t < notificationFade)
        {
            t += Time.deltaTime;
            notificationPanel.alpha = Mathf.Lerp(0f, 1f, t / notificationFade);
            yield return null;
        }
        notificationPanel.alpha = 1f;

        yield return new WaitForSeconds(notificationDuration);

        t = 0f;
        while (t < notificationFade)
        {
            t += Time.deltaTime;
            notificationPanel.alpha = Mathf.Lerp(1f, 0f, t / notificationFade);
            yield return null;
        }
        notificationPanel.alpha = 0f;
    }


    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
