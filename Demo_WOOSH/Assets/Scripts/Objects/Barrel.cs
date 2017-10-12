using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MovableObject
{
    private Sprite destoryedSpr;
    private Sprite normalSpr;

    private SpriteRenderer sprRender;

    private bool destroyed = false;
    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            destroyed = value;
            canBeTargeted = !destroyed;
            GameManager.Instance.TileManager.SwitchStateTile(type, gridPosition);
            type = destroyed ? TileManager.ContentType.BrokenBarrel : TileManager.ContentType.Barrel;

            GetComponent<SpriteRenderer>().sprite = destroyed ? destoryedSpr : normalSpr;
            if (destroyed) gameObject.layer = 0;
            else gameObject.layer = 8;
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        sprRender = GetComponent<SpriteRenderer>();
        type = TileManager.ContentType.Barrel;

        normalSpr = sprRender.sprite;
        destoryedSpr = Resources.Load<Sprite>("Sprites/World/brokenbarrel");
    }

    public override void Clear()
    {
        GameManager.Instance.LevelManager.RemoveBarrel(this);
    }

    public override bool Hit()
    {
        Destroyed = true;
        //GameManager.Instance.LevelManager.RemoveBarrel(this);
        return true;
    }

    public void RemoveByGap()
    {
        Destroyed = true;
        GameManager.Instance.LevelManager.RemoveBarrel(this);
    }
}
