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

    //private SkipTurnButton skipTurnButton;
    private PlayerAPSkipButton playerActionPoints;
    public PlayerAPSkipButton PlayerActionPoints { get { return playerActionPoints; } }

    private WorldObject target = null;

    private Dictionary<GameManager.SpellType, SpellButton> spellButtons;
    private bool spellButtonsOn = false;
    private const float RADIUS = 200f;

    public SpellButton AttackButton { get { return spellButtons[GameManager.SpellType.Attack]; } }
    public SpellButton TeleportButton { get { return spellButtons[GameManager.SpellType.Teleport]; } }

    private SpellVisual spellVisual;
    private GameObject wizard;
    private Animator wizardAnimController;

    private int castingSpell = -1;
    public int CastingSpell { get { return castingSpell; } set { castingSpell = value; } }
    public Dictionary<GameManager.SpellType, Color> SpellColors;

    private List<SurroundingPushButton> teleportButtons = new List<SurroundingPushButton>();
    private bool teleportButtonsOn = false;

    //------------------ TUTORIAL VARS ------------------------
    private GameObject guidanceArrow;
    private Text guidanceText;
    //---------------------------------------------------------

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();
        anchorBottomLeft = canvas.gameObject.transform.Find("Anchor_BottomLeft").GetComponent<RectTransform>();

        enemyInfoUI = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/EnemyInfo", new Vector2(0.0f, -50.0f), anchorTopMid).GetComponent<EnemyInfoUI>();
        enemyInfoUI.Initialize();
        enemyInfoUI.Clear();

        playerActionPoints = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/AP-Skip-Indicator", Vector2.zero, anchorBottomLeft).GetComponent<PlayerAPSkipButton>();
        playerActionPoints.Initialize();
        wizard = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/WizardUI", new Vector2(-100.0f, 0.0f), anchorBottomRight).gameObject;
        wizardAnimController = wizard.GetComponent<Animator>();

        playerTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/YourTurn", new Vector2(0, 500), anchorCenter);
        playerTurnBanner.SetActive(false);

        enemyTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/OthersTurn", new Vector2(0, 500), anchorCenter);
        enemyTurnBanner.SetActive(false);

        warningText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/InGame/WarningText"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponentInChildren<Text>();
        warningText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);
        warningText.gameObject.SetActive(false);

        //spellVisual = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellVisual/SpellInGame", Vector2.zero, anchorCenter).GetComponent<SpellVisual>();
        //spellVisual.Initialize();
 
        teleportButtonsOn = false;
        teleportButtons = new List<SurroundingPushButton>();
        InitializeTeleportButtons();

        SpellColors = new Dictionary<GameManager.SpellType, Color>();
        SpellColors.Add(GameManager.SpellType.Attack, Color.white);
        SpellColors.Add(GameManager.SpellType.Fireball, Color.red);
        SpellColors.Add(GameManager.SpellType.FrostBite, Color.blue);
        SpellColors.Add(GameManager.SpellType.Teleport, Color.magenta);

        if (!UberManager.Instance.Tutorial && !initializedInGame)
        {
            InitializeInGame();
            initializedInGame = true;
        }
        else
        {
            InitializeTutorial();
        }

        if (UberManager.Instance.DevelopersMode) Pause(true);
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

        playerActionPoints.gameObject.SetActive(true);
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

        playerActionPoints.Reset();
        wizard.gameObject.SetActive(true);

        if (UberManager.Instance.DevelopersMode) Pause(true);
    }

    public override void Clear()
    {
        CastingSpell = -1;

        // you knwo the banners are off

        // let enemy info ui clear itself
        enemyInfoUI.Clear();

        // clear player ap elements
        playerActionPoints.gameObject.SetActive(false);
        playerActionPoints.SetAPText();
        wizard.gameObject.SetActive(false);

        // hide spell buttons 
        HideSpellButtons();

        teleportButtonsOn = false;
        teleportButtons.HandleAction(b => b.Destory());
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
            playerActionPoints.gameObject.SetActive(false);

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

            playerActionPoints.gameObject.SetActive(true);

            wizard.gameObject.SetActive(true);
        }
    }

    public void InitializeTeleportButtons()
    {
        teleportButtons = new List<SurroundingPushButton>();

        for (int x = 0; x < GameManager.Instance.TileManager.Rows; x++)
        {
            for (int y = 0; y < GameManager.Instance.TileManager.Columns; y++)
            {
                Coordinate gridPos = new Coordinate(x,y);
                teleportButtons.Add(UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/SpellButton/SurroundingPushButton", Vector2.zero, AnchorCenter).GetComponent<SurroundingPushButton>());

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

        //TODO: skipTurnButton.Active = true;
    }

    public void EndPlayerTurn()
    {
        playerActionPoints.SetAPText();

        //TODO: skipTurnButton.Active = false;
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
       // wizardAnimController.SetTrigger("CastSpell");

         //yield return UberManager.Instance.StartCoroutine(spellVisual.Activate(type, worldPos));

        //spellVisual.gameObject.SetActive(false);
        yield return null;
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

    public static Vector2 CalculatePointOnCircle(float radius, float partialCircle, float divider, float offset, int index)
    {
        return new Vector2(radius * Mathf.Cos(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI),
                -radius * Mathf.Sin(partialCircle * Mathf.PI * (float)index / divider + offset * Mathf.PI));
    }
}
