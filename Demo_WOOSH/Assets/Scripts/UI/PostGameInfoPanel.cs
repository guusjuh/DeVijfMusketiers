using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameInfoPanel : MonoBehaviour
{
    private Text statusText;
    private const string WIN_STRING = "Level completed!";
    private const string LOSE_STRING = "Defeated...";

    private Text humansLostText;
    private const string HUMANS_LOST_STRING1 = "You have lost ";
    private const string HUMANS_LOST_STRING2 = " humans.";

    public void Initialize()
    {
        statusText = transform.Find("StatusText").GetComponent<Text>();
        humansLostText = transform.Find("Text").GetComponent<Text>();

        SetText();
    }

    public void Restart()
    {
        SetText();
    }

    private void SetText()
    {
        //TODO: make pritty animations
        statusText.text = GameManager.Instance.Won ? WIN_STRING : LOSE_STRING;
        humansLostText.text = HUMANS_LOST_STRING1 + GameManager.Instance.SelectedContracts.FindAll(c => c.Died).Count + HUMANS_LOST_STRING2;
    }
}
