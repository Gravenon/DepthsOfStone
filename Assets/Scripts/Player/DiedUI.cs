using UnityEngine;
using UnityEngine.SceneManagement;

public class DiedUI : MonoBehaviour
{
    public GameObject diedUI;

    private void OnEnable()  { PlayerHealth.Died += EnableDiedMenu; SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { PlayerHealth.Died -= EnableDiedMenu; SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (diedUI != null) diedUI.SetActive(false); }

    public void EnableDiedMenu() { if (diedUI != null) diedUI.SetActive(true); }

    public void Respawn()
    {
        DataPersistenceeManager.RespawnMaxHealth = StatsManager.Instance != null ? StatsManager.Instance.maxHealth : 0;
        DataPersistenceeManager.instance.SaveGame();
        DataPersistenceeManager.SuppressNextSave = true;
        DataPersistenceeManager.IsRespawning     = true;

        string scene = DataPersistenceeManager.instance.HasCheckpoint()
            ? DataPersistenceeManager.instance.GetCheckpointScene()
            : DataPersistenceeManager.instance.GetLastSavedScene();

        SceneManager.LoadScene(scene);
    }

    public void GoToMainMenu()
    {
        DataPersistenceeManager.instance?.SaveGame();
        DataPersistenceeManager.SuppressNextSave = true;
        SceneManager.LoadScene("Menu");
    }
}
