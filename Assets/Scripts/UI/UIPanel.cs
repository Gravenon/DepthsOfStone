using System;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    private CanvasGroup _cg;

    [SerializeField] private bool openPauseMenuOnClose;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();
        UIManager.SetVisible(_cg, false);
    }

    public void Toggle()
    {
        if (UIManager.Instance != null)
        {
            Action onClose = openPauseMenuOnClose ? () => PauseMenuManager.Instance?.OpenPause() : (Action)null;
            UIManager.Instance.Toggle(_cg, onClose);
        }
        else
            UIManager.SetVisible(_cg, _cg.alpha < 0.5f);
    }
}
