using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arnest : Enemy
{
    public GameObject heal;
    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        startHealth = 100;
        hasSpecial = false;
        viewDistance = 3;

        this.totalSpecialCooldown = 2;
        type = SecContentType.Arnest;
        this.specialCost = 1;
        this.hasSpecial = true;

        this.SpellIconSprite = Resources.Load<Sprite>("Sprites/UI/Spells/enemyHeal");

        heal.SetActive(false);

        base.Initialize(startPos);
    }

    public override void Reset()
    {
        totalActionPoints = 3;
        base.Reset();
    }

    protected override void Attack(EnemyTarget other)
    {
        base.Attack(other);
    }

    public override bool CheckForSpell()
    {
        // target reached
        bool enoughAP = currentActionPoints >= specialCost;
        bool onCooldown = specialCooldown > 0;

        if (enoughAP && !onCooldown && Health < startHealth)
        {
            currentActionPoints -= specialCost;
            specialCooldown = totalSpecialCooldown;
            Heal(20);//heal a certain amount
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);

            StartCoroutine(HealVisual());

            return true;
        }
        return false;
    }

    public override bool IsWalking()
    {
        return true;
    }

    protected IEnumerator HealVisual()
    {
        heal.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        heal.SetActive(false);

        yield break;
    }
}