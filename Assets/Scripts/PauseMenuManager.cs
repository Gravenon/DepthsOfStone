using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    [Header("Scene Settings")]
    [Tooltip("Exact name of the Main Menu scene in Build Settings.")]
    [SerializeField] private string mainMenuSceneName = "Menu";

    public static bool IsGamePaused;

    private void Start()
    {
        pauseMenu.SetActive(false);
        IsGamePaused = false;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        pauseMenu.SetActive(false);
    }

    private void Pause()
    {
        IsGamePaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        DataPersistenceeManager.instance.SaveGame();
        Time.timeScale = 1f;
        IsGamePaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        DataPersistenceeManager.instance.SaveGame();
        Application.Quit();
    }
}
