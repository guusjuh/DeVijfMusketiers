using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : SpellButton
{
    public override void Initialize()
    {
        base.Initialize();
        cost = 0;
        type = GameManager.SpellType.Push;

    }

    public override void CastSpell()
    {
        UIManager.Instance.InGameUI.ActivatePushButtons(true, (MovableObject)target);

        base.CastSpell();
    }

    public override void Activate(WorldObject target)
    {
        base.Activate(target);

        if (!target.GetComponent<MovableObject>().CanBePushed()) Active = false;
    }
}
