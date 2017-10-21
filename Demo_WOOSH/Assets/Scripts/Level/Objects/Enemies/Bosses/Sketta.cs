using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketta : Enemy {
    private float blockChange = 0.2f;
    public GameObject Shield;
    private float shieldActiveTime;

    public override void Initialize(Coordinate startPos)
    {
        this.startHealth = 130;

        Shield.SetActive(false);

        totalActionPoints++;
        this.hasSpecial = false;
        viewDistance = 3;

        this.type = SecContentType.Sketta;

        base.Initialize(startPos);
    }

    public override bool Hit(int dmg)
    {
        float block = Random.Range(0.0f, 1.0f);
        if (block <= blockChange)
        {
            StartCoroutine(ShieldVisual());
            return false;
        }
        return base.Hit(dmg);
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
