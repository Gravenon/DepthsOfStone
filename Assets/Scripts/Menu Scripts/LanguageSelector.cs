using System.Collections;
using UnityEditor.Localization.Platform.iOS;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SocialPlatforms;

public class LanguageSelector : MonoBehaviour
{
    private bool active = false;
    
    public void ChangeLanguage(int localeID)
    {
        if(active) 
            return;
        StartCoroutine(SetLocale(localeID));
    }

        IEnumerator SetLocale(int localeID)
        {
            active = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
            PlayerPrefs.SetInt("Language", localeID);
            active = false;
        }
}
