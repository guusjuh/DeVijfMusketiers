using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : SubUIManager {
    private RectTransform anchorCenter;
    public RectTransform AnchorCenter { get { return anchorCenter; } }
    private RectTransform anchorTopMid;
    private RectTransform anchorBottomRight;
    private RectTransform anchorBottomLeft;

    private GameObject playerTurnBanner;
    private GameObject enemyTurnBanner;
    private Text warningText;

    private EnemyInfoUI enemyInfoUI;
    public EnemyInfoUI EnemyInfoUI { get { return enemyInfoUI; } }

    private SkipTurnButton skipTurnButton;
    private PlayerActionPointsUI playerActionPoints;
    public PlayerActionPointsUI PlayerActionPoints { get { return playerActionPoints; } }

    private WorldObject target = null;

    private Dictionary<GameManager.SpellType, SpellButton> spellButtons;
    private bool spellButtonsOn = false;
    private const float RADIUS = 200f;

    private SpellVisual spellVisual;
    private bool castingSpell = false;
    public bool CastingSpell { get { return castingSpell; } set { castingSpell = value; } }

    private List<SurroundingPushButton> teleportButtons = new List<SurroundingPushButton>();
    private bool teleportButtonsOn = false;

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();
        anchorBottomLeft = canvas.gameObject.transform.Find("Anchor_BottomLeft").GetComponent<RectTransform>();

        enemyInfoUI = UIManager.Instance.CreateUIElement("Prefabs/UI/EnemyInfo", new Vector2(0.0f, -50.0f), anchorTopMid).GetComponent<EnemyInfoUI>();
        enemyInfoUI.Initialize();
        enemyInfoUI.Clear();

        skipTurnButton = UIManager.Instance.CreateUIElement("Prefabs/UI/SkipTurnButton", Vector2.zero, anchorBottomLeft).GetComponent<SkipTurnButton>();
        skipTurnButton.Initialize();

        playerActionPoints = UIManager.Instance.CreateUIElement("Prefabs/UI/PlayerActionPoints", new Vector2(-10.0f, 10.0f), anchorBottomRight).GetComponent<PlayerActionPointsUI>();
        playerActionPoints.Initialize();

        playerTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/YourTurn", new Vector2(0, 500), anchorCenter);
        playerTurnBanner.SetActive(false);

        enemyTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/OthersTurn", new Vector2(0, 500), anchorCenter); 
        enemyTurnBanner.SetActive(false);

        warningText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/WarningText"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponentInChildren<Text>();
        warningText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);
        warningText.gameObject.SetActive(false);

        spellButtons = new Dictionary<GameManager.SpellType, SpellButton>();
        CreateSpellButton(GameManager.SpellType.Attack, "Prefabs/UI/SpellButton/AttackButton");
        CreateSpellButton(GameManager.SpellType.FrostBite, "Prefabs/UI/SpellButton/FrostBiteButton");
        CreateSpellButton(GameManager.SpellType.Fireball, "Prefabs/UI/SpellButton/FireballButton");
        CreateSpellButton(GameManager.SpellType.Teleport, "Prefabs/UI/SpellButton/TeleportButton");

        InitializeTeleportButtons();

        spellVisual = UIManager.Instance.CreateUIElement("Prefabs/UI/SpellVisual/SpellInGame", Vector2.zero, anchorCenter).GetComponent<SpellVisual>();
        spellVisual.Initialize();
    }

    private void CreateSpellButton(GameManager.SpellType type, string prefabPath)
    {
        spellButtons.Add(type, UIManager.Instance.CreateUIElement(prefabPath, Vector2.zero, anchorCenter).GetComponent<SpellButton>());
        spellButtons.Get(type).Initialize();
        spellButtons.Get(type).gameObject.SetActive(false);
    }

    protected override void Restart()
    {
        enemyInfoUI.Restart();

        teleportButtonsOn = false;
        teleportButtons = new List<SurroundingPushButton>();
        InitializeTeleportButtons();

        skipTurnButton.gameObject.SetActive(true);
        playerActionPoints.gameObject.SetActive(true);
    }

    public override void Clear()
    {
        CastingSpell = false;

        // you knwo the banners are off

        // let enemy info ui clear itself
        enemyInfoUI.Clear();

        // clear skip and player ap elements
        skipTurnButton.gameObject.SetActive(false);
        playerActionPoints.gameObject.SetActive(false);

        // hide spell buttons 
        HideSpellButtons();
        //ActivateTeleportButtons(false);
        teleportButtons.HandleAction(b => b.Destory());
        teleportButtons.Clear();
        teleportButtons = null;

        base.Clear();
    }

    public void InitializeTeleportButtons()
    {
        for (int x = 0; x < GameManager.Instance.TileManager.Rows; x++)
        {
            for (int y = 0; y < GameManager.Instance.TileManager.Columns; y++)
            {
                Coordinate gridPos = new Coordinate(x,y);
                teleportButtons.Add(UIManager.Instance.CreateUIElement("Prefabs/UI/SpellButton/SurroundingPushButton", Vector2.zero, AnchorCenter).GetComponent<SurroundingPushButton>());

                teleportButtons.Last().Deactivate();
                teleportButtons.Last().Initialize(gridPos);
            }
        }
    }

    public override void Update()
    {
        // update button positions every frame
        if (spellButtonsOn)
        {
            SetSpellButtonPositions();
        }
        else if (teleportButtonsOn)
        {
            SetPushButtonPositions();
        }
    }

    public void ActivateTeleportButtons(bool on, MovableObject target = null)
    {
        //fill all possible surrounding TELEPORT buttons beforehand (each possible tile)
        List<SurroundingPushButton> tempButtons = new List<SurroundingPushButton>();
        for (int i = 0; i < teleportButtons.Count; i++)
        {
            if (!on)
            {
                tempButtons.Add(teleportButtons[i]);
            }
            else if (target != null && teleportButtons[i].GridPosition != target.GridPosition)
            {

                bool walkable = GameManager.Instance.TileManager.GetNodeReference(teleportButtons[i].GridPosition) != null &&
                                GameManager.Instance.TileManager.GetNodeReference(teleportButtons[i].GridPosition).WalkAble();

                if (walkable)
                {
                    tempButtons.Add(teleportButtons[i]);
                }
            }
        }

        if (on) tempButtons.HandleAction(b => b.Activate(target));
        else teleportButtons.HandleAction(b => b.Deactivate());

        this.target = target;
        teleportButtonsOn = on;
    }

    private void SetPushButtonPositions()
    {
        teleportButtons.HandleAction(b => b.SetPosition());
    }

    public void BeginPlayerTurn()
    {
        playerActionPoints.SetAPText();

        skipTurnButton.Active = true;
    }

    public void EndPlayerTurn()
    {
        playerActionPoints.SetAPText();

        skipTurnButton.Active = false;
    }

    public IEnumerator StartTurn(bool player)
    {
        if (player)
        {
            playerTurnBanner.SetActive(true);
            playerTurnBanner.GetComponentInChildren<Text>().text = "YOUR TURN: " + GameManager.Instance.LevelManager.AmountOfTurns;
        }
        else enemyTurnBanner.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        if (player) playerTurnBanner.SetActive(false);
        else enemyTurnBanner.SetActive(false);
    }

    public IEnumerator WarningText()
    {
        warningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        warningText.gameObject.SetActive(false);
    }

    public void ShowSpellButtons(WorldObject target)
    {
        HideSpellButtons();

        this.target = target;
        spellButtonsOn = true;

        for (int i = 0; i < this.target.PossibleSpellTypes.Count; i++)
        {
            spellButtons.Get(this.target.PossibleSpellTypes[i]).gameObject.SetActive(true);
            spellButtons.Get(this.target.PossibleSpellTypes[i]).Activate(target);
        }

        SetSpellButtonPositions();
    }

    private void SetSpellButtonPositions()
    {
        Vector2 canvasPos = WorldToCanvas(target.transform.position);

        float divider = target.PossibleSpellTypes.Count > 1 ? (float)target.PossibleSpellTypes.Count - 1.0f : (float)target.PossibleSpellTypes.Count;
        float partialCircle = (target.PossibleSpellTypes.Count - 1) / 4.0f * 0.9f;
        float offSetCircle = (1.0f - partialCircle) / 2.0f;

        for (int i = 0; i < target.PossibleSpellTypes.Count; i++)
        { 
            spellButtons.Get(target.PossibleSpellTypes[i]).GetComponent<RectTransform>().anchoredPosition = 
                canvasPos - CalculatePointOnCircle(RADIUS, partialCircle, divider, offSetCircle, i);
        }
    }

    public void HideSpellButtons()
    {
        spellButtonsOn = false;

        foreach (var pair in spellButtons)
        {
            pair.Value.gameObject.SetActive(false);
        }
    }

    public IEnumerator CastSpell(GameManager.SpellType type, Vector2 worldPos)
    {
        yield return UberManager.Instance.StartCoroutine(spellVisual.Activate(type, worldPos));

        spellVisual.gameObject.SetActive(false);
    }

    public static Vector2 CalculatePointOnCircle(float radius, float partialCircle, float divider, float offset, int index)
    {
        return new Vector2(radius * Mathf.Cos(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI),
                -radius * Mathf.Sin(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI));
    }
}
