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

    void Start()
    {
        playerCamera = Camera.main;

        if (worldCanvas != null)
            worldCanvas.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (alwaysFacePlayer && worldCanvas.enabled)
        {
            worldCanvas.transform.LookAt(playerCamera.transform);
        }
    }

    // ---- FADING ----

    public void ShowImage()
    {
        if (rawImage == null || worldCanvas == null) return;

        rawImage.texture = imageToShow;
        worldCanvas.gameObject.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCanvas(0f, 1f));
    }

    public void HideImage()
    {
        if (rawImage == null || worldCanvas == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeAndDisable());
    }

    private IEnumerator FadeAndDisable()
    {
        yield return FadeCanvas(1f, 0f);
        worldCanvas.gameObject.SetActive(false);
    }

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
}
