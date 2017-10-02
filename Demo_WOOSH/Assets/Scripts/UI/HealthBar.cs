using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    private RectTransform transformForeground;
    private float minFromRight;
    private float maxFromRight;
    private float height;

    public void Initialize()
    {
        transformForeground = transform.Find("Foreground").GetComponent<RectTransform>();
        minFromRight = 0;
        maxFromRight = transform.GetComponent<RectTransform>().sizeDelta.x;
        height = transform.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Sets health immediately (when other enemy clicked) 
    public void SetHealthbar(float percentage)
    {
        transformForeground.offsetMax = -new Vector2((maxFromRight / 100) * (100 - percentage), 0);
    }

    // Adjusts and animates healthbar (enemy taking dmg while being selected)
    public void AdjustHealthbar(int percentage)
    {
        
    }
}
