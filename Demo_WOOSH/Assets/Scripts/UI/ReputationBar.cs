using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReputationBar : MonoBehaviour
{

    private RectTransform transformForeground;
    private float maxFromRight;

    public void Initialize()
    {
        transformForeground = transform.Find("Foreground").GetComponent<RectTransform>();
        maxFromRight = transform.GetComponent<RectTransform>().sizeDelta.x;
    }

    // Sets health immediately (when other enemy clicked) 
    public void SetBar(float current)
    {
        float percentage = (current / 100) * 100;
        transformForeground.offsetMax = -new Vector2((maxFromRight / 100) * (100 - percentage), 0);
    }
}
