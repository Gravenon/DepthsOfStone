using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

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
