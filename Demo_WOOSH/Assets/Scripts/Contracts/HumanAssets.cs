using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HumanAssets {
    [SerializeField] private GameObject inWorld;
    [SerializeField] private GameObject inWorldAngry;
    public GameObject InWorld(int hapiness)
    {
        return inWorld;
    }

    [SerializeField] private Sprite inWorldSprite;
    [SerializeField] private Sprite inWorldSpriteAngry;
    public Sprite InWorldSprite(int hapiness)
    {
        return inWorldSprite;
    }

    [SerializeField] private Sprite portrait;
    [SerializeField] private Sprite portraitAngry;
    public Sprite Portrait(int hapiness)
    {
        return portrait;
    }
}
