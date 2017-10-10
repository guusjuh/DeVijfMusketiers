using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    protected Button button;
    protected WorldObject target;
    protected int cost;

    protected List<GameObject> apIndicator;
    protected GameObject disabledObject;
    protected Text cooldownText;
    protected GameManager.SpellType type;

    protected bool active = true;

    private const float RADIUS = 90f;

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
        apIndicator = new List<GameObject>();
        //apIndicator = transform.Find("APIndicator").gameObject;
    }

    public virtual void CastSpell()
    {
        GameManager.Instance.LevelManager.EndPlayerMove(cost);
        UIManager.Instance.InGameUI.HideSpellButtons();
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
            foreach (var indicator in apIndicator)
            {
                if (!indicator.gameObject.activeInHierarchy) indicator.gameObject.SetActive(true);
            }
        }
        else
        {
            if (!disabledObject.activeInHierarchy) disabledObject.SetActive(true);
            if (!cooldownText.gameObject.activeInHierarchy) cooldownText.gameObject.SetActive(true);
            foreach (var indicator in apIndicator)
            {
                if (indicator.gameObject.activeInHierarchy) indicator.gameObject.SetActive(false);
            }
            cooldownText.text = "" + value;
        }
    }

    public void SpawnAP(int amount)
    {
        Vector2 canvasPos = new Vector2(0, 0);

        float divider = amount > 1 ? (float)amount - 1.0f : (float)amount;
        float partialCircle = (amount - 1) / 4.0f * 0.4f;
        float offSetCircle = (1.0f - partialCircle) / 2.0f;

        for (int i = 0; i < amount; i++)
        {

            Vector2 pos = new Vector2(RADIUS * Mathf.Cos(partialCircle * Mathf.PI * (float)i / divider + offSetCircle * Mathf.PI),
                -RADIUS * Mathf.Sin(partialCircle * Mathf.PI * (float)i / divider + offSetCircle * Mathf.PI)); 

            GameObject APPoint = Resources.Load<GameObject>("Prefabs/UI/SpellButton/APIndicator");
            apIndicator.Add(Instantiate(APPoint, -pos, Quaternion.identity, this.transform));
        }
    }
}