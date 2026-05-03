using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;
    public Animator fadeAnim;
    public float fadeTime = .5f;
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
        {
            fadeAnim.Play("FadeToBlack");
            StartCoroutine(DelayFade());
        }
    }

    public void ChangeScene()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;

        fadeAnim.Play("FadeToBlack");
        StartCoroutine(DelayFade());
    }

    private IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);

        // Set position before saving so the save contains the correct spawn point
        if (_player != null)
            _player.position = newPlayerPosition;

        // Save before unloading — suppress the automatic save on sceneUnloaded to avoid a double-save
        if (DataPersistenceeManager.instance != null)
        {
            DataPersistenceeManager.instance.SaveGame();
            DataPersistenceeManager.SuppressNextSave = true;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_player != null)
            _player.position = newPlayerPosition;
    }
}
