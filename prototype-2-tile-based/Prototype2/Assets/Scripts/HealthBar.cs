using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private Creature creatureReference;

    void Update()
    {
        if (GameManager.Instance.Creature == null)
        {
            rectTransform.sizeDelta = new Vector2((0.0f / 100.0f) * 1000.0f, GetComponent<RectTransform>().sizeDelta.y);
            return;
        }

        if (creatureReference == null || rectTransform == null)
        {
            creatureReference = FindObjectOfType(typeof(Creature)) as Creature;
            rectTransform = GetComponent<RectTransform>();
        } 

        rectTransform.sizeDelta = new Vector2((creatureReference.Health / 100.0f) * 1000.0f, GetComponent<RectTransform>().sizeDelta.y);
    }
}
