using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : SpellButton
{

    public override void Initialize()
    {
        base.Initialize();
        cost = 4;
        type = GameManager.SpellType.Teleport;
        SpawnAP(cost);
    }

    public override void CastSpell()
    {
        target.GetComponent<Enemy>().Hit(spellDamage);
        base.CastSpell();
    }
}
