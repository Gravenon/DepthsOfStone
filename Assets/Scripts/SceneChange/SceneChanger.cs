using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;
    public Vector2 newPlayerPosition;

    public bool selectedLevel;
    public MineManager mineManager;

    private Transform _player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _player = collision.transform;

        if (selectedLevel && mineManager != null)
            mineManager.ShowLevelSelect();
        else
            StartCoroutine(DoTransition());
    }

    public void ChangeScene()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        if (FadeScreen.Instance != null)
            yield return StartCoroutine(FadeScreen.Instance.FadeIn());

        if (_player != null)
            _player.position = newPlayerPosition;

        if (DataPersistenceeManager.instance != null)
        {
            DataPersistenceeManager.instance.SaveGame();
            DataPersistenceeManager.SuppressNextSave = true;
        }

        // Synchronous load — FadeScreen.OnSceneLoaded handles fade-out after load.
        SceneManager.LoadScene(sceneName);
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_player != null)
            _player.position = newPlayerPosition;
    }
}
