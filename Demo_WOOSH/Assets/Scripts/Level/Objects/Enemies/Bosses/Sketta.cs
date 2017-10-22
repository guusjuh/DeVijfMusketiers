using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketta : Enemy {
    public GameObject Shield;
    private float shieldActiveTime;

    public override void Initialize(Coordinate startPos)
    {
        this.startHealth = 130;
        blockChance = 0.3f;
        canBlock = true;

        Shield.SetActive(false);

        totalActionPoints++;
        this.hasSpecial = false;
        viewDistance = 3;

        this.type = SecContentType.Sketta;

        base.Initialize(startPos);
    }

    public override bool TryHit(int dmg)
    {
        if (!base.TryHit(dmg))
        {
            StartCoroutine(ShieldVisual());
            return false;
        }

        return true;
    }

    protected IEnumerator ShieldVisual()
    {
        Shield.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Shield.SetActive(false);

        yield break;
    }

    public override bool IsWalking() { return true; }
}
