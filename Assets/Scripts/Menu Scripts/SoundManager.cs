using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Keep this on a regular (non-persistent) GameObject in the Menu scene.
// It will be destroyed when leaving the menu and recreated fresh on return.
public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;

    private void Start() => SyncSliders();

    // Called by slider OnValueChanged events in Inspector.
    public void SetMusicVolume()   { if (AudioManager.Instance) AudioManager.Instance.MusicVolume   = musicVolumeSlider.value; }
    public void SetSFXVolume()     { if (AudioManager.Instance) AudioManager.Instance.SfxVolume     = sfxVolumeSlider.value; }
    public void SetAmbientVolume() { if (AudioManager.Instance) AudioManager.Instance.AmbientVolume = ambientVolumeSlider.value; }

    private void SyncSliders()
    {
        if (AudioManager.Instance == null) return;
        if (musicVolumeSlider   != null) musicVolumeSlider.value   = AudioManager.Instance.MusicVolume;
        if (sfxVolumeSlider     != null) sfxVolumeSlider.value     = AudioManager.Instance.SfxVolume;
        if (ambientVolumeSlider != null) ambientVolumeSlider.value = AudioManager.Instance.AmbientVolume;
    }
}
