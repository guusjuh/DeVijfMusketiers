using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellProxy {
    //descriptional variables
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite spellImage;
    [SerializeField] private GameManager.SpellType type;
    [SerializeField] private Color spellColor;
    [SerializeField] private int cost;

    //casting variables
    [SerializeField] private int damage;
    [SerializeField] private float hitChance;

    [SerializeField] private int fireDamage;
    [SerializeField] private int fireTurns;
    [SerializeField] private float fireChance;

    [SerializeField] private int freezeTurns;
    [SerializeField] private float freezeChance;

    [SerializeField] private bool isDirect;
    [SerializeField] private int range;

    //descriptional getters
    public string Name() { return name; }
    public string Description() { return description; }
    public Sprite SpellSprite() { return spellImage; }
    public GameManager.SpellType Type() { return type; }
    public Color SpellColor() { return spellColor; }
    public int Cost() { return cost; }

    //casting getters
    public int Damage() { return damage; }
    public float HitChance() { return hitChance; }

    public int FireDamage() { return fireDamage; }
    public int FireTurns() { return fireTurns; }
    public float FireChance() { return fireChance; }


    public int FreezeTurns() { return freezeTurns; }
    public float FreezeChance() { return freezeChance; }

    public bool IsDirect() { return isDirect; }
    public int Range() { return range; }
}
