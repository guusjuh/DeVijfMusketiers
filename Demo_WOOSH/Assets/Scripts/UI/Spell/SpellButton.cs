using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour {
    private Sprite SpellSprite { set { spellImg.sprite = value; } }
    private string description;
    private ISpell spell;
    private SpellManager.SpellType type;

    private Button btn;
    private Text cooldownText;
    private Image spellImg;
    private List<GameObject> apIndicator;

    private Color activeColor { get { return Color.white; } }
    private Color dissabledColor { get { return new Color(0.5f, 0.5f, 0.5f, 1.0f); } }

    private const float RADIUS = 90f;
    private int cooldown;
    public int Cooldown { get { return cooldown; } }

    public void Initialize(ISpell spell, SpellManager.SpellType type, string description, Sprite sprite, int cooldown)
    {
        btn = GetComponent<Button>();
        cooldownText = GetComponentInChildren<Text>();
        cooldownText.gameObject.SetActive(false);

        apIndicator = new List<GameObject>();
        spellImg = transform.Find("Image").GetComponent<Image>();

        this.spell = spell;
        this.type = type;
        this.description = description;
        SpellSprite = sprite;
        this.cooldown = cooldown;

        SpawnAP();
    }

    public void Activate(WorldObject target)
    {
        //setTarget?
        bool cooldownFinished = GameManager.Instance.LevelManager.Player.GetCurrentCooldown(type) <= 0;
        bool enoughActionPoints = spell.Cost() <= GameManager.Instance.LevelManager.Player.CurrentActionPoints;
        SetButtonPosition(target);

        if (!cooldownFinished)
        {
            SetCooldownText(GameManager.Instance.LevelManager.Player.GetCurrentCooldown(type));
        }

        if (!enoughActionPoints || !cooldownFinished)
        {
            SetInteractable(false);
        }

        if (enoughActionPoints && cooldownFinished)
        {
            SetCooldownText(0);
            SetInteractable(true);
        }
    }

    public void SetInteractable(bool isActive)
    {
        btn.interactable = isActive;

        if (isActive)
        {
            spellImg.color = activeColor;
        }
        else
        {
            spellImg.color = dissabledColor;
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

    public void OnClick()
    {
        UberManager.Instance.SpellManager.CastingSpell = type;

        StartCoroutine(UberManager.Instance.SpellManager.ShowSpellVisual(type));
    }

    public void CastSpell(WorldObject target)
    {
        spell.CastSpell(target);
        if (spell.IsDirect())
        {
            UberManager.Instance.SpellManager.CastingSpell = SpellManager.SpellType.NoSpell;

            GameManager.Instance.LevelManager.Player.SetCooldown(type, cooldown);
            SetCooldownText(GameManager.Instance.LevelManager.Player.GetCurrentCooldown(type));
            SetInteractable(cooldown <= 0);
        }
        else
            UberManager.Instance.SpellManager.SetActiveIndirect(spell);
    }

    private void SpawnAP()
    {
        int amount = spell.Cost();

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

    public void SetButtonPosition(WorldObject target)
    {
        Vector2 canvasPos = UIManager.Instance.InGameUI.WorldToCanvas(target.transform.position);

        float divider = target.PossibleSpellTypes.Count > 1 ? (float)target.PossibleSpellTypes.Count - 1.0f : (float)target.PossibleSpellTypes.Count;
        float partialCircle = (target.PossibleSpellTypes.Count - 1) / 4.0f * 0.9f;
        float offSetCircle = (1.0f - partialCircle) / 2.0f;

        for (int i = 0; i < target.PossibleSpellTypes.Count; i++)
        {
            if (target.PossibleSpellTypes[i] == type)
            {
                //200.0f is worldObject radius, used for spawning spellbuttons
                GetComponent<RectTransform>().anchoredPosition = canvasPos - CalculatePointOnCircle(200.0f, partialCircle, divider, offSetCircle, i);
            }
        }
    }

    public static Vector2 CalculatePointOnCircle(float radius, float partialCircle, float divider, float offset, int index)
    {
        return new Vector2(radius * Mathf.Cos(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI),
                -radius * Mathf.Sin(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI));
    }
}
