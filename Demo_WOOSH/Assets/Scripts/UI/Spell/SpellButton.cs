using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    protected Button button;
    protected WorldObject target;
    protected int cost;

    protected GameObject apIndicator;
    protected GameObject disabledObject;
    protected Text cooldownText;
    protected GameManager.SpellType type;

    protected bool active = true;
    public bool Active
    {
        get { return active; }
        set
        {
            if (active == value) return;
            active = value;
            button.interactable = active;
        }
    }

    public virtual void Initialize()
    {
        button = GetComponent<Button>();

        disabledObject = transform.Find("Disabled").gameObject;
        cooldownText = transform.Find("Text").GetComponent<Text>();
        apIndicator = transform.Find("APIndicator").gameObject;
    }

    public virtual void CastSpell()
    {
        GameManager.Instance.LevelManager.EndPlayerMove(cost);
        GameManager.Instance.UiManager.HideSpellButtons();
    }

    public virtual void Activate(WorldObject target)
    {
        this.target = target;
        // cant do spell due to cooldown
        if (GameManager.Instance.LevelManager.Player.GetCurrentCooldown(type) > 0)
        {
            Active = false;
            SetCooldownText(GameManager.Instance.LevelManager.Player.GetCurrentCooldown(type));
        }
        // cant do spell due to action points
        else if (cost > GameManager.Instance.LevelManager.Player.CurrentActionPoints)
        {
            Active = false;
        }
        // can do spell
        else
        {
            Active = true;
            SetCooldownText(0);
        }
    }

    public void SetCooldownText(int value)
    {
        if (value <= 0)
        {
            if (disabledObject.activeInHierarchy) disabledObject.SetActive(false);
            if (cooldownText.gameObject.activeInHierarchy) cooldownText.gameObject.SetActive(false);
            if (!apIndicator.gameObject.activeInHierarchy) apIndicator.gameObject.SetActive(true);
        }
        else
        {
            if (!disabledObject.activeInHierarchy) disabledObject.SetActive(true);
            if (!cooldownText.gameObject.activeInHierarchy) cooldownText.gameObject.SetActive(true);
            if (apIndicator.gameObject.activeInHierarchy) apIndicator.gameObject.SetActive(false);
            cooldownText.text = "" + value;
        }
    }
}