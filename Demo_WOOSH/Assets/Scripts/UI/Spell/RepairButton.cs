﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairButton : SpellButton
{
    public override void Initialize()
    {
        base.Initialize();
        cost = 1;
    }

    public override void CastSpell()
    {
        GameManager.Instance.LevelManager.Player.SetRepairCooldown();
        target.GetComponent<Barrel>().Destroyed = false;

        base.CastSpell();
    }

    public override void Activate(WorldObject target)
    {
        base.Activate(target);

        if (!target.GetComponent<Barrel>().Destroyed || GameManager.Instance.LevelManager.Player.RepairCooldown > 0) Active = false;
    }
}
