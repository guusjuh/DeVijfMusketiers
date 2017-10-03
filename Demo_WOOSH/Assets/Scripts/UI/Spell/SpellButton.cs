using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    protected Button button;
    protected WorldObject target;
    protected int cost;

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
    }

    public virtual void CastSpell()
    {
        GameManager.Instance.LevelManager.EndPlayerMove(cost);
        GameManager.Instance.UiManager.HideSpellButtons();
    }

    public virtual void Activate(WorldObject target)
    {
        this.target = target;
        if (cost > GameManager.Instance.LevelManager.Player.CurrentActionPoints) Active = false;
        else Active = true;
    }
}