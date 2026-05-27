using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SettingsWindowManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;

    private Resolution[] resolutions;
    private readonly List<Resolution> filteredResolutions = new List<Resolution>();

    private void Start()
    {
        screenModeDropdown.AddOptions(new List<string> { "Windowed", "Fullscreen", "Borderless" });
        int savedMode = PlayerPrefs.GetInt("ScreenMode", 1);
        screenModeDropdown.value = savedMode;
        screenModeDropdown.RefreshShownValue();
        ApplyScreenMode(savedMode);

        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        var seenResolutions = new HashSet<string>();
        foreach (Resolution r in resolutions)
        {
            string key = $"{r.width}x{r.height}";
            if (seenResolutions.Add(key))
            {
                options.Add(key);
                filteredResolutions.Add(r);
            }
        }
        resolutionDropdown.AddOptions(options);

        int savedRes = PlayerPrefs.GetInt("Resolution", 0);
        resolutionDropdown.value = savedRes;
        resolutionDropdown.RefreshShownValue();
    }

    public void ChangeScreenMode()
    {
        int index = screenModeDropdown.value;
        ApplyScreenMode(index);
        PlayerPrefs.SetInt("ScreenMode", index);
        PlayerPrefs.Save();
    }

    private void ApplyScreenMode(int index)
    {
        Screen.fullScreenMode = index switch
        {
            0 => FullScreenMode.Windowed,
            1 => FullScreenMode.ExclusiveFullScreen,
            2 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.Windowed
        };
    }

    public void ChangeResolution()
    {
        int index = resolutionDropdown.value;
        Resolution r = filteredResolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", index);
        PlayerPrefs.Save();
    }
}
