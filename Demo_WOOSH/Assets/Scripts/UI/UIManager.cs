using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

//TODO: begin and end turn 
//TODO: do a lot of stuff to its own script
// skipturn button enable/disable
public class UIManager
{
    private EnemyInfoUI enemyInfoUI;
    public EnemyInfoUI EnemyInfoUI { get { return enemyInfoUI; } }

    private SkipTurnButton skipTurnButton;
    private PlayerActionPointsUI playerActionPoints;
    public PlayerActionPointsUI PlayerActionPoints { get { return playerActionPoints; } }

    private Canvas levelCanvas;
    private RectTransform anchorCenter;
    public RectTransform AnchorCenter { get { return anchorCenter; } }
    private RectTransform anchorTopMid;
    private RectTransform anchorBottomRight;
    private RectTransform anchorBottomLeft;

    private GameObject playerTurnBanner;
    private GameObject enemyTurnBanner;

    private Dictionary<GameManager.SpellType, SpellButton> spellButtons;

    private List<SurroundingPushButton> surroundingPushButtons;

    private Text warningText;

    private const float RADIUS = 200f;

    public void Initialize()
    {
        levelCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

        anchorCenter = levelCanvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopMid = levelCanvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomRight = levelCanvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();
        anchorBottomLeft = levelCanvas.gameObject.transform.Find("Anchor_BottomLeft").GetComponent<RectTransform>();

        enemyInfoUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/EnemyInfo"), Vector3.zero, Quaternion.identity, anchorTopMid.transform).GetComponent<EnemyInfoUI>();
        enemyInfoUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -50.0f, 0.0f);
        enemyInfoUI.Initialize();

        skipTurnButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SkipTurnButton"), Vector3.zero, Quaternion.identity, anchorBottomLeft.transform).GetComponent<SkipTurnButton>();
        skipTurnButton.Initialize();

        playerActionPoints = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlayerActionPoints"), Vector3.zero, Quaternion.identity, anchorBottomRight.transform).GetComponent<PlayerActionPointsUI>();
        playerActionPoints.Initialize();

        playerTurnBanner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/YourTurn"), Vector3.zero, Quaternion.identity, anchorCenter.transform);
        playerTurnBanner.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        playerTurnBanner.SetActive(false);

        enemyTurnBanner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/OthersTurn"), Vector3.zero, Quaternion.identity, anchorCenter.transform);
        enemyTurnBanner.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        enemyTurnBanner.SetActive(false);

        warningText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/WarningText"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponentInChildren<Text>();
        warningText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        warningText.gameObject.SetActive(false);

        spellButtons = new Dictionary<GameManager.SpellType, SpellButton>();
        spellButtons.Add(GameManager.SpellType.Attack, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/AttackButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Add(GameManager.SpellType.Invisible, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/InvisibleButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Add(GameManager.SpellType.Push, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/PushButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Add(GameManager.SpellType.Repair, GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/RepairButton"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponent<SpellButton>());
        spellButtons.Get(GameManager.SpellType.Attack).Initialize();
        spellButtons.Get(GameManager.SpellType.Invisible).Initialize();
        spellButtons.Get(GameManager.SpellType.Push).Initialize();
        spellButtons.Get(GameManager.SpellType.Repair).Initialize();
        spellButtons.Get(GameManager.SpellType.Attack).gameObject.SetActive(false);
        spellButtons.Get(GameManager.SpellType.Invisible).gameObject.SetActive(false);
        spellButtons.Get(GameManager.SpellType.Push).gameObject.SetActive(false);
        spellButtons.Get(GameManager.SpellType.Repair).gameObject.SetActive(false);

        surroundingPushButtons = new List<SurroundingPushButton>();
        Coordinate direction = new Coordinate(0, 0);

        for (int i = 0; i < 6; i++)
        {
            surroundingPushButtons.Add(
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/SpellButton/SurroundingPushButton"), Vector3.zero, Quaternion.identity, GameManager.Instance.UiManager.AnchorCenter)
                .GetComponent<SurroundingPushButton>());

            surroundingPushButtons[i].Deactivate();
            surroundingPushButtons[i].Initialize(new Coordinate(0,0), direction);
        }
    }

    public void ActivatePushButtons(bool on, MovableObject target)
    {
        if (on)
        {
            // set the button positions 
            Coordinate direction = new Coordinate(0, 0);
            for (int i = 0; i < surroundingPushButtons.Count; i++)
            {
                direction = GameManager.Instance.TileManager.Directions(target.GridPosition)[i];

                Vector3 worldPos = GameManager.Instance.TileManager.GetWorldPosition(target.GridPosition + direction);
                surroundingPushButtons[i].GetComponent<RectTransform>().anchoredPosition =
                    GameManager.Instance.UiManager.WorldToCanvas(worldPos);

                surroundingPushButtons[i].GetComponent<RectTransform>().anchoredPosition =
                    GameManager.Instance.UiManager.WorldToCanvas(worldPos);

                surroundingPushButtons[i].Initialize(target.GridPosition + direction, direction);
            }

            // search relevant buttons and activate
            List<SurroundingPushButton> tempButtons = new List<SurroundingPushButton>();

            for (int i = 0; i < surroundingPushButtons.Count; i++)
            {
                if (GameManager.Instance.TileManager.GetNodeReference(surroundingPushButtons[i].GridPosition) != null)
                {
                    if (GameManager.Instance.TileManager.GetNodeReference(surroundingPushButtons[i].GridPosition).Content.CompletelyEmpty())
                    {
                        tempButtons.Add(surroundingPushButtons[i]);
                    }
                }
            }

            tempButtons.HandleAction(b => b.Activate(target));
        }
        else
        {
            surroundingPushButtons.HandleAction(b => b.Deactivate());
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
        if (player) playerTurnBanner.SetActive(true);
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

    public void ShowSpellButtons(Vector2 position, List<GameManager.SpellType> spellTypes, WorldObject target)
    {
        //TODO: check is the human can be pushed
        //TODO: check if the invisibility spell is a legal move
        HideSpellButtons();

        Vector2 canvasPos = WorldToCanvas(position);

        float divider = spellTypes.Count > 1 ? (float)spellTypes.Count - 1.0f : (float)spellTypes.Count;
        float partialCircle = (spellTypes.Count - 1) / 4.0f * 0.9f;
        float offSetCircle = (1.0f - partialCircle) / 2.0f;

        for (int i = 0; i < spellTypes.Count; i++)
        {
            spellButtons.Get(spellTypes[i]).gameObject.SetActive(true);
            spellButtons.Get(spellTypes[i]).Activate(target);

            Vector2 pos = new Vector2(RADIUS * Mathf.Cos(partialCircle * Mathf.PI * (float) i / divider + offSetCircle * Mathf.PI),
                -RADIUS * Mathf.Sin(partialCircle * Mathf.PI * (float) i / divider + offSetCircle * Mathf.PI));
            spellButtons.Get(spellTypes[i]).GetComponent<RectTransform>().anchoredPosition = canvasPos - pos;
        }
    }

    public void HideSpellButtons()
    {
        foreach (var pair in spellButtons)
        {
            pair.Value.gameObject.SetActive(false);
        }
    }

    public Vector2 WorldToCanvas(Vector3 worldPosition)
    {
        //TODO: lol we don't have a camera script or ref
        Camera camera = Camera.main;

        var viewportPos = camera.WorldToViewportPoint(worldPosition);
        var canvasRect = levelCanvas.GetComponent<RectTransform>();

        return new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
    }
}
