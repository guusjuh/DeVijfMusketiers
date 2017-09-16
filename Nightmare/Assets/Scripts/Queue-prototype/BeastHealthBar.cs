using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastHealthBar : MonoBehaviour
{
    // singleton
    private static BeastHealthBar instance = null;

    public static BeastHealthBar Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType(typeof(BeastHealthBar)) as BeastHealthBar;
            return instance;
        }
    }

    public float maxScale;
    public int maxHealth;
    public int health;
    private RectTransform rTransform;

    public void Start()
    {
        rTransform = gameObject.GetComponent<RectTransform>();
        maxScale = Screen.currentResolution.width;
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Auwh");
        health -= amount;
        float newScale = maxScale - (maxScale / maxHealth * health);
        rTransform.offsetMax = new Vector2(-newScale, 0);
    }

}
