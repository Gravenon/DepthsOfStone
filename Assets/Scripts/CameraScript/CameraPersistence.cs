using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to the Cameras GameObject.
/// Makes the camera DontDestroyOnLoad and re-finds the Player by tag on every scene load.
/// </summary>
public class CameraPersistence : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private string playerTag = "Player";

    private static CameraPersistence instance;
    public static CameraPersistence Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null && vcam != null)
        {
            vcam.Follow = player.transform;
        }

        // Apply saved zoom
        int saved = PlayerPrefs.GetInt("ZoomMode", 1);
        ApplyZoom(saved);
    }

    public void ApplyZoom(int index)
    {
        if (vcam == null) return;
        int[] options = { 7, 5, 4 };
        if (index >= 0 && index < options.Length)
            vcam.Lens.OrthographicSize = options[index];
    }
}


