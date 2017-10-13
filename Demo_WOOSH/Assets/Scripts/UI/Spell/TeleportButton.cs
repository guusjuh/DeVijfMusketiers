using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : SpellButton
{

    public override void Initialize()
    {
        base.Initialize();
        cost = 0;
        type = GameManager.SpellType.Teleport;
        SpawnAP(4);
    }

    public override void CastSpell()
    {
        target.GetComponent<Human>().ActivateTeleportButtons();
        base.CastSpell();
    }
}
