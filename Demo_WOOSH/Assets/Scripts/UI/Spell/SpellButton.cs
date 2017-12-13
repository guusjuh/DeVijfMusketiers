using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour {
    private Sprite SpellSprite { set { SpellImg.sprite = value; } }
    private string description;
    private ISpell spell;
    private GameManager.SpellType type;

    private Button btn;
    private Text cooldownText;
    private Image background;
    private Image SpellImg;
    private List<GameObject> apIndicator;

    private const float RADIUS = 90f;
    private int cost;

    public void Initialize(ISpell spell, GameManager.SpellType type, string description, Sprite sprite, int cost)
    {
        btn = GetComponent<Button>();
        background = transform.Find("Background").GetComponent<Image>();
        cooldownText = GetComponentInChildren<Text>();
        cooldownText.gameObject.SetActive(false);
        apIndicator = new List<GameObject>();
        SpellImg = transform.Find("SpellImage").GetComponent<Image>();

        this.spell = spell;
        this.type = type;
        this.description = description;
        SpellSprite = sprite;
        this.cost = cost;

        SpawnAP();
    }

    public void Activate(WorldObject target)
    {
        //setTarget?
        bool cooldownFinished = GameManager.Instance.LevelManager.Player.GetCurrentCooldown(type) <= 0;
        bool enoughActionPoints = cost <= GameManager.Instance.LevelManager.Player.CurrentActionPoints;
        SetButtonPosition(target);
        if (!cooldownFinished)
        {
            SetCooldownText(GameManager.Instance.LevelManager.Player.GetCurrentCooldown(type));
        }

        if (!enoughActionPoints || !cooldownFinished)
        {
            SetInteractable(false, target);
        }

        if (enoughActionPoints && cooldownFinished)
        {
            SetCooldownText(0);
            SetInteractable(true, target);
        }
    }

    private void SetInteractable(bool isActive, WorldObject target)
    {
        btn.interactable = isActive;

        if (isActive)
        {
            background.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            SpellImg.color = Color.white;
        }
        else
        {
            background.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            SpellImg.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
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
            UberManager.Instance.SpellManager.CastingSpell = GameManager.SpellType.NoSpell;
    }

    private void SpawnAP()
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
