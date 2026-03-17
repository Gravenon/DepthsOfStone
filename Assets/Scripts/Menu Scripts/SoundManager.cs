using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    
    public Slider musicVolumeSlider;

    void Start()
    {
        if(PlayerPrefs.HasKey("MusicVolume"))
        {
            LoadVolume();
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", .5f);
            LoadVolume();
        }

    }

    public void SetMusicVolume()
    {
        AudioListener.volume = musicVolumeSlider.value;
        SaveVolume();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    public void LoadVolume()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume"); 
    }

 
}
