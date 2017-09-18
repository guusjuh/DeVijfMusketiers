using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    public float maxScale;
    public float maxHealth;
    private Shadow enemy;
    private RectTransform rTransform;
    private float deduction;

    // Use this for initialization
    void Start()
    {
        enemy = FindObjectOfType(typeof(Shadow)) as Shadow;
        maxScale = Screen.currentResolution.width;
        rTransform = gameObject.GetComponent<RectTransform>();
        maxHealth = enemy.health;
    }

    // Update is called once per frame
    void Update()
    {
        deduction = (((maxScale / 50) * enemy.health));
        float newScale = maxScale - deduction;
        Debug.Log("deduction " + deduction);
        Debug.Log("newScale " + newScale);
        Debug.Log("maxHealth " + maxHealth);
        Debug.Log("enemy.health " + enemy.health);
        rTransform.offsetMax = new Vector2(50, (-newScale / 3.7f));
        //  this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, (enemy.health / 30.0f) * 499);
    }
}
