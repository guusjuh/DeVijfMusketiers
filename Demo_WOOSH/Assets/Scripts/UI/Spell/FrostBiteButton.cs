using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrostBiteButton : SpellButton
{
    public override void Initialize()
    {
        //TODO: cooldown = 3
        base.Initialize();
        cost = 2;
        hitchance = 0.3f;
        spellDamage = 5;
        duration = 2;
        type = GameManager.SpellType.Attack;
        SpawnAP(cost);
    }

    public override void CastSpell()
    {
        float chance = Random.Range(0.0f, 1.0f);
        if (chance < hitchance)
        {
            target.GetComponent<Enemy>().Hit(spellDamage);
        }
        target.GetComponent<Enemy>().Slow(duration);
        
        base.CastSpell();
    }
}