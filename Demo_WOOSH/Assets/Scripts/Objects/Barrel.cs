using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MovableObject
{
    private Sprite destoryedSpr;
    private Sprite normalSpr;

    private SpriteRenderer sprRender;

    public bool destroyed = false;
    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            destroyed = value;
            canBeTargeted = !destroyed;
            type = destroyed ? TileManager.ContentType.BrokenBarrel : TileManager.ContentType.Barrel;

            GetComponent<SpriteRenderer>().sprite = destroyed ? destoryedSpr : normalSpr;
            GameManager.Instance.TileManager.SwitchStateTile(type, gridPosition);
            if (destroyed) gameObject.layer = 0;
            else gameObject.layer = 8;
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        //TODO: obtain sprites

        sprRender = GetComponent<SpriteRenderer>();
        type = TileManager.ContentType.Barrel;
    }

    public override bool Hit()
    {
        Destroyed = true;
        GameManager.Instance.LevelManager.RemoveBarrel(this);
        return true;
    }
}
