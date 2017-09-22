using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : Damagable {
    public bool destroyed = false;
    private bool active = false;

    private Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1);
    private Color normalColor;

    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            GetComponent<SpriteRenderer>().color = active ? activeColor : normalColor;
        }
    }

    public void Start()
    {
        normalColor = GetComponent<SpriteRenderer>().color;
    }

    public int GetActionPoints()
    {
        // note destroyed
        return active ? 1 : 0;
    }

    public override bool Hit()
    {
        destroyed = true;
        GameManager.Instance.LevelManager.RemoveShrine(this);

        return true;
    }
}
