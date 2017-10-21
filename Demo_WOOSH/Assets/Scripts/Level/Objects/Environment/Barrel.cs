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

            sprRender.sprite = destroyed ? destoryedSpr : normalSpr;
            if (destroyed) gameObject.layer = 0;
            else gameObject.layer = 8;
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        sprRender = GetComponent<SpriteRenderer>();
        type = SecContentType.Barrel;

        normalSpr = sprRender.sprite;
        destoryedSpr = Resources.Load<Sprite>("Sprites/World/brokenbarrel");
    }

    public override void Clear()
    {
        GameManager.Instance.LevelManager.RemoveObject(this);
    }

    public override bool Hit()
    {
        Destroyed = true;
        return true;
    }

    public override bool IsBarrel() { return true; }
}
