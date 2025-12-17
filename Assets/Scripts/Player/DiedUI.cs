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
        SceneManager.LoadScene("VilageDwarfs");
    }

    //public void GoToMainMenu()
    //{
    //    SceneManager.LoadScene("MainMenu");
    //}
}
