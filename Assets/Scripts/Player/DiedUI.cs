using UnityEngine;
using UnityEngine.SceneManagement;

public class DiedUI : MonoBehaviour
{
    [SerializeField] private SceneChanger sceneChanger;

    public GameObject diedUI; 

    private void Start()
    {
        if (sceneChanger == null)
            sceneChanger = FindAnyObjectByType<SceneChanger>();
    }

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
        sceneChanger.ChangeSceneAfterDie();
        int health = StatsManager.Instance.currentHealth + StatsManager.Instance.maxHealth;
        StatsManager.Instance.UpdateHealth(health);
    }

    //public void GoToMainMenu()
    //{
    //    SceneManager.LoadScene("MainMenu");
    //}
}
