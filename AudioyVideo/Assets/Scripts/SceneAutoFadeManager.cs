using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneAutoFadeManager : MonoBehaviour
{
    public static SceneAutoFadeManager Instance;

    [Header("Duración del Fade")]
    public float fadeDuration = 1f;

    private Image fadeImage;
    private Canvas fadeCanvas;
    private bool isFading = false;
    private bool sceneIsLoading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateFadeCanvas();

            // Interceptamos los cambios de escena
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateFadeCanvas()
    {
        fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;
        DontDestroyOnLoad(fadeCanvas.gameObject);

        fadeImage = new GameObject("FadeImage").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false);
        fadeImage.color = new Color(0, 0, 0, 0); // inicio transparente

        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // Detecta cuando Unity cambia de escena
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        StartCoroutine(FadeIn());
    }

    // Función pública que intercepta cualquier LoadScene
    public void LoadSceneWithFade(string sceneName)
    {
        if (!isFading)
            StartCoroutine(FadeOutIn(sceneName));
    }

    private IEnumerator FadeOutIn(string sceneName)
    {
        sceneIsLoading = true;
        yield return FadeOut();

        SceneManager.LoadScene(sceneName);
        sceneIsLoading = false;
    }

    private IEnumerator FadeIn()
    {
        if (isFading) yield break;
        isFading = true;

        float t = fadeDuration;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
        isFading = false;
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
        isFading = false;
    }
}
