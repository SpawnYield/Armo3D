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
            EditorApplication.isPlaying = false;  // ������������� ��������������� � ���������
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
        PlayerPrefs.SetFloat(SensivityKey,value); // ��������� ��� ������
        PlayerPrefs.Save(); // ��������� ����������
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
        // ����� ������ �� � ���� (��������, "en", "ru")
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            Debug.Log($"������ ������� �����������: {locale.LocaleName}");
            PlayerPrefs.SetString(LocaleKey, localeCode); // ��������� ��� ������
            PlayerPrefs.Save(); // ��������� ����������
        }
        else
        {
            Debug.LogWarning($"������ � ����� '{localeCode}' �� �������.");
        }
    }
    public void LoadSavedLocale()
    {
        if (PlayerPrefs.HasKey(LocaleKey)) // ���������, ������� �� ��� ������
        {
            string savedLocaleCode = PlayerPrefs.GetString(LocaleKey);
            SetLocale(savedLocaleCode);
            Debug.Log($"��������� ���������� ������: {savedLocaleCode}");
        }
        else
        {
            Debug.Log("���������� ������ �� �������. ������������ ������ �� ���������.");
        }
    }
    public static void LoadSavedSensivity()
    {
        if (PlayerPrefs.HasKey(SensivityKey)) // ���������, ������� �� ��� ������
        {
            Sensivity= PlayerPrefs.GetFloat(SensivityKey);
            Debug.Log($"��������� ���������� SensivityKey: {Sensivity}");
        }
        else
        {
            Debug.Log("���������� SensivityKey �� �������. ������������ Sensivity �� ���������.");
        }
    }
    
}
