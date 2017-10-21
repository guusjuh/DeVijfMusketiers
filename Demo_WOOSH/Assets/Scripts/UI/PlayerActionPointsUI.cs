using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionPointsUI : MonoBehaviour {
    private Text playerAPText;
    private Image playerAPBackground;
    private Color normalColorPlayerAP;
    private Color dissabledColorPlayerAP;

    public void Initialize()
    {
        playerAPText = gameObject.GetComponentInChildren<Text>();
        playerAPBackground = gameObject.GetComponentInChildren<Image>();
        normalColorPlayerAP = playerAPBackground.color;
        dissabledColorPlayerAP = Color.grey;
    }

    public void SetAPText()
    {
        playerAPText.text = GameManager.Instance.LevelManager.Player.CurrentActionPoints + "";

        if (GameManager.Instance.LevelManager.Player.CurrentActionPoints <= 0)
            playerAPBackground.color = dissabledColorPlayerAP;
        else
            playerAPBackground.color = normalColorPlayerAP;
    }
}
