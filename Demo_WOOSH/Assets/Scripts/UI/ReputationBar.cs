using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReputationBar : MonoBehaviour
{

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
    public void SetBar(float current)
    {
        float percentage = (current / 100) * 100;
        transformForeground.offsetMax = -new Vector2((maxFromRight / 100) * (100 - percentage), 0);
    }
}
