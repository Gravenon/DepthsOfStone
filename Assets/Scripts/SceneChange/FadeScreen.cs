using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    public static FadeScreen Instance;
    public float fadeInDuration = 0.4f;
    public float fadeOutDuration = 0.6f;

    
    public float holdAfterLoad = 0.15f;

    private CanvasGroup _cg;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildOverlay();
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }


    /// Fades screen to black. Await in a coroutine before loading the scene.
    public IEnumerator FadeIn()
    {
        _cg.blocksRaycasts = true;
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));
    }

    /// Fades screen from black. Called automatically on scene load.
    public IEnumerator FadeOut()
    {
        _cg.alpha = 1f;
        _cg.blocksRaycasts = true;
        yield return new WaitForSecondsRealtime(holdAfterLoad);
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));
        _cg.blocksRaycasts = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        _cg.alpha = from;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            _cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        _cg.alpha = to;
    }

    private void BuildOverlay()
    {
        // Root Canvas — always on top
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        // Black full-screen image
        var imgObj = new GameObject("BlackOverlay");
        imgObj.transform.SetParent(transform, false);
        var img  = imgObj.AddComponent<Image>();
        img.color = Color.black;
        var rect = imgObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // CanvasGroup on overlay for alpha control
        _cg = imgObj.AddComponent<CanvasGroup>();
        _cg.alpha = 0f;
        _cg.blocksRaycasts = false;
        _cg.interactable = false;
    }
}

