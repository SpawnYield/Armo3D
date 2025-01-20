#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    private readonly static string LocaleKey = "Language";
    private readonly static string SensivityKey = "Sensivity";
    private string TranslatedSensivityText;

    public Slider SensivitySlider;
    public TMP_Text TMPSensivityInfo_Text;
    public void ExitOfGame()
    {

    #if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;  // Останавливает воспроизведение в редакторе
        }
    #else
        Application.Quit();
    #endif
    }
    private void Start()
    {
        LoadSavedLocale();
        LoadSavedSensivity();
        SensivitySlider.value = Sensivity;
    }
    public static float Sensivity=0.75f;
    public void SaveSensivity(Single value)
    {
        PlayerPrefs.SetFloat(SensivityKey,value); // Сохраняем код локали
        PlayerPrefs.Save(); // Применяем сохранения
        Sensivity=value;
        SetSensivityText();
    }

    public void SetSensivityTextTranslate(string _text)
    {
        TranslatedSensivityText = _text;
        SetSensivityText();
    }
    public void SetSensivityText()
    {
        TMPSensivityInfo_Text.text = TranslatedSensivityText+":"+Sensivity;
    }
    public void SetLocale(string localeCode)
    {
        // Найти локаль по её коду (например, "en", "ru")
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            Debug.Log($"Локаль успешно установлена: {locale.LocaleName}");
            PlayerPrefs.SetString(LocaleKey, localeCode); // Сохраняем код локали
            PlayerPrefs.Save(); // Применяем сохранения
        }
        else
        {
            Debug.LogWarning($"Локаль с кодом '{localeCode}' не найдена.");
        }
    }
    public void LoadSavedLocale()
    {
        if (PlayerPrefs.HasKey(LocaleKey)) // Проверяем, сохранён ли код локали
        {
            string savedLocaleCode = PlayerPrefs.GetString(LocaleKey);
            SetLocale(savedLocaleCode);
            Debug.Log($"Загружена сохранённая локаль: {savedLocaleCode}");
        }
        else
        {
            Debug.Log("Сохранённая локаль не найдена. Используется локаль по умолчанию.");
        }
    }
    public static void LoadSavedSensivity()
    {
        if (PlayerPrefs.HasKey(SensivityKey)) // Проверяем, сохранён ли код локали
        {
            Sensivity= PlayerPrefs.GetFloat(SensivityKey);
            Debug.Log($"Загружена сохранённая SensivityKey: {Sensivity}");
        }
        else
        {
            Debug.Log("Сохранённая SensivityKey не найдена. Используется Sensivity по умолчанию.");
        }
    }
    
}
