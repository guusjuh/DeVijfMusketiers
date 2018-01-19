using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    //TODO: move that shit to spell class
    protected float hitchance = 1.0f; //normalized value, 1.0f = 100%
    protected int spellDamage = 0;
    protected int fireDamage = 0;
    protected int duration = 0;//how long does the effect of the spell last

    protected Button button;
    protected WorldObject target;
    protected int cost;

    protected List<GameObject> apIndicator;
    protected GameObject disabledObject;
    protected Text cooldownText;
    protected GameManager.SpellType type;

    protected bool active = true;

    protected const float RADIUS = 90f;

    public bool Active
    {
        get { return active; }
        set
        {
            if (active == value) return;
            active = value;
            disabledObject.SetActive(!value);
            button.interactable = active;
        }
    }

    public virtual void Initialize()
    {
        button = GetComponent<Button>();

        disabledObject = transform.Find("Disabled").gameObject;
        cooldownText = transform.Find("Text").GetComponent<Text>();
        apIndicator = new List<GameObject>();
    }

    public void Click()
    {
        StartCoroutine(CastSpell());

        UIManager.Instance.InGameUI.CastingSpell = (int)type;
    }

    public virtual IEnumerator CastSpell()
    {
        UberManager.Instance.SoundManager.PlaySoundEffect(type);

        yield return StartCoroutine(UIManager.Instance.InGameUI.CastSpell(type, GameManager.Instance.TileManager.GetWorldPosition(target.GridPosition)));

        if (UberManager.Instance.Tutorial)
        {
            UberManager.Instance.TutorialManager.Next();
            yield break;
        }

        GameManager.Instance.LevelManager.Player.SetCooldown(type);
        GameManager.Instance.LevelManager.EndPlayerMove(cost);

        ApplyEffect();

        UIManager.Instance.InGameUI.CastingSpell = -1;

        UIManager.Instance.InGameUI.HideSpellButtons();

        UberManager.Instance.ParticleManager.PlaySpellParticle(type, target.transform.position, target.transform.rotation);

        yield return null;
    }

    public virtual void ApplyEffect()
    {
        
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
            if (cooldownText.gameObject.activeInHierarchy) cooldownText.gameObject.SetActive(false);
            foreach (var indicator in apIndicator)
            {
                if (!indicator.gameObject.activeInHierarchy) indicator.gameObject.SetActive(true);
            }
        }
        else
        {
            if (!cooldownText.gameObject.activeInHierarchy) cooldownText.gameObject.SetActive(true);
            foreach (var indicator in apIndicator)
            {
                if (indicator.gameObject.activeInHierarchy) indicator.gameObject.SetActive(false);
            }
            cooldownText.text = "" + value;
        }
    }

    protected virtual void SpawnAP()
    {
        int amount = cost;

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
 