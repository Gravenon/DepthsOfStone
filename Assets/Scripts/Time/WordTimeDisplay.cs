using System;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TMP_Text))]
public class WordTimeDisplay : MonoBehaviour
{
    [SerializeField]
    private WorldTime worldTime;
    private TMP_Text timeText;

    private void Awake()
    {
        timeText = GetComponent<TMP_Text>();
        worldTime.WorldTimeChange += OnWorldTimeChange;
    }

    private void OnDestroy()
    {
        worldTime.WorldTimeChange -= OnWorldTimeChange;
    }

    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        timeText.SetText(newTime.ToString(@"hh\:mm"));
    }
}
