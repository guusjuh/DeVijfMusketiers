using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private Sprite onSprite;
    private Sprite offSprite;

    private Image musicStatusImg;
    private Image fxStatusImg;

    private GameObject creditsPanel;

    public void Initialize()
    {
        onSprite = Resources.Load<Sprite>("Sprites/UI/Settings/On");
        offSprite = Resources.Load<Sprite>("Sprites/UI/Settings/Off");

        Transform settingsGrid = transform.Find("SettingsMenu").Find("SettingsGrid");

        Button musicButton = settingsGrid.Find("Music").Find("Button").GetComponent<Button>();
        musicButton.onClick.AddListener(SwitchMusic);
        musicStatusImg = musicButton.transform.Find("Image").GetComponent<Image>();
        musicStatusImg.sprite = SoundManager.MusicOn ? onSprite : offSprite;

        Button fxButton = settingsGrid.Find("Soundeffects").Find("Button").GetComponent<Button>();
        fxButton.onClick.AddListener(SwitchSoundeffects);
        fxStatusImg = fxButton.transform.Find("Image").GetComponent<Image>();
        fxStatusImg.sprite = SoundManager.FXOn ? onSprite : offSprite;

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
        GooglePlayScript.Instance.SaveData();

        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        gameObject.SetActive(false);
    }

    public void SwitchMusic()
    {
        SoundManager.MusicOn = !SoundManager.MusicOn;
        musicStatusImg.sprite = SoundManager.MusicOn ? onSprite : offSprite;
    }

    public void SwitchSoundeffects()
    {
        SoundManager.FXOn = !SoundManager.FXOn;
        fxStatusImg.sprite = SoundManager.FXOn ? onSprite : offSprite;
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
