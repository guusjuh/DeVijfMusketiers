using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public int maxScale;
    public int maxHealth;
    private Shadow enemy;
    private RectTransform rTransform;

	// Use this for initialization
	void Start ()
    {
        enemy = FindObjectOfType(typeof(Shadow)) as Shadow;
        maxScale = Screen.currentResolution.width;
        rTransform = gameObject.GetComponent<RectTransform>();
        maxHealth = enemy.health;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float newScale = maxScale - ((maxScale / maxHealth ) * enemy.health);
        rTransform.offsetMax = new Vector2(100, -newScale);
      //  this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, (enemy.health / 30.0f) * 499);
	}
}
