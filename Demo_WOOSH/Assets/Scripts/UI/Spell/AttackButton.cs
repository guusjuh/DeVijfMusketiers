using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : SpellButton
{
    public override void Initialize()
    {
        base.Initialize();
        cost = 1;
        type = GameManager.SpellType.Attack;
        spellDamage = 1000;
        SpawnAP();
    }

    public override void ApplyEffect()
    {
        target.GetComponent<Enemy>().TryHit(spellDamage);
    }
}
