using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private RectTransform transformForeground;
    private Text text;
    private float minFromRight;
    private float maxFromRight;
    private float height;

    public void Initialize()
    {
        transformForeground = transform.Find("Foreground").GetComponent<RectTransform>();
        text = transform.Find("Text").GetComponent<Text>();
        minFromRight = 0;
        maxFromRight = transform.GetComponent<RectTransform>().sizeDelta.x;
        height = transform.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Sets health immediately (when other enemy clicked) 
    public void SetHealthbar(Enemy enemy)
    {
        if (enemy != null)
        {
            transformForeground.offsetMax = -new Vector2((maxFromRight / 100) * (100 - enemy.HealthPercentage), 0);
            text.text = "" + enemy.Health + "/" + enemy.StartHealth;
        }
    }

    // Adjusts and animates healthbar (enemy taking dmg while being selected)
    public void AdjustHealthbar(int percentage)
    {
        
    }
}
