using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private RectTransform transformForeground;
    private Text text;
    private float maxFromRight;

    public void Initialize()
    {
        transformForeground = transform.Find("Foreground").GetComponent<RectTransform>();
        text = transform.Find("Text").GetComponent<Text>();
        maxFromRight = transform.GetComponent<RectTransform>().sizeDelta.x;
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
}
