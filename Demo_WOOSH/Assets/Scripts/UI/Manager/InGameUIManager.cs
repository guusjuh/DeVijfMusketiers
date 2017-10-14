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

    private EnemyInfoUI enemyInfoUI;
    public EnemyInfoUI EnemyInfoUI { get { return enemyInfoUI; } }

    private SkipTurnButton skipTurnButton;
    private PlayerActionPointsUI playerActionPoints;
    public PlayerActionPointsUI PlayerActionPoints { get { return playerActionPoints; } }

    private WorldObject target = null;

    private Dictionary<GameManager.SpellType, SpellButton> spellButtons;
    private bool spellButtonsOn = false;

    private List<SurroundingPushButton> surroundingPushButtons = new List<SurroundingPushButton>();
    private bool pushButtonsOn = false;

    private Text warningText;

    private const float RADIUS = 200f;

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<Canvas>();

        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();
        anchorBottomLeft = canvas.gameObject.transform.Find("Anchor_BottomLeft").GetComponent<RectTransform>();

        enemyInfoUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/EnemyInfo"), Vector3.zero, Quaternion.identity, anchorTopMid.transform).GetComponent<EnemyInfoUI>();
        enemyInfoUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -50.0f, 0.0f);
        enemyInfoUI.Initialize();
        enemyInfoUI.Clear();

        skipTurnButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SkipTurnButton"), Vector3.zero, Quaternion.identity, anchorBottomLeft.transform).GetComponent<SkipTurnButton>();
        skipTurnButton.Initialize();

        playerActionPoints = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlayerActionPoints"), Vector3.zero, Quaternion.identity, anchorBottomRight.transform).GetComponent<PlayerActionPointsUI>();
        playerActionPoints.Initialize();

        playerTurnBanner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/YourTurn"), Vector3.zero, Quaternion.identity, anchorCenter.transform);
        playerTurnBanner.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);
        playerTurnBanner.SetActive(false);

        enemyTurnBanner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/OthersTurn"), Vector3.zero, Quaternion.identity, anchorCenter.transform);
        enemyTurnBanner.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);
        enemyTurnBanner.SetActive(false);

        warningText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/WarningText"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponentInChildren<Text>();
        warningText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);
        warningText.gameObject.SetActive(false);

        spellButtons = new Dictionary<GameManager.SpellType, SpellButton>();
        spellButtons.Add(GameManager.SpellType.Attack, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/AttackButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Get(GameManager.SpellType.Attack).Initialize();
        spellButtons.Get(GameManager.SpellType.Attack).gameObject.SetActive(false);

        spellButtons.Add(GameManager.SpellType.FrostBite, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/FrostBiteButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Get(GameManager.SpellType.FrostBite).Initialize();
        spellButtons.Get(GameManager.SpellType.FrostBite).gameObject.SetActive(false);

        spellButtons.Add(GameManager.SpellType.Fireball, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/FireballButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Get(GameManager.SpellType.Fireball).Initialize();
        spellButtons.Get(GameManager.SpellType.Fireball).gameObject.SetActive(false);

        spellButtons.Add(GameManager.SpellType.Teleport, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/TeleportButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Get(GameManager.SpellType.Teleport).Initialize();
        spellButtons.Get(GameManager.SpellType.Teleport).gameObject.SetActive(false);

        InitializeTeleportButtons();
    }

    protected override void Restart()
    {
        enemyInfoUI.Restart();
        skipTurnButton.gameObject.SetActive(true);
        playerActionPoints.gameObject.SetActive(true);
    }

    public override void Clear()
    {
        // you knwo the banners are off

        // let enemy info ui clear itself
        enemyInfoUI.Clear();

        // clear skip and player ap elements
        skipTurnButton.gameObject.SetActive(false);
        playerActionPoints.gameObject.SetActive(false);

        // hide spell buttons 
        HideSpellButtons();
        ActivateTeleportButtons(false);

        base.Clear();
    }

    public void InitializeTeleportButtons()
    {
        TileNode[,] grid = GameManager.Instance.TileManager.Grid;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Coordinate gridPos = new Coordinate(x,y);
                GameObject temp = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/SurroundingPushButton"),Vector3.zero, Quaternion.identity, AnchorCenter);
                surroundingPushButtons.Add(temp.GetComponent<SurroundingPushButton>());

                surroundingPushButtons.Last().Deactivate();
                surroundingPushButtons.Last().Initialize(gridPos);
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
        else if (pushButtonsOn)
        {
            SetPushButtonPositions();
        }
    }

    public void ActivateTeleportButtons(bool on, MovableObject target = null)
    {
        //fill all possible surrounding TELEPORT buttons beforehand (each possible tile)
        //TODO: highlight all tiles
        List<SurroundingPushButton> tempButtons = new List<SurroundingPushButton>();
        for (int i = 0; i < surroundingPushButtons.Count; i++)
        {
            if (!on)
            {
                //dehighlight button
                tempButtons.Add(surroundingPushButtons[i]);

            } else if (target != null && surroundingPushButtons[i].GridPosition != target.GridPosition)
            {
                bool walkable = GameManager.Instance.TileManager.GetNodeReference(surroundingPushButtons[i].GridPosition) != null &&
                    GameManager.Instance.TileManager.GetNodeReference(surroundingPushButtons[i].GridPosition)
                                                    .Content.WalkAble();
                if (walkable)
                {
                    tempButtons.Add(surroundingPushButtons[i]);
                }
            }
        }
        if (on) tempButtons.HandleAction(b => b.Activate(target));
        else surroundingPushButtons.HandleAction(b => b.Deactivate());
    }

    private void SetPushButtonPositions()
    {
        Coordinate direction = new Coordinate(0, 0);
        Vector3 worldPos = new Vector3();
        for (int i = 0; i < surroundingPushButtons.Count; i++)
        {
            direction = GameManager.Instance.TileManager.Directions(target.GridPosition)[i];
            worldPos = GameManager.Instance.TileManager.GetWorldPosition(target.GridPosition + direction);

            surroundingPushButtons[i].SetPosition(worldPos);
        }
    }

    public void BeginPlayerTurn()
    {
        playerActionPoints.SetAPText();

        skipTurnButton.Active = true;

        //TODO: maybe this from enemy class, but dissable enemyinfo
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
            playerTurnBanner.GetComponent<YourTurn>().UpdateTurn();
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
            Vector2 pos = new Vector2(RADIUS * Mathf.Cos(partialCircle * Mathf.PI * (float)i / divider + offSetCircle * Mathf.PI),
                -RADIUS * Mathf.Sin(partialCircle * Mathf.PI * (float)i / divider + offSetCircle * Mathf.PI));
            spellButtons.Get(target.PossibleSpellTypes[i]).GetComponent<RectTransform>().anchoredPosition = canvasPos - pos;
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
}
