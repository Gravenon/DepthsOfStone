using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Light2D))]

public class WorldLight : MonoBehaviour
{
    private static WorldLight instance;
    private Light2D worldLight;

    [SerializeField]
    private WorldTime worldTime;

    [SerializeField]
    private Gradient gradient;

    [SerializeField]
    private string[] disabledScenes;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        worldLight = GetComponent<Light2D>();
        worldTime.WorldTimeChange += OnWorldTimeChange;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (worldTime == null)
            worldTime = FindFirstObjectByType<WorldTime>();
    }

    private void OnDestroy()
    {
        worldTime.WorldTimeChange -= OnWorldTimeChange;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Disable any Global Light2D objects from the loaded scene that duplicate our persistent light
        foreach (var l in FindObjectsByType<Light2D>(FindObjectsSortMode.None))
        {
            if (l != worldLight && l.lightType == Light2D.LightType.Global)
            {
                l.enabled = false;
            }
        }

        bool disable = false;

        foreach (var s in disabledScenes)
        {
            if (scene.name == s)
            {
                disable = true;
                break;
            }
        }

        worldLight.enabled = !disable;
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
       worldLight.color = gradient.Evaluate(PercentOfDay(newTime));
    }

    private float PercentOfDay(TimeSpan time)
    {
        return (float)(time.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay);
    }
}
