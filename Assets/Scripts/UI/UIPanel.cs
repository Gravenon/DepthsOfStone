using UnityEngine;

public class UIPanel : MonoBehaviour
{
    private CanvasGroup _cg;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();
        UIManager.SetVisible(_cg, false);
    }

    public void Toggle()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.Toggle(_cg);
        else
            UIManager.SetVisible(_cg, _cg.alpha < 0.5f);
    }
}
