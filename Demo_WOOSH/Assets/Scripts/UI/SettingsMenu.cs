using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private GameObject creditsPanel;

    public void Initialize()
    {
        Slider musicSlider =
            transform.Find("SettingsMenu").Find("SettingsGrid").Find("Music").Find("Slider").GetComponent<Slider>();
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);

        Slider fxSlider =
            transform.Find("SettingsMenu").Find("SettingsGrid").Find("Soundeffects").Find("Slider").GetComponent<Slider>();
        fxSlider.onValueChanged.AddListener(ChangeFxVolume);

        creditsPanel = transform.Find("CreditsPanel").gameObject;

        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
    }

    public void Deactivate()
    {
        GooglePlayScript.Instance.SaveData();

        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        gameObject.SetActive(false);
    }

    public void ChangeMusicVolume(float real)
    {
        UberManager.Instance.SoundManager.MusicVolume = real;
    }

    public void ChangeFxVolume(float real)
    {
        UberManager.Instance.SoundManager.FXVolume = real;
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
        GooglePlayScript.ShowAchievmentUI();
    }

    public void DeleteProgress()
    {
        GooglePlayScript.Instance.DeleteProgress();
    }
}
