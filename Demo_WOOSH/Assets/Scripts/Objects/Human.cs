using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MovableObject {

    private Color shieldColor = new Color(0.0f, 0.0f, 0.5f, 0.5f);
    private Color normalColor;

    private bool invisible;
    private static int totalInvisiblePoints = 2;
    private int currInvisiblePoints;
    public bool Inivisible { get { return invisible;} }

    private SpriteRenderer sprRender;

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        invisible = false;
        sprRender = GetComponent<SpriteRenderer>();
        type = TileManager.ContentType.Human;
        normalColor = sprRender.color;
        
        possibleSpellTypes.Add(GameManager.SpellType.Invisible);
        possibleSpellTypes.Add(GameManager.SpellType.Push);
    }

    public override void Clear()
    {
        GameManager.Instance.LevelManager.RemoveHuman(this);
    }

    public void MakeInvisible()
    {
        GetComponent<SpriteRenderer>().color = shieldColor;
        invisible = true;
        currInvisiblePoints = totalInvisiblePoints;

        GameManager.Instance.TileManager.SwitchStateTile(type, gridPosition);
        type = TileManager.ContentType.InivisbleHuman;
        gameObject.layer = 0;
    }

    public void DecreaseInvisiblePoints()
    {
        if (invisible)
        {
            currInvisiblePoints--;

            //TODO: shield color change

            if (currInvisiblePoints <= 0)
            {
                sprRender.color = normalColor;
                invisible = false;
                GameManager.Instance.TileManager.SwitchStateTile(type, gridPosition);
                type = TileManager.ContentType.Human;
                gameObject.layer = 8;
            }
        }
    }

    public override bool Hit()
    {
        canBeTargeted = false;

        GameManager.Instance.LevelManager.RemoveHuman(this, true);

        return true;
    }
}
