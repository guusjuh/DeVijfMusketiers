using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellManager {
    public enum SpellType
    {
        NoSpell = -1,
        Attack = 0,
        FrostBite,
        Fireball,
        Teleport
    }

    public enum SpellTarget
    {
        None,
        Enemy,
        Human
    }

    [SerializeField] private List<SpellProxy> spellProxies;
    private SpellFactory factory = new SpellFactory();
    private List<ISpell> spells = new List<ISpell>();

    private Dictionary<SpellType, SpellButton> spellButtons = new Dictionary<SpellType, SpellButton>();
    private Dictionary<SpellTarget, List<SpellType>> possibleSpells = new Dictionary<SpellTarget, List<SpellType>>();

    private WorldObject selectedTarget = null;
    private bool spellButtonsActive = false;
    public Dictionary<SpellType, Color> SpellColors = new Dictionary<SpellType, Color>();
    private SpellType castingSpell = SpellType.NoSpell;
    private Coordinate selectedTile = new Coordinate(-1, -1);
    private ISpell activeInDirectSpell = null;

    public Coordinate SelectedTile { get {return selectedTile;} }
    public SpellType CastingSpell { get { return castingSpell; } set { castingSpell = value; } }
    public bool SpellButtonsActive { get { return spellButtonsActive; } }
    public WorldObject SelectedTarget { get { return selectedTarget; } }

    public void Initialize()
    {
        // No spells will be hard to play!
        if (spellProxies.Count <= 0)
        {
            Debug.LogWarning("No spells have been found!");
            return;
        }

        // Create spell for each spell proxy.
        for (int i = 0; i < spellProxies.Count; i++)
        {
            spells.Add(factory.CreateSpell(spellProxies[i]));
            SpellColors.Add(spellProxies[i].Type(), spellProxies[i].SpellColor());
            CreateSpellButton(spells.Last(), spellProxies[i]);

            for (int j = 0; j < spellProxies[i].PossibleTargets().Count; j++)
            {
                if (!possibleSpells.ContainsKey(spellProxies[i].PossibleTargets()[j]))
                    possibleSpells[spellProxies[i].PossibleTargets()[j]] = new List<SpellType>();

                possibleSpells[spellProxies[i].PossibleTargets()[j]].Add(spellProxies[i].Type());
            }
        }
    }

    public void CastInDirect()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Coordinate gridPos = UberManager.Instance.GameManager.TileManager.GetGridPosition(worldPos);

        if ((UberManager.Instance.GameManager.TileManager.GetWorldPosition(gridPos) - worldPos).magnitude <
            GameManager.Instance.TileManager.HexagonScale / 2.0f)
        {
            //teleport if tile has no content
            if (UberManager.Instance.GameManager.TileManager.GetNodeReference(gridPos).OpenForTeleport())
            {
                selectedTile = gridPos;
                //random is set to 1.0f, because the chance of hitting has already been calculated
                float rnd = UnityEngine.Random.Range(0.0f, 1.0f);
                activeInDirectSpell.Execute(selectedTarget, rnd, true);
                UberManager.Instance.GameManager.LevelManager.CheckForExtraAP();
                CastingSpell = SpellType.NoSpell;
            }
            else
            {
                CastingSpell = SpellType.NoSpell;
                UberManager.Instance.GameManager.TileManager.DisableHighlights();
                HideSpellButtons();
            }

        }
    }

    public bool CastingInDirect()
    {
        if (spells != null && spells.Count > 0)
        {
            ISpell spell = spells.Find(s => (s.Type() == castingSpell && s.Type() != SpellType.NoSpell));
            return (spell != null && !spell.IsDirect());
        }
        return false;
    }

    public void CreateSpellButton(ISpell spell, SpellProxy proxy)
    {
        //TODO: improve
        Canvas canvas = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<Canvas>();
        RectTransform anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        //--

        spellButtons.Add(proxy.Type(), UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellButton/SpellButton", Vector2.zero, anchorCenter).GetComponent<SpellButton>());
        spellButtons.Get(proxy.Type()).Initialize(spell, proxy.Type(), proxy.Description(), proxy.SpellSprite(), proxy.Cooldown());
        spellButtons.Get(proxy.Type()).gameObject.SetActive(false);


      /*  spellButtons.Add(
            proxy.Type(), 
            UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellButton/SpellButton", Vector2.zero, UberManager.Instance.UiManager.InGameUI.AnchorCenter).GetComponent<SpellButton>()
        );

        spellButtons.Get(proxy.Type())
            .Initialize(spell, proxy.Type(), proxy.Description(), proxy.SpellSprite(), proxy.Cooldown());

        spellButtons.Get(proxy.Type())
            .gameObject.SetActive(false);*/
    }

    public void SetActiveIndirect(ISpell spell)
    {
        activeInDirectSpell = spell;
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

    public List<SpellType> GetPossibleSpellTypes(SpellTarget target)
    {
        if (possibleSpells.ContainsKey(target))
            return possibleSpells[target];
        return null;
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

    public IEnumerator ShowSpellVisual(SpellType type)
    {
        if (selectedTarget != null)
        {
            UIManager.Instance.InGameUI.SpellIsCast();

            UberManager.Instance.ParticleManager.PlaySpellParticle(type, selectedTarget.transform.position, selectedTarget.transform.rotation);

            yield return new WaitForSeconds(0.8f);

            spellButtons[type].CastSpell(selectedTarget);
            HideSpellButtons();
        }

        yield break;
    }

    public void SetCooldown(SpellType type)
    {
        SpellButton bttn = spellButtons[type];

        GameManager.Instance.LevelManager.Player.SetCooldown(type, bttn.Cooldown);
        bttn.SetCooldownText(bttn.Cooldown);
        bttn.SetInteractable(bttn.Cooldown <= 0);
    }
}
