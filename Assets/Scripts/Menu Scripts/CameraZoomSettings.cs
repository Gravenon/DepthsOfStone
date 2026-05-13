using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CameraZoomSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown zoomModeDropdown;

    private void Start()
    {
        zoomModeDropdown.ClearOptions();
        zoomModeDropdown.AddOptions(new List<string> { "Far", "Normal", "Close" });

        int saved = PlayerPrefs.GetInt("ZoomMode", 1);
        zoomModeDropdown.value = saved;
    }

    public void ChangeZoomMode()
    {
        int index = zoomModeDropdown.value;
        PlayerPrefs.SetInt("ZoomMode", index);
        PlayerPrefs.Save();
        CameraPersistence.Instance?.ApplyZoom(index);
    }
}
