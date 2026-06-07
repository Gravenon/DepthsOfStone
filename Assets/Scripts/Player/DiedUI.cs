using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiedUI : MonoBehaviour
{
    public GameObject diedUI;

    [Tooltip("Секунд после смерти до авто-возрождения (время анимации)")]
    [SerializeField] private float respawnDelay = 3f;

    private Coroutine _respawnCoroutine;

    private void OnEnable()  { PlayerHealth.Died += EnableDiedMenu; SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { PlayerHealth.Died -= EnableDiedMenu; SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (diedUI != null) diedUI.SetActive(false);
        if (_respawnCoroutine != null) { StopCoroutine(_respawnCoroutine); _respawnCoroutine = null; }
    }

    public void EnableDiedMenu()
    {
        if (diedUI != null) diedUI.SetActive(true);
        if (_respawnCoroutine != null) StopCoroutine(_respawnCoroutine);
        _respawnCoroutine = StartCoroutine(AutoRespawnAfterDelay());
    }

    private IEnumerator AutoRespawnAfterDelay()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        Respawn();
    }

    // Кнопка «Возродиться» — пропускает таймер.
    public void Respawn()
    {
        if (_respawnCoroutine != null) { StopCoroutine(_respawnCoroutine); _respawnCoroutine = null; }

        DataPersistenceeManager.RespawnMaxHealth = StatsManager.Instance != null ? StatsManager.Instance.maxHealth : 0;
        DataPersistenceeManager.instance.SaveGame();
        DataPersistenceeManager.SuppressNextSave = true;
        DataPersistenceeManager.IsRespawning = true;

        string scene = DataPersistenceeManager.instance.HasCheckpoint()
            ? DataPersistenceeManager.instance.GetCheckpointScene()
            : DataPersistenceeManager.instance.GetLastSavedScene();

        SceneManager.LoadScene(scene);
    }

    public void GoToMainMenu()
    {
        if (_respawnCoroutine != null) { StopCoroutine(_respawnCoroutine); _respawnCoroutine = null; }
        DataPersistenceeManager.instance?.SaveGame();
        DataPersistenceeManager.SuppressNextSave = true;
        SceneManager.LoadScene("Menu");
    }
}
