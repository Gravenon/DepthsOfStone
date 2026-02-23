using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WorldTimeWatcher : MonoBehaviour
{
    private WorldTime worldTime;

    [SerializeField]
    private List<Schedule> schedules;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryBindWorldTime();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (worldTime != null)
        {
            worldTime.WorldTimeChange -= OnWorldTimeChange;
        }
    }

    private void Start()
    {
        TryBindWorldTime();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryBindWorldTime();
    }

    private void TryBindWorldTime()
    {
        WorldTime targetWorldTime = WorldTime.Instance;

        if (targetWorldTime == null)
        {
            targetWorldTime = FindFirstObjectByType<WorldTime>();
        }

        if (targetWorldTime == null)
        {
            Debug.LogWarning("WorldTimeWatcher: WorldTime not found.");
            return;
        }

        if (worldTime != null && worldTime != targetWorldTime)
        {
            worldTime.WorldTimeChange -= OnWorldTimeChange;
        }

        worldTime = targetWorldTime;

        worldTime.WorldTimeChange -= OnWorldTimeChange;
        worldTime.WorldTimeChange += OnWorldTimeChange;
    }

    private void OnDestroy()
    {
        if (worldTime != null)
        {
            worldTime.WorldTimeChange -= OnWorldTimeChange;
        }
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        var schedule = schedules.FirstOrDefault(s => s.hour == newTime.Hours && s.minute == newTime.Minutes);

        schedule?.action?.Invoke();
    }

    [Serializable]
    private class Schedule
    {
        public int hour;
        public int minute;
        public UnityEvent action;
    }
}
