using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HumanAssets {
    [SerializeField] private GameObject inWorld;
    public GameObject InWorld { get { return inWorld; } }

    [SerializeField] private Sprite inWorldSprite;
    public Sprite InWorldSprite { get { return inWorldSprite; } }

    [SerializeField] private Sprite portrait;
    public Sprite Portrait { get { return portrait; } }
}
