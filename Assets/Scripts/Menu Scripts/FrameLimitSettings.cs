using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FrameLimitSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown frameLimitDropdown;

    private readonly int[] frameLimitOptions = { 30, 60, 120, 144, -1 };

    private void Start()
    {
        frameLimitDropdown.ClearOptions();
        frameLimitDropdown.AddOptions(new List<string> { "30", "60", "120", "144", "Unlimited" });

        int saved = PlayerPrefs.GetInt("FrameLimit", 1);
        frameLimitDropdown.value = saved;
        ApplyFrameLimit(saved);
    }

    public void ChangeFPSLimit()
    {
        int index = frameLimitDropdown.value;
        ApplyFrameLimit(index);
        PlayerPrefs.SetInt("FrameLimit", index);
        PlayerPrefs.Save();
    }

    private void ApplyFrameLimit(int index)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameLimitOptions[index];
    }
}
