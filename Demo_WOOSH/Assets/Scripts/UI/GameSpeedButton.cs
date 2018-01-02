using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedButton : MonoBehaviour {
    private Text text;

    public void Initialize()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector3(325.0f, 100.0f);

        text = transform.Find("Speed").Find("SpeedText").GetComponent<Text>();
        SetText();
    }

    private bool SetText()
    {
        if(text != null)
        {
            text.text = "x" + UberManager.Instance.GameSpeed.ToString();
            return true;
        }

        return false;
    }

    public void OnClick()
    {
        UberManager.Instance.AdjustGameSpeed();
        SetText();
    }
}
