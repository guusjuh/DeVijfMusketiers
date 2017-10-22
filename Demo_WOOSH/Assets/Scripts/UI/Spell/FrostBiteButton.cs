﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrostBiteButton : SpellButton
{
    public override void Initialize()
    {
        base.Initialize();
        cost = 1;
        hitchance = 0.3f;
        spellDamage = 5;
        duration = 2;
        type = GameManager.SpellType.FrostBite;
        SpawnAP();
    }

    public override void ApplyEffect()
    {
        float chance = Random.Range(0.0f, 1.0f);
        if (chance < hitchance)
        {
            target.GetComponent<Enemy>().TryHit(spellDamage);
        }
        target.GetComponent<Enemy>().Slow(duration);
    }
}