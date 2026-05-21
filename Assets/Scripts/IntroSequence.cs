using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class IntroSequence : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TextMeshProUGUI loreText;

    [Header("Lore Lines")]
    [TextArea(2, 5)]
    [SerializeField] private string[] loreLines;

    [Header("Timing")]
    [SerializeField] private float charDelay = 0.04f;
    [SerializeField] private float readTime = 3f;
    [SerializeField] private float textFadeInTime = 0.5f;
    [SerializeField] private float textFadeOutTime = 0.7f;
    [SerializeField] private float panelFadeOutTime = 0.8f;
    [SerializeField] private float delayBetweenLines = 0.3f;

    [Header("On Finished")]
    public UnityEvent onIntroFinished;

    [Header("Debug")]
    [SerializeField] private bool forcePlayIntro = false;

    private PlayerMovment _player;

    private void Awake()
    {
        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.blocksRaycasts = true;
    }

    private void Start()
    {
        bool shouldPlay = forcePlayIntro ||
                          (DataPersistenceeManager.instance != null && DataPersistenceeManager.instance.IsFirstTime);

        if (shouldPlay)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _player = playerObj.GetComponent<PlayerMovment>();

            _player?.SetMovementLocked(true);
            SetTextAlpha(0f);
            StartCoroutine(PlayIntro());
        }
        else
        {
            // Not first time — just fade out the black screen, skip intro & cutscene
            StartCoroutine(FadePanel(1f, 0f, panelFadeOutTime));
        }
    }

    private IEnumerator PlayIntro()
    {
        foreach (string line in loreLines)
        {
            yield return StartCoroutine(FadeTextColor(0f, 1f, textFadeInTime));

            loreText.text = "";
            foreach (char c in line)
            {
                loreText.text += c;
                yield return new WaitForSeconds(charDelay);
            }

            yield return new WaitForSeconds(readTime);
            yield return StartCoroutine(FadeTextColor(1f, 0f, textFadeOutTime));

            loreText.text = "";
            yield return new WaitForSeconds(delayBetweenLines);
        }

        yield return StartCoroutine(FadePanel(1f, 0f, panelFadeOutTime));
        FinishIntro();
    }

    private IEnumerator FadePanel(float from, float to, float duration)
    {
        float elapsed = 0f;
        panelCanvasGroup.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        panelCanvasGroup.alpha = to;
        panelCanvasGroup.blocksRaycasts = to > 0.5f;
    }

    private IEnumerator FadeTextColor(float from, float to, float duration)
    {
        float elapsed = 0f;
        SetTextAlpha(from);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetTextAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetTextAlpha(to);
    }

    private void SetTextAlpha(float a)
    {
        Color c = loreText.color;
        c.a = a;
        loreText.color = c;
    }

    private void FinishIntro()
    {
        DataPersistenceeManager.instance?.MarkIntroPlayed();
        _player?.SetMovementLocked(false);
        loreText.text = "";
        onIntroFinished?.Invoke();
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.blocksRaycasts = false;
    }

    public void SkipIntro()
    {
        StopAllCoroutines();
        FinishIntro();
    }
}
