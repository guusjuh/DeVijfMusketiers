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
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-10.0f, 10.0f, 0.0f);

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
