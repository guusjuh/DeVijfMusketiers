using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : Damagable {
    public bool destroyed = false;
    private bool active = false;

    private Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1);
    private Color normalColor;

    static Vector3[] positions = { new Vector3(0, 1), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, -1) };

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
        cannotBeTarget = true;
        GameManager.Instance.LevelManager.RemoveShrine(this);

        return true;
    }

    public void CheckForActive()
    {
        // check for standing next to a shrine
        List<Human> all = new List<Human>();
        all.AddMultiple(FindObjectsOfType<Human>() as Human[]);

        // each human
        for (int i = 0; i < all.Count; i++)
        {
            // each neighbouring node
            for (int j = 0; j < positions.Length; j++)
            {
                if (Mathf.Abs(x - (all[i].x + positions[j].x)) < 0.1f
                    && Mathf.Abs(y - (all[i].y + positions[j].y)) < 0.1f)
                {
                    Active = true;
                    return;
                }
            }
        }

        Active = false;
    }
}
