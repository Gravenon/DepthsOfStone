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

        StatsManager.Instance.ApplyPlayerData(data);

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerHealth.Revive();

        diedUI.SetActive(false);
    }

    //public void GoToMainMenu()
    //{
    //    SceneManager.LoadScene("MainMenu");
    //}
}
