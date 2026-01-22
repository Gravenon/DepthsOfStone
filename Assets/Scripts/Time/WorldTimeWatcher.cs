using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WorldTimeWatcher : MonoBehaviour
{
    [SerializeField]
    private WorldTime worldTime;

    [SerializeField]
    private List<Schedule> schedules;

    private void Start()
    {
        worldTime.WorldTimeChange += OnWorldTimeChange;
    }

    private void OnDestroy()
    {
        worldTime.WorldTimeChange -= OnWorldTimeChange;
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
