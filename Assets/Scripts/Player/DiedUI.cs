using System.Collections;
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
        PlayerData data = SaveSystem.LodadPlayer();
        if (data == null) return;

        StartCoroutine(RespawnRoutine(data));

        StatsManager.Instance.ApplyPlayerData(data);

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerHealth.Revive();
    }

    //public void GoToMainMenu()
    //{
    //    SceneManager.LoadScene("MainMenu");
    //}

    IEnumerator RespawnRoutine(PlayerData data)
    {
        SceneManager.LoadScene(data.sceneName);

        yield return null;
        yield return null;

        diedUI.SetActive(false);
    }
}
