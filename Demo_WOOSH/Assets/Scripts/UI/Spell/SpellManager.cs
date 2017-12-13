using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellManager {
    [SerializeField] private List<SpellProxy> spellProxies;
    private SpellFactory factory = new SpellFactory();
    private List<ISpell> spells = new List<ISpell>();

    private Dictionary<GameManager.SpellType, SpellButton> spellButtons = new Dictionary<GameManager.SpellType, SpellButton>();

    private WorldObject selectedTarget = null;
    private bool spellButtonsActive = false;
    private SpellVisual spellVisual;
    public Dictionary<GameManager.SpellType, Color> SpellColors = new Dictionary<GameManager.SpellType, Color>();
    private GameManager.SpellType castingSpell = GameManager.SpellType.NoSpell;

    public GameManager.SpellType CastingSpell { get { return castingSpell; } set { Debug.Log("Cassting spell: " + value); castingSpell = value; } }
    public bool SpellButtonsActive { get { return spellButtonsActive; } }
    public WorldObject SelectedTarget { get { return selectedTarget; } }

    public void Initialize()
    {
        if (spellProxies.Count <= 0)
        {
            Debug.LogWarning("No spells have been found!");
            return;
        }

        for (int i = 0; i < spellProxies.Count; i++)
        {
            spells.Add(factory.CreateSpell(spellProxies[i]));
            SpellColors.Add(spellProxies[i].Type(), spellProxies[i].SpellColor());
            CreateSpellButton(spells.Last(), spellProxies[i]);
        }

        //TODO: improve
        Canvas canvas = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<Canvas>();
        RectTransform anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        //--

        spellVisual = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellVisual/SpellInGame", Vector2.zero, anchorCenter).GetComponent<SpellVisual>();
        spellVisual.Initialize();
    }

    public void CreateSpellButton(ISpell spell, SpellProxy proxy)
    {
        //TODO: improve
        Canvas canvas = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<Canvas>();
        RectTransform anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        //--

        spellButtons.Add(proxy.Type(), UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellButton/SpellButton", Vector2.zero, anchorCenter).GetComponent<SpellButton>());
        spellButtons.Get(proxy.Type()).Initialize(spell, proxy.Type(), proxy.Description(), proxy.SpellSprite(), proxy.Cost());
        spellButtons.Get(proxy.Type()).gameObject.SetActive(false);
    }

    public void SelectTarget(WorldObject target)
    {
        selectedTarget = target;
        UpdateButtonPositions();
    }

    public void ShowSpellButtons()
    {
        HideSpellButtons();
        spellButtonsActive = true;

        for (int i = 0; i < selectedTarget.PossibleSpellTypes.Count; i++)
        {
            spellButtons.Get(selectedTarget.PossibleSpellTypes[i]).gameObject.SetActive(true);
            spellButtons.Get(selectedTarget.PossibleSpellTypes[i]).Activate(selectedTarget);
        }
    }

    public void UpdateButtonPositions()
    {
        foreach (var pair in spellButtons)
        {
            pair.Value.SetButtonPosition(selectedTarget);
        }
    }

    public void HideSpellButtons()
    {
        foreach (var pair in spellButtons)
        {
            pair.Value.gameObject.SetActive(false);
        }
        spellButtonsActive = false;
    }

    public IEnumerator ShowSpellVisual(GameManager.SpellType type)
    {
        if (selectedTarget != null)
        {
            UIManager.Instance.InGameUI.SpellIsCast();

            yield return UberManager.Instance.StartCoroutine(spellVisual.Activate(type, UberManager.Instance.GameManager.TileManager.GetWorldPosition(SelectedTarget.GridPosition)));

            spellVisual.gameObject.SetActive(false);         
            spellButtons[type].CastSpell(selectedTarget);
            HideSpellButtons();
        }
    }
}
