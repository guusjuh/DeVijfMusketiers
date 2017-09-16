using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class Vase : Damagable {
    public Sprite destoryedSpr;
    public Sprite normalSpr;

    public bool destroyed = false;
    public bool Destroyed {
        get { return destroyed; }
        set
        {
            destroyed = value;
            GetComponent<SpriteRenderer>().sprite = destroyed ? destoryedSpr : normalSpr;

            if (destroyed) gameObject.layer = 0;
            else gameObject.layer = 8;
        }
    }

    public override bool Hit()
    {
        Destroyed = true;
        return true;
    }
}
