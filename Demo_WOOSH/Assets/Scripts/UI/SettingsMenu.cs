using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private GameObject creditsPanel;
    private Slider musicSlider;
    private Slider fxSlider;

    public void Initialize()
    {
        musicSlider = transform.Find("SettingsMenu").Find("SettingsGrid").Find("Music").Find("Slider").GetComponent<Slider>();
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);

        fxSlider = transform.Find("SettingsMenu").Find("SettingsGrid").Find("Soundeffects").Find("Slider").GetComponent<Slider>();
        fxSlider.onValueChanged.AddListener(ChangeFxVolume);

        creditsPanel = transform.Find("CreditsPanel").gameObject;

        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
    }

    public void Deactivate()
    {
        UberManager.Instance.Save();

        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        gameObject.SetActive(false);
    }

    public void ChangeMusicVolume(float real)
    {
        SoundManager.MusicVolume = real;
        musicSlider.value = real;
    }

    public void ChangeFxVolume(float real)
    {
        SoundManager.FXVolume = real;
        fxSlider.value = real;
    }

    public void OpenCredits()
    {
        creditsPanel.gameObject.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.gameObject.SetActive(false);
    }

    public void OpenAchievements()
    {
        Debug.Log("achievements");
        GooglePlayServices.ShowAchievmentUI();
    }
}
