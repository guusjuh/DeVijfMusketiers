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
        return (hapiness > 10) ? inWorld : inWorldAngry;
    }

    [SerializeField] private Sprite inWorldSprite;
    [SerializeField] private Sprite inWorldSpriteAngry;
    public Sprite InWorldSprite(int hapiness)
    {
        return (hapiness > 10) ? inWorldSprite : inWorldSpriteAngry;
    }

    [SerializeField] private Sprite portrait;
    [SerializeField] private Sprite portraitAngry;
    public Sprite Portrait(int hapiness)
    {
        return (hapiness > 10) ? portrait : portraitAngry;
    }
}
