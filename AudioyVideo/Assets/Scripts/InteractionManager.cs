using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;

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

        [Header("Final del juego (opcional)")]
        public bool isEnding = false; // 🔹 Si esta acción marca el final del juego
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
    private bool shouldQuitAfterVideo = false; // 🔹 Nuevo: indica si se debe cerrar el juego después del video

    void Start()
    {
        Debug.Log($"📦 InteractionManager iniciado con {interactionEvents.Count} objetos configurados.");

        if (fullscreenVideoUI != null)
            fullscreenVideoUI.gameObject.SetActive(false);
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

        // Activar / desactivar objetos
        foreach (var obj in button.objectsToEnable)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in button.objectsToDisable)
            if (obj != null) obj.SetActive(false);

        // Reproducir video (si aplica)
        if (button.videoClip != null)
        {
            if (button.stopOtherVideos)
            {
                foreach (var vp in FindObjectsOfType<VideoPlayer>())
                    vp.Stop();
            }

            if (button.playFullscreen)
            {
                // Si es final del juego, esperamos a que el video termine para cerrar
                shouldQuitAfterVideo = button.isEnding;
                PlayFullscreenVideo(button.videoClip);
                return; // 🔹 Esperamos al final del video antes de hacer cualquier cierre
            }
        }

        // Mostrar mensaje (si aplica)
        if (!string.IsNullOrEmpty(button.message))
            Debug.Log($"💬 {button.message}");

        // Si es final del juego pero sin video, cerramos inmediatamente
        if (button.isEnding && button.videoClip == null)
        {
            Debug.Log("🏁 Acción marcada como final del juego. Cerrando aplicación...");
            QuitGame();
        }
    }

    // 🎥 Control del video en pantalla completa
    private void PlayFullscreenVideo(VideoClip clip)
    {
        if (fullscreenVideoPlayer == null || fullscreenVideoUI == null)
        {
            Debug.LogError("❌ Falta asignar VideoPlayer o RawImage para fullscreen.");
            return;
        }

        isPlayingVideo = true;
        fullscreenVideoUI.gameObject.SetActive(true);
        fullscreenVideoPlayer.clip = clip;
        fullscreenVideoPlayer.Prepare();

        // Desactivar scripts del jugador temporalmente
        if (playerObject != null)
        {
            foreach (var script in playerObject.GetComponents<MonoBehaviour>())
            {
                if (script.enabled && script != this)
                    script.enabled = false;
            }
        }

        fullscreenVideoPlayer.prepareCompleted += (v) => fullscreenVideoPlayer.Play();

        fullscreenVideoPlayer.loopPointReached += (v) =>
        {
            fullscreenVideoPlayer.Stop();
            fullscreenVideoUI.gameObject.SetActive(false);
            isPlayingVideo = false;

            // Reactivar scripts del jugador
            if (playerObject != null)
            {
                foreach (var script in playerObject.GetComponents<MonoBehaviour>())
                {
                    if (!script.enabled)
                        script.enabled = true;
                }
            }

            // 🔹 Si debe cerrarse después del video
            if (shouldQuitAfterVideo)
            {
                Debug.Log("🎬 Video final terminado. Cerrando el juego...");
                shouldQuitAfterVideo = false;
                QuitGame();
            }
        };
    }

    // 🔹 Método para cerrar el juego correctamente
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Detiene el modo Play en el editor
#else
        Application.Quit(); // Cierra la aplicación compilada
#endif
    }
}
