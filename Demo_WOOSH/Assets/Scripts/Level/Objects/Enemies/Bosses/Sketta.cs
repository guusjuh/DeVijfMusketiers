﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketta : Enemy {
    public GameObject Shield;

    public override void Initialize(Coordinate startPos)
    {
        startHealth = 130;
        blockChance = 0.3f;
        canBlock = true;

        Shield.SetActive(false);

        hasSpecial = false;
        viewDistance = 3;

        type = SecContentType.Sketta;

        base.Initialize(startPos);
    }

    public override void Reset()
    {
        base.Reset();
        Shield.SetActive(false);
    }

    public override bool TryHit(int dmg)
    {
        if (!base.TryHit(dmg))
        {
            StartCoroutine(ShieldVisual());
            canBlock = false;
            return false;
        }
        else
        {
            canBlock = true;
        }

        return true;
    }

    protected IEnumerator ShieldVisual()
    {
        Shield.SetActive(true);
        Shield.GetComponent<ParticleSystem>().startColor = UberManager.Instance.UiManager.InGameUI.SpellColors[(GameManager.SpellType)UberManager.Instance.UiManager.InGameUI.CastingSpell];

        yield return new WaitForSeconds(0.5f);

        Shield.SetActive(false);

        yield break;
    }

    public override bool IsWalking() { return true; }
}
