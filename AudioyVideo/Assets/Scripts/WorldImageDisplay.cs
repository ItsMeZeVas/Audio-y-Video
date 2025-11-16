using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldImageDisplay : MonoBehaviour
{
    [Header("Canvas en world space hijo del objeto")]
    public Canvas worldCanvas;

    [Header("RawImage dentro del canvas")]
    public RawImage rawImage;

    [Header("Imagen que se mostrará al mirar el objeto")]
    public Texture imageToShow;

    [Header("Ajustes")]
    public bool alwaysFacePlayer = true;
    public float fadeDuration = 0.3f;

    private Camera playerCamera;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        playerCamera = Camera.main;
    }

    void OnEnable()
    {
        // Forzar alpha en 0 y canvas apagado
        if (rawImage != null)
            rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 0);

        if (worldCanvas != null)
            worldCanvas.enabled = false;

        // Arreglar que no aparezca rotado o invertido al activarse
        FixRotation();
    }

    void LateUpdate()
    {
        if (alwaysFacePlayer && worldCanvas != null && worldCanvas.enabled)
        {
            FixRotation();
        }
    }

    // ======= APARECER =======

    public void ShowImage()
    {
        if (!IsSafe()) return;

        rawImage.texture = imageToShow;

        worldCanvas.enabled = true;

        // Fijar rotación CORRECTA justo al activarse
        FixRotation();

        RestartCoroutine(FadeCanvas(0f, 1f));
    }

    // ======= DESAPARECER =======

    public void HideImage()
    {
        if (!IsSafe()) return;

        RestartCoroutine(FadeAndDisable());
    }

    private IEnumerator FadeAndDisable()
    {
        yield return FadeCanvas(1f, 0f);

        if (worldCanvas != null)
            worldCanvas.enabled = false;
    }

    // ======= FADE =======

    private IEnumerator FadeCanvas(float from, float to)
    {
        float t = 0;
        Color c = rawImage.color;

        while (t < fadeDuration)
        {
            float blend = t / fadeDuration;
            float a = Mathf.Lerp(from, to, blend);

            rawImage.color = new Color(c.r, c.g, c.b, a);
            t += Time.deltaTime;
            yield return null;
        }

        rawImage.color = new Color(c.r, c.g, c.b, to);
    }

    // ======= FIX ROTATION =======
    private void FixRotation()
    {
        if (playerCamera == null || worldCanvas == null) return;

        Vector3 dir = worldCanvas.transform.position - playerCamera.transform.position;

        // Asegura que nunca quede invertido
        if (dir.sqrMagnitude > 0.001f)
            worldCanvas.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    // ======= HELPERS =======

    private bool IsSafe()
    {
        return gameObject.activeInHierarchy && worldCanvas != null && rawImage != null;
    }

    private void RestartCoroutine(IEnumerator routine)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(routine);
    }
}
