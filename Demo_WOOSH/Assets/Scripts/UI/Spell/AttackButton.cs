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
    }

    public override void CastSpell()
    {
        target.GetComponent<Enemy>().Hit(GameManager.Instance.LevelManager.Player.Damage);

        base.CastSpell();
    }
}
