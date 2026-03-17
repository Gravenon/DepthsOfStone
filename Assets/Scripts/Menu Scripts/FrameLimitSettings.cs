using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FrameLimitSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown frameLimitDropdown; // Frame limit dropdown

    private readonly int[] frameLimitOptions = { 30, 60, 120, 144, -1};
    private int selectedFrameLimitIndex;

    private void Start()
    {
        frameLimitDropdown.ClearOptions();
        frameLimitDropdown.AddOptions(new List<string>
        {
            "30",
            "60",
            "120",
            "144",
            "Unlimited"
        });
        
        int saved = PlayerPrefs.GetInt("FrameLimit", 2); // Default to 60 FPS
        frameLimitDropdown.value = saved;
        ApplyFrameLimit(saved);
    }

    public void ChangeFPSLimit()
    {
        selectedFrameLimitIndex = frameLimitDropdown.value;
        ApplyFrameLimit(selectedFrameLimitIndex);
        PlayerPrefs.SetInt("FrameLimit", selectedFrameLimitIndex);
        PlayerPrefs.Save();
    }

    private void ApplyFrameLimit(int index)
    {
        QualitySettings.vSyncCount = 0; // VSync всегда выкл
        Application.targetFrameRate = frameLimitOptions[index];
        Debug.Log($"Frame limit set to: {(frameLimitOptions[index] == -1 ? "Unlimited" : frameLimitOptions[index].ToString())} FPS");
    }
}
