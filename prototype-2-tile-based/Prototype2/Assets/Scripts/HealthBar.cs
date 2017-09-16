using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private Creature creatureReference;

    void Update()
    {
        if (creatureReference == null || rectTransform == null)
        {
            creatureReference = FindObjectOfType(typeof(Creature)) as Creature;
            rectTransform = GetComponent<RectTransform>();
        } 

        rectTransform.sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, (creatureReference.Health / 100.0f) * 500.0f);
    }
}
