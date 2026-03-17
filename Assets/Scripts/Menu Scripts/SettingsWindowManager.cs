using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsWindowManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // Resolution dropdown
    [SerializeField] private TMP_Dropdown screenModeDropdown;   // Screen mode dropdown

    private Resolution[] resolutions;
    private List<Resolution> selectedResolutions = new List<Resolution>();

    private int selectedResolutionIndex;
    private int selectedScreenModeIndex;


    void Start()
    {
        // Initialize screen mode dropdown
        screenModeDropdown.AddOptions(new List<string>
        {
            "Windowed",
            "Fullscreen",
            "Borderless Windows"    
        });

        int savedMode = PlayerPrefs.GetInt("ScreenMode", 0);
        screenModeDropdown.value = savedMode;
        screenModeDropdown.RefreshShownValue();
        ApplyScreenMode(savedMode);


        // Initialize resolution dropdown
        resolutions = Screen.resolutions;

        List<string> optionresolution = new List<string>();
        string newRes;
        foreach(Resolution resolution in resolutions)
        {
            int refreshRate = Mathf.RoundToInt((float)resolution.refreshRateRatio.value);
            newRes = $"{resolution.width}x{resolution.height} @ {refreshRate} HZ";
            if(!optionresolution.Contains(newRes))
            {
                optionresolution.Add(newRes);
                selectedResolutions.Add(resolution);
            }
        }
        resolutionDropdown.AddOptions(optionresolution);

        Debug.Log($"Screen modes: {screenModeDropdown.options.Count}");
        Debug.Log($"Current resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height}");
    }


    // Change screen mode based on dropdown selection
    public void ChangeScreenMode()
    {
        selectedScreenModeIndex = screenModeDropdown.value;
        ApplyScreenMode(selectedScreenModeIndex);
        PlayerPrefs.SetInt("ScreenMode", selectedScreenModeIndex);
        PlayerPrefs.Save();
    }

    private void ApplyScreenMode(int index)
    {
        switch (index)
        {
            case 0: // Windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1: // Fullscreen
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2: // Borderless Windows
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }

        Debug.Log($"Screen mode set to: {screenModeDropdown.options[index].text}");
    }

    // Change resolution based on dropdown selection
    public void ChangeResolution()
    {
        selectedResolutionIndex = resolutionDropdown.value;
        Screen.SetResolution(selectedResolutions[selectedResolutionIndex].width, selectedResolutions[selectedResolutionIndex].height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", selectedResolutionIndex);
        PlayerPrefs.Save();
        Debug.Log($"Resolution set to: {resolutionDropdown.options[selectedResolutionIndex].text}");
    }
}
