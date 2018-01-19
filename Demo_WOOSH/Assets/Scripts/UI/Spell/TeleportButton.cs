﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : SpellButton
{
    private int secondCost = 3;

    public override void Initialize()
    {
        base.Initialize();
        cost = 0;
        type = GameManager.SpellType.Teleport;
        SpawnAP();
    }

    public override IEnumerator CastSpell()
    {
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        UberManager.Instance.ParticleManager.PlaySpellParticle(type, target.transform.position, target.transform.rotation);

        yield return StartCoroutine(UIManager.Instance.InGameUI.CastSpell(type,
                GameManager.Instance.TileManager.GetWorldPosition(target.GridPosition)));

        target.GetComponent<Human>().ActivateTeleportButtons();

        GameManager.Instance.LevelManager.EndPlayerMove(cost);
        UIManager.Instance.InGameUI.HideSpellButtons();

        if(UberManager.Instance.Tutorial) UberManager.Instance.TutorialManager.Next();

        yield return null;
    }

    public override void Activate(WorldObject target)
    {
        base.Activate(target);

        if (secondCost > GameManager.Instance.LevelManager.Player.CurrentActionPoints)
        {
            Active = false;
        }
    }

    protected override void SpawnAP()
    {
        int amount = secondCost;

        float divider = amount > 1 ? (float)amount - 1.0f : (float)amount;
        float partialCircle = (amount - 1) / 4.0f * 0.4f;
        float offSetCircle = (1.0f - partialCircle) / 2.0f;

        for (int i = 0; i < amount; i++)
        {
            Vector2 pos = new Vector2(RADIUS * Mathf.Cos(partialCircle * Mathf.PI * (float)i / divider + offSetCircle * Mathf.PI),
                -RADIUS * Mathf.Sin(partialCircle * Mathf.PI * (float)i / divider + offSetCircle * Mathf.PI));

            GameObject APPoint = Resources.Load<GameObject>("Prefabs/UI/InGame/SpellButton/APIndicator");
            apIndicator.Add(Instantiate(APPoint, -pos, Quaternion.identity, this.transform));
            apIndicator[i].GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition - new Vector2(pos.x, pos.y);
        }
    }
}
