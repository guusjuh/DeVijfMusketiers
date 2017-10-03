using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleButton : SpellButton
{
    public override void Initialize()
    {
        base.Initialize();
        cost = 2;
    }

    public override void CastSpell()
    {
        GameManager.Instance.LevelManager.Player.SetInvisibleCooldown();
        target.GetComponent<Human>().MakeInvisible();

        base.CastSpell();
    }

    public override void Activate(WorldObject target)
    {
        base.Activate(target);

        if (GameManager.Instance.LevelManager.Player.InvisibleCooldown > 0) Active = false;
    }
}
