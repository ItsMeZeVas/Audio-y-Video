using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
    [System.Serializable]
    public class InteractionEvent
    {
        [Header("Objeto que activa la interacción")]
        public GameObject targetObject;              // Objeto interactuable (ej. Libro)

        [Header("Acciones de visibilidad")]
        public List<GameObject> objectsToEnable;     // Objetos que se ACTIVARÁN
        public List<GameObject> objectsToDisable;    // Objetos que se DESACTIVARÁN

        [Header("Video (opcional)")]
        public VideoClip videoClip;                  // Clip a reproducir
        public bool playFullscreen = false;          // ¿Mostrar en pantalla completa?
        public bool stopOtherVideos = false;         // ¿Detener otros videos activos?

        [Header("Mensaje (opcional)")]
        [TextArea] public string message;            // Texto de depuración o diálogo
    }

    [Header("Lista de interacciones configuradas")]
    public List<InteractionEvent> interactionEvents = new List<InteractionEvent>();

    [Header("Opciones de video globales")]
    public RawImage fullscreenVideoUI;               // Imagen del Canvas donde se muestra el video
    public VideoPlayer fullscreenVideoPlayer;        // VideoPlayer global
    public GameObject playerObject;                  // El jugador (para desactivar movimiento)

    private bool isPlayingVideo = false;

    void Start()
    {
        Debug.Log($"InteractionManager activo. Eventos configurados: {interactionEvents.Count}");
        foreach (var ev in interactionEvents)
        {
            if (ev != null && ev.targetObject != null)
                Debug.Log($" - Evento configurado para: {ev.targetObject.name}");
        }

        if (fullscreenVideoUI != null)
            fullscreenVideoUI.gameObject.SetActive(false);
    }

    // Se llama desde el PlayerInteraction cuando se detecta un objeto interactuable
    public void TriggerEvent(GameObject interactedObject)
    {
        if (isPlayingVideo) return; // evita nuevas interacciones durante video

        var ev = interactionEvents.Find(e => e.targetObject == interactedObject);
        if (ev == null)
        {
            Debug.LogWarning($"No se encontró evento para: {interactedObject.name}");
            return;
        }

        Debug.Log($"Interacción ejecutada con: {interactedObject.name}");

        // Activar objetos
        if (ev.objectsToEnable != null)
        {
            foreach (var obj in ev.objectsToEnable)
                if (obj != null) obj.SetActive(true);
        }

        // Desactivar objetos
        if (ev.objectsToDisable != null)
        {
            foreach (var obj in ev.objectsToDisable)
                if (obj != null) obj.SetActive(false);
        }

        // Reproducir video (si aplica)
        if (ev.videoClip != null)
        {
            if (ev.stopOtherVideos)
            {
                foreach (var vp in FindObjectsOfType<VideoPlayer>())
                    vp.Stop();
            }

            if (ev.playFullscreen)
                PlayFullscreenVideo(ev.videoClip);
            else
                Debug.Log("⚠️ El clip está asignado pero no está configurado para pantalla completa.");
        }

        // Mensaje opcional
        if (!string.IsNullOrEmpty(ev.message))
            Debug.Log($"Mensaje: {ev.message}");
    }

    // 🎥 Reproduce el video en pantalla completa
    private void PlayFullscreenVideo(VideoClip clip)
    {
        if (fullscreenVideoPlayer == null || fullscreenVideoUI == null)
        {
            Debug.LogError("❌ No se asignó el VideoPlayer o RawImage para video fullscreen.");
            return;
        }

        isPlayingVideo = true;

        // Mostrar UI y preparar video
        fullscreenVideoUI.gameObject.SetActive(true);
        fullscreenVideoPlayer.clip = clip;
        fullscreenVideoPlayer.Prepare();

        // Bloquear movimiento del jugador
        if (playerObject != null)
        {
            MonoBehaviour[] scripts = playerObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script.enabled && script != this)
                    script.enabled = false;
            }
        }

        // Esperar a que el video esté listo
        fullscreenVideoPlayer.prepareCompleted += (v) =>
        {
            fullscreenVideoPlayer.Play();
        };

        // Cuando termina, desbloquear todo
        fullscreenVideoPlayer.loopPointReached += (v) =>
        {
            fullscreenVideoPlayer.Stop();
            fullscreenVideoUI.gameObject.SetActive(false);
            isPlayingVideo = false;

            // Reactivar movimiento del jugador
            if (playerObject != null)
            {
                MonoBehaviour[] scripts = playerObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (!script.enabled)
                        script.enabled = true;
                }
            }
        };
    }
}
