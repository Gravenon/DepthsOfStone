using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private string mainMenuSceneName = "Menu";

    public static bool IsGamePaused;

    private CanvasGroup _pauseCG;

    private void Start()
    {
        _pauseCG = pauseMenu.GetComponent<CanvasGroup>() ?? pauseMenu.AddComponent<CanvasGroup>();
        UIManager.SetVisible(_pauseCG, false);
        IsGamePaused = false;
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (UIManager.Instance != null && UIManager.Instance.IsAnyOpen) return;
        if (IsGamePaused) Resume(); else Pause();
    }

    public void Resume() { Time.timeScale = 1f; IsGamePaused = false; UIManager.SetVisible(_pauseCG, false); }
    private void Pause() { IsGamePaused = true; Time.timeScale = 0f; UIManager.SetVisible(_pauseCG, true); }

    public void Retry()
    {
        Time.timeScale = 1f; IsGamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        DataPersistenceeManager.instance.SaveGame();
        DataPersistenceeManager.SuppressNextSave = true;
        Time.timeScale = 1f; IsGamePaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame() { DataPersistenceeManager.instance.SaveGame(); Application.Quit(); }
}
