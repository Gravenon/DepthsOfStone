using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsWindowManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    // Full screne toggle 

    Resolution[] resolutions;
    int selectedResolutionIndex;
    List<Resolution> selectedResolutions = new List<Resolution>();

    void Start()
    {
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
    }

    public void ChangeResolution()
    {
        selectedResolutionIndex = resolutionDropdown.value;
        Screen.SetResolution(selectedResolutions[selectedResolutionIndex].width, selectedResolutions[selectedResolutionIndex].height, Screen.fullScreen);
    }

    void Update()
    {
        
    }
}
