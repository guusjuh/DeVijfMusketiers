using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AcceptButton
{
    private Button accept;
    public RectTransform Accept { get { return accept.GetComponent<RectTransform>(); } }
    private Button decline;
    private GameObject acceptWindow;

    /// <summary>
    /// assigns functions to buttons and activates acception window. if declineAction is null it will only close the window
    /// </summary>
    public AcceptButton(GameObject acceptWindow, UnityAction acceptAction, string windowMsg = "", UnityAction declineAction = null)
    {
        this.acceptWindow = acceptWindow;
        if (windowMsg != "") acceptWindow.transform.Find("Text").GetComponent<Text>().text = windowMsg;
        accept = acceptWindow.transform.Find("Yes").GetComponent<Button>();
        decline = acceptWindow.transform.Find("No").GetComponent<Button>();
        
        accept.onClick.RemoveAllListeners();
        accept.onClick.AddListener(acceptAction);
        accept.onClick.AddListener(DisableWindow);

        decline.onClick.RemoveAllListeners();
        if (declineAction == null)
        {
            decline.onClick.AddListener(DisableWindow);
        }
        else
        {
            decline.onClick.AddListener(declineAction);
            decline.onClick.AddListener(DisableWindow);
        }
        
        acceptWindow.gameObject.SetActive(true);
    }

    public void DisableWindow()
    {
        acceptWindow.SetActive(false);
        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
    }
}
