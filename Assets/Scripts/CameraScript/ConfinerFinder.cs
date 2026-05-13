using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ConfinerFinder : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
        if (confiner == null) return;

        GameObject confinerObj = GameObject.FindWithTag("Confiner");
        if (confinerObj == null) return;

        confiner.BoundingShape2D = confinerObj.GetComponent<PolygonCollider2D>();
    }
}
