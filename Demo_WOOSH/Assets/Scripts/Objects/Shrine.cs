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
        List<Human> humans = GameManager.Instance.LevelManager.Humans;

        // each human
        for (int i = 0; i < humans.Count; i++)
        {
            // each neighbouring node
            for (int j = 0; j < GameManager.Instance.TileManager.Directions(humans[i].GridPosition).Length; j++)
            {             
                if (Mathf.Abs(gridPosition.x - (humans[i].GridPosition.x + GameManager.Instance.TileManager.Directions(humans[i].GridPosition)[j].x)) < 0.1f
                    && Mathf.Abs(gridPosition.y - (humans[i].GridPosition.y + GameManager.Instance.TileManager.Directions(humans[i].GridPosition)[j].y)) < 0.1f)
                {
                    Active = true;
                    return;
                }
            }
        }

        Active = false;
    }
}
