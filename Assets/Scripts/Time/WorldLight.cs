using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]

public class WorldLight : MonoBehaviour
{
    private Light2D worldLight;

    [SerializeField]
    private WorldTime worldTime;

    [SerializeField]
    private Gradient gradient;

    private void Awake()
    {
        worldLight = GetComponent<Light2D>();
        worldTime.WorldTimeChange += OnWorldTimeChange;
    }

    private void OnDestroy()
    {
        worldTime.WorldTimeChange -= OnWorldTimeChange;
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
