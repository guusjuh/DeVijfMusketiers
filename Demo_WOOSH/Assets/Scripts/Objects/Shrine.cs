using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : EnemyTarget
{
    private bool active = false;

    private static Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private static Color normalColor = new Color(0.55f, 0.55f, 0.55f, 1.0f);

    private SpriteRenderer sprRender;

    public bool Active {
        get { return active; }
        private set {
            active = value;
            sprRender.color = active ? activeColor : normalColor;
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        sprRender = GetComponent<SpriteRenderer>();
        type = TileManager.ContentType.Shrine;
    }

    public override bool Hit()
    {
        canBeTargeted = false;
        active = false;

        sprRender.color = normalColor;

        GameManager.Instance.LevelManager.RemoveShrine(this);

        return true;
    }

    public void CheckForActive()
    {
        // check for standing next to a shrine
        //TODO: obtain humans through levelmanager
        List<Human> all = new List<Human>();
        all.AddMultiple(FindObjectsOfType<Human>() as Human[]);

        // each human
        for (int i = 0; i < all.Count; i++)
        {
            // each neighbouring node
            for (int j = 0; j < GameManager.Instance.TileManager.Directions(gridPosition).Length; j++)
            {
                if (Mathf.Abs(gridPosition.x - (all[i].GridPosition.x + GameManager.Instance.TileManager.Directions(gridPosition)[j].x)) < 0.1f
                    && Mathf.Abs(gridPosition.y - (all[i].GridPosition.y + GameManager.Instance.TileManager.Directions(gridPosition)[j].y)) < 0.1f)
                {
                    Active = true;
                    return;
                }
            }
        }

        Active = false;
    }
}
