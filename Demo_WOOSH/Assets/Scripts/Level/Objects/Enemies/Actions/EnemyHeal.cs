﻿using System.Collections;
using UnityEngine;

public class EnemyHeal : Action
{
    private GameObject heal;

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        totalCooldown = 2;
        cost = 1;

        spellIconSprite = Resources.Load<Sprite>("Sprites/UI/InGame/Spells/enemyHeal");


        heal = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actions/Heal"), parent.transform);
        heal.transform.localPosition = new Vector3(0, 0.5f, 0);
        heal.SetActive(false);
        HasSpellIcon = true;
    }

    public override void Reset()
    {
        base.Reset();
        heal.SetActive(false);
    }

    public override bool DoAction()
    {
        bool enoughAP = parent.CurrentActionPoints >= cost;
        bool onCooldown = currentCooldown > 0;

        if (enoughAP && !onCooldown && parent.Health < parent.StartHealth)
        {
            parent.EndMove(cost);
            currentCooldown = totalCooldown;
            parent.Heal(20);//heal a certain amount
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(parent);

            UberManager.Instance.StartCoroutine(HealVisual());

            return true;
        }
        return false;
    }

    private IEnumerator HealVisual()
    {
        heal.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        heal.SetActive(false);

        yield break;
    }
}
