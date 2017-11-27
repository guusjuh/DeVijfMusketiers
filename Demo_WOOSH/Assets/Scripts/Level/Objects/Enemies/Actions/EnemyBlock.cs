using System.Collections;
using UnityEngine;

public class EnemyBlock : Action
{
    private GameObject shield;

    private float blockChance;
    private bool canBlock;

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        blockChance = 0.3f;
        canBlock = true;

        shield = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actions/Shield"), parent.transform);
        shield.SetActive(false);
    }

    public override void Reset()
    {
        base.Reset();
        shield.SetActive(false);
    }

    public override bool TryHit()
    {
        float roll = Random.Range(0.0f, 1.0f);
        if (canBlock && roll < blockChance)
        {
            UberManager.Instance.StartCoroutine(ShieldVisual());
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
        shield.SetActive(true);
        shield.GetComponent<ParticleSystem>().startColor = UberManager.Instance.UiManager.InGameUI.SpellColors[(GameManager.SpellType)UberManager.Instance.UiManager.InGameUI.CastingSpell];

        yield return new WaitForSeconds(0.5f);

        shield.SetActive(false);

        yield break;
    }
}
