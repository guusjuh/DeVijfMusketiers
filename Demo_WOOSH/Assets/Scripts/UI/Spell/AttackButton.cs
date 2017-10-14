﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : SpellButton
{

    public override void Initialize()
    {
        base.Initialize();
        cost = 2;
        type = GameManager.SpellType.Attack;
        spellDamage = 10;
        SpawnAP(cost);
    }

    public override void CastSpell()
    {
        target.GetComponent<Enemy>().Hit(spellDamage);
        base.CastSpell();
    }
}
