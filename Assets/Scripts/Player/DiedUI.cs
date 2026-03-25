using UnityEngine;
using UnityEngine.SceneManagement;

public class DiedUI : MonoBehaviour
{
    public GameObject diedUI;

    private void OnEnable()
    {
        PlayerHealth.Died += EnableDiedMenu;
    }

    private void OnDisable()
    {
        PlayerHealth.Died -= EnableDiedMenu;
    }

    public void EnableDiedMenu()
    {
        diedUI.SetActive(true);
    }

    /// <summary>
    /// Reloads the scene where the last checkpoint was triggered.
    /// OnSceneLoaded → LoadGame() restores position, stats and inventory from the save file.
    /// </summary>
    public void Respawn()
    {
        // Suppress the auto-save on scene unload so we don't overwrite the
        // checkpoint save with the dead state.
        DataPersistenceeManager.SuppressNextSave = true;

        string targetScene = DataPersistenceeManager.instance.GetLastSavedScene();
        SceneManager.LoadScene(targetScene);
    }

    public void GoToMainMenu()
    {
        DataPersistenceeManager.SuppressNextSave = true;
        SceneManager.LoadScene("Menu");
    }
}
