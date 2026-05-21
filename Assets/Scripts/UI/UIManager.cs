using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private CanvasGroup _activeGroup;
    private Action _onForcedClose;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (_activeGroup != null)
            CloseActive();
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _activeGroup  = null;
        _onForcedClose = null;
    }

    public void Toggle(CanvasGroup group, Action onForcedClose = null)
    {
        if (PauseMenuManager.IsGamePaused) return;
        if (_activeGroup == group) { CloseActive(); return; }
        CloseActive();
        OpenGroup(group, onForcedClose);
    }

    public void NotifyOpened(CanvasGroup group, Action onForcedClose = null)
    {
        if (PauseMenuManager.IsGamePaused) return;
        if (_activeGroup == group) return;
        CloseActive();
        _activeGroup   = group;
        _onForcedClose = onForcedClose;
        SetVisible(group, true);
    }

    public void Close()      => CloseActive();
    public void ForceClose() { _activeGroup = null; _onForcedClose = null; }

    public bool IsAnyOpen => _activeGroup != null;

    private void OpenGroup(CanvasGroup group, Action onForcedClose)
    {
        _activeGroup   = group;
        _onForcedClose = onForcedClose;
        SetVisible(group, true);
    }

    private void CloseActive()
    {
        if (_activeGroup == null) return;
        SetVisible(_activeGroup, false);
        _activeGroup = null;
        var cb = _onForcedClose;
        _onForcedClose = null;
        cb?.Invoke();
    }

    public static void SetVisible(CanvasGroup cg, bool visible)
    {
        cg.alpha          = visible ? 1f : 0f;
        cg.blocksRaycasts = visible;
        cg.interactable   = visible;
    }
}
