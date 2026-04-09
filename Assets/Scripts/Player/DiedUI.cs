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

    public void Respawn()
    {
        DataPersistenceeManager.SuppressNextSave = true;
        DataPersistenceeManager.IsRespawning = true;

        string targetScene = DataPersistenceeManager.instance.HasCheckpoint()
            ? DataPersistenceeManager.instance.GetCheckpointScene()
            : DataPersistenceeManager.instance.GetLastSavedScene();

        SceneManager.LoadScene(targetScene);
    }

    public void GoToMainMenu()
    {
        DataPersistenceeManager.SuppressNextSave = true;
        SceneManager.LoadScene("Menu");
    }
}
