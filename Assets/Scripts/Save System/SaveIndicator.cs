using System.Collections;
using UnityEngine;

public class SaveIndicator : MonoBehaviour
{
    public static SaveIndicator Instance;
    [SerializeField] private GameObject saveIndicatorUI;
    [SerializeField] private float displayDuration = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this; 
        else  Destroy(gameObject); return; 
    }

    public void Show()
    {
        StopAllCoroutines();
        saveIndicatorUI.SetActive(true);
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(displayDuration);
        saveIndicatorUI.SetActive(false);
    }
}
