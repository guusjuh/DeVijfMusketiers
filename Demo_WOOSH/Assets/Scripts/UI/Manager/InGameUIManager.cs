using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : SubUIManager {
    public RectTransform AnchorCenter { get { return anchorCenter; } }

    private GameObject playerTurnBanner;
    private GameObject enemyTurnBanner;
    private Text warningText;

    private EnemyInfoUI enemyInfoUI;
    public EnemyInfoUI EnemyInfoUI { get { return enemyInfoUI; } }

    private PlayerAPSkipButton playerAPIndicator;
    public PlayerAPSkipButton PlayerApIndicator { get { return playerAPIndicator; } }

    private WorldObject selectedObject = null;

    private Dictionary<GameManager.SpellType, SpellButton> spellButtons;
    private bool spellButtonsOn = false;
    //used for placing the spellbuttons
    private const float WORLDOBJECT_RADIUS = 200f;

    public SpellButton AttackButton { get { return spellButtons[GameManager.SpellType.Attack]; } }
    public SpellButton TeleportButton { get { return spellButtons[GameManager.SpellType.Teleport]; } }

    private SpellVisual spellVisual;
    private GameObject wizard;
    private Animator wizardAnimController;

    private int castingSpell = -1;
    public int CastingSpell { get { return castingSpell; } set { castingSpell = value; } }
    private Dictionary<GameManager.SpellType, Color> spellColors;
    public Color SpellColor(GameManager.SpellType type) { return spellColors[type]; }

    private List<SurroundingTeleportButton> teleportButtons = new List<SurroundingTeleportButton>();
    private bool teleportButtonsOn = false;

    //------------------ TUTORIAL VARS ------------------------
    private GameObject guidanceArrow;
    private Text guidanceText;
    //---------------------------------------------------------

    protected override void Initialize()
    {
        canvasName = "InGameCanvas";
        base.Initialize();

        CreateUIElements();

        teleportButtonsOn = false;
        teleportButtons = new List<SurroundingTeleportButton>();
        InitializeTeleportButtons();

        spellColors = new Dictionary<GameManager.SpellType, Color>();
        spellColors.Add(GameManager.SpellType.Attack, Color.white);
        spellColors.Add(GameManager.SpellType.Fireball, Color.red);
        spellColors.Add(GameManager.SpellType.FrostBite, Color.blue);
        spellColors.Add(GameManager.SpellType.Teleport, Color.magenta);

        FinishInitialize();

        if (UberManager.Instance.DevelopersMode) Pause(true);
    }

    private void CreateUIElements()
    {
        //Enemy HUD
        enemyInfoUI = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/EnemyInfo", new Vector2(0.0f, -50.0f), anchorTopCenter).GetComponent<EnemyInfoUI>();
        enemyInfoUI.Initialize();
        enemyInfoUI.Clear();

        playerAPIndicator = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/AP-Skip-Indicator", Vector2.zero, anchorBottomLeft).GetComponent<PlayerAPSkipButton>();
        playerAPIndicator.Initialize();

        wizard = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/WizardUI", new Vector2(-100.0f, 0.0f), anchorBottomRight).gameObject;
        wizardAnimController = wizard.GetComponent<Animator>();

        playerTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/YourTurn", new Vector2(0, 500), anchorCenter);
        playerTurnBanner.SetActive(false);

        enemyTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/OthersTurn", new Vector2(0, 500), anchorCenter);
        enemyTurnBanner.SetActive(false);

        //Gap warning
        warningText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/InGame/WarningText"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponentInChildren<Text>();
        warningText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);
        warningText.gameObject.SetActive(false);

        spellVisual = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellVisual/SpellInGame", Vector2.zero, anchorCenter).GetComponent<SpellVisual>();
        spellVisual.Initialize();
    }

    protected override void InitializeTutorial()
    {
        noClickPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/NoClickPanel", Vector2.zero, canvas.transform).GetComponent<RectTransform>();
        onlyButton = noClickPanel.Find("OnlyButton").GetComponent<Button>();
        clickToContinue = noClickPanel.Find("ClickToContinue").GetComponent<Button>();
        onlyButton.gameObject.SetActive(false);
        clickToContinue.gameObject.SetActive(false);

        guidanceArrow = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceArrow", Vector2.zero, canvas.transform);
        guidanceText = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceText", Vector2.zero, canvas.transform).GetComponent<Text>();

        spellButtons = new Dictionary<GameManager.SpellType, SpellButton>();
        CreateSpellButton(GameManager.SpellType.Attack, "Prefabs/UI/InGame/SpellButton/AttackButton");
        CreateSpellButton(GameManager.SpellType.Teleport, "Prefabs/UI/InGame/SpellButton/TeleportButton");
    }

    protected override void InitializeInGame()
    {
        //for tutorial spells
        if (spellButtons != null)
        {
            GameObject.Destroy(spellButtons[GameManager.SpellType.Attack].gameObject);
            GameObject.Destroy(spellButtons[GameManager.SpellType.Teleport].gameObject);
            spellButtons = null;
        }

        InitializeTeleportButtons();

        spellButtons = new Dictionary<GameManager.SpellType, SpellButton>();
        CreateSpellButton(GameManager.SpellType.Attack, "Prefabs/UI/InGame/SpellButton/AttackButton");
        CreateSpellButton(GameManager.SpellType.FrostBite, "Prefabs/UI/InGame/SpellButton/FrostBiteButton");
        CreateSpellButton(GameManager.SpellType.Fireball, "Prefabs/UI/InGame/SpellButton/FireballButton");
        CreateSpellButton(GameManager.SpellType.Teleport, "Prefabs/UI/InGame/SpellButton/TeleportButton");

        enemyInfoUI.Restart();

        playerAPIndicator.gameObject.SetActive(true);
        wizard.gameObject.SetActive(true);
    }

    public void SetArrow(Vector2 centerPos, float zRotation, float radius, string text)
    {
        guidanceArrow.SetActive(true);
        guidanceText.gameObject.SetActive(true);
        guidanceText.text = text;

        guidanceArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        Quaternion q = Quaternion.AngleAxis(zRotation, Vector3.forward);
        guidanceArrow.GetComponent<RectTransform>().Rotate(Vector3.forward, zRotation);

        guidanceArrow.GetComponent<RectTransform>().anchoredPosition = centerPos + (Vector2)(q * Vector2.right * radius);
        guidanceText.GetComponent<RectTransform>().anchoredPosition = centerPos + (Vector2)(q * Vector2.right * (radius + 75.0f));
    }

    public override void DeactivateNoClickPanel()
    {
        base.DeactivateNoClickPanel();
        guidanceArrow.SetActive(false);
        guidanceArrow.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
        guidanceText.gameObject.SetActive(false);
    }

    public void DeactivateBanners()
    {
        enemyTurnBanner.gameObject.SetActive(false);
        playerTurnBanner.gameObject.SetActive(false);
    }

    private void CreateSpellButton(GameManager.SpellType type, string prefabPath)
    {
        spellButtons.Add(type, UIManager.Instance.CreateUIElement(prefabPath, Vector2.zero, anchorCenter).GetComponent<SpellButton>());
        spellButtons.Get(type).Initialize();
        spellButtons.Get(type).gameObject.SetActive(false);
    }

    protected override void Restart()
    {
        if (!UberManager.Instance.Tutorial && !initializedInGame)
        {
            InitializeInGame();
            return;
        }
        InitializeTeleportButtons();

        enemyInfoUI.Restart();

        playerAPIndicator.Reset();
        wizard.gameObject.SetActive(true);

        if (UberManager.Instance.DevelopersMode) Pause(true);
    }

    public override void Clear()
    {
        CastingSpell = -1;

        // let enemy info ui clear itself
        enemyInfoUI.Clear();
        playerAPIndicator.Clear();
        wizard.gameObject.SetActive(false);

        // hide spell buttons 
        HideSpellButtons();
        teleportButtonsOn = false;
        teleportButtons.HandleAction(b => b.Destroy());
        teleportButtons.Clear();
        teleportButtons = null;

        if(UberManager.Instance.Tutorial) DeactivateNoClickPanel();

        base.Clear();
    }

    public void Pause(bool on)
    {
        if (on)
        {
            foreach (var pair in spellButtons)
            {
                pair.Value.gameObject.SetActive(false);
            }
            enemyInfoUI.OnChange();
            playerAPIndicator.gameObject.SetActive(false);

            wizard.gameObject.SetActive(false);
        }
        else
        {
            if (spellButtonsOn)
            {
                foreach (var pair in spellButtons)
                {
                    pair.Value.gameObject.SetActive(true);
                }
            }
            playerAPIndicator.gameObject.SetActive(true);
            wizard.gameObject.SetActive(true);
        }
    }

    public void InitializeTeleportButtons()
    {
        teleportButtons = new List<SurroundingTeleportButton>();

        for (int x = 0; x < GameManager.Instance.TileManager.Rows; x++)
        {
            for (int y = 0; y < GameManager.Instance.TileManager.Columns; y++)
            {
                Coordinate gridPos = new Coordinate(x,y);
                teleportButtons.Add(UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellButton/SurroundingPushButton", Vector2.zero, AnchorCenter).GetComponent<SurroundingTeleportButton>());

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
            SetTeleportButtonPositions();
        }
    }

    public void ActivateTeleportButtons(bool on, MovableObject target = null)
    {
        //fill all possible surrounding TELEPORT buttons beforehand (each possible tile)
        List<SurroundingTeleportButton> tempButtons = new List<SurroundingTeleportButton>();
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

        this.selectedObject = target;
        teleportButtonsOn = on;
    }

    private void SetTeleportButtonPositions()
    {
        teleportButtons.HandleAction(b => b.SetPosition());
    }

    public void BeginPlayerTurn()
    {
        playerAPIndicator.SetAPText();
    }

    public void EndPlayerTurn()
    {
        playerAPIndicator.SetAPText();
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

        this.selectedObject = target;
        spellButtonsOn = true;

        for (int i = 0; i < selectedObject.PossibleSpellTypes.Count; i++)
        {
            spellButtons.Get(selectedObject.PossibleSpellTypes[i]).gameObject.SetActive(true);
            spellButtons.Get(selectedObject.PossibleSpellTypes[i]).Activate(target);
        }

        SetSpellButtonPositions();
    }

    private void SetSpellButtonPositions()
    {
        Vector2 canvasPos = WorldToCanvas(selectedObject.transform.position);

        //divides spellbuttons over a circle, so they can be positioned correctly
        float divider = selectedObject.PossibleSpellTypes.Count > 1 ? (float)selectedObject.PossibleSpellTypes.Count - 1.0f : (float)selectedObject.PossibleSpellTypes.Count;
        float partialCircle = (selectedObject.PossibleSpellTypes.Count - 1) / 4.0f * 0.9f;
        float offSetCircle = (1.0f - partialCircle) / 2.0f;

        for (int i = 0; i < selectedObject.PossibleSpellTypes.Count; i++)
        { 
            spellButtons.Get(selectedObject.PossibleSpellTypes[i]).GetComponent<RectTransform>().anchoredPosition = 
                canvasPos - CalculatePointOnCircle(partialCircle, divider, offSetCircle, i);
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
        wizardAnimController.SetTrigger("CastSpell");

        yield return UberManager.Instance.StartCoroutine(spellVisual.Activate(type, worldPos));

        spellVisual.gameObject.SetActive(false);
    }

    public void HumanDied()
    {
        if (wizard == null) return;
        wizardAnimController.SetTrigger("HoomanDied");
    }

    public void EnemyDied()
    {
        if (wizard == null) return;
        wizardAnimController.SetTrigger("EnemyDied");
    }

    /// <summary>
    /// calculates a point on a circle around the selected worldobject
    /// </summary>
    public static Vector2 CalculatePointOnCircle(float partialCircle, float divider, float offset, int index)
    {
        return new Vector2(WORLDOBJECT_RADIUS * Mathf.Cos(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI),
                -WORLDOBJECT_RADIUS * Mathf.Sin(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI));
    }
}