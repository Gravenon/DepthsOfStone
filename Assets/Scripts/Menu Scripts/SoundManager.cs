using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;

    private void Start()
    {
        SyncSliders();
    }

    public void SetMusicVolume()
    {
        if (AudioManager.Instance != null && musicVolumeSlider != null)
            AudioManager.Instance.MusicVolume = musicVolumeSlider.value;
    }

    public void SetSFXVolume()
    {
        if (AudioManager.Instance != null && sfxVolumeSlider != null)
            AudioManager.Instance.SfxVolume = sfxVolumeSlider.value;
    }

    public void SetAmbientVolume()
    {
        if (AudioManager.Instance != null && ambientVolumeSlider != null)
            AudioManager.Instance.AmbientVolume = ambientVolumeSlider.value;
    }

    private void SyncSliders()
    {
        if (AudioManager.Instance == null) return;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = AudioManager.Instance.MusicVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = AudioManager.Instance.SfxVolume;

        if (ambientVolumeSlider != null)
            ambientVolumeSlider.value = AudioManager.Instance.AmbientVolume;
    }
}