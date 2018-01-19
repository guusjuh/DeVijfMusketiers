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

    private PlayerAPSkipButton playerActionPoints;
    public PlayerAPSkipButton PlayerActionPoints { get { return playerActionPoints; } }

    //something spell
    private const float RADIUS = 200f;

    private GameObject wizard;
    private Animator wizardAnimController;

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

        enemyInfoUI = UIManager.Instance.CreateAndInitUIElement<EnemyInfoUI>("Prefabs/UI/InGame/EnemyInfo", new Vector2(0.0f, -50.0f), anchorTopMid);
        enemyInfoUI.Clear();

        playerActionPoints = UIManager.Instance.CreateAndInitUIElement<PlayerAPSkipButton>("Prefabs/UI/InGame/AP-Skip-Indicator", Vector2.zero, anchorBottomLeft);

        wizard = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/WizardUI", new Vector2(-100.0f, 0.0f), anchorBottomRight).gameObject;
        wizardAnimController = wizard.GetComponent<Animator>();

        playerTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/YourTurn", new Vector2(0, 500), anchorCenter);
        playerTurnBanner.SetActive(false);

        enemyTurnBanner = UIManager.Instance.CreateUIElement("Prefabs/UI/InGame/OthersTurn", new Vector2(0, 500), anchorCenter);
        enemyTurnBanner.SetActive(false);

        warningText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/InGame/WarningText"), Vector3.zero, Quaternion.identity, anchorCenter.transform).GetComponentInChildren<Text>();
        warningText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 500);
        warningText.gameObject.SetActive(false);

        if (!UberManager.Instance.Tutorial && !initializedInGame)
        {
            InitializeInGame();
            initializedInGame = true;
        }
        else
        {
            //TODO: if teleportButtons are not working in tutorial uncomment line below
            //InitializeTeleportButtons();
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
    }

    protected override void InitializeInGame()
    {
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

    protected override void Restart()
    {
        if (!UberManager.Instance.Tutorial && !initializedInGame)
        {
            InitializeInGame();
            return;
        }

        enemyInfoUI.Restart();

        playerActionPoints.Reset();
        wizard.gameObject.SetActive(true);

        if (UberManager.Instance.DevelopersMode) Pause(true);
    }

    public override void Clear()
    {
        UberManager.Instance.SpellManager.CastingSpell = SpellManager.SpellType.NoSpell;

        // you knwo the banners are off

        // let enemy info ui clear itself
        enemyInfoUI.Clear();

        // clear player ap elements
        playerActionPoints.gameObject.SetActive(false);
        playerActionPoints.SetAPText();
        wizard.gameObject.SetActive(false);

        // hide spell buttons 
        //HideSpellButtons();

        //TODO: implement new spellsystem
        //teleportButtonsOn = false;
        //teleportButtons.HandleAction(b => b.Destory());
        //teleportButtons.Clear();
        //teleportButtons = null;

        if (UberManager.Instance.Tutorial) DeactivateNoClickPanel();

        base.Clear();
    }

    public void Pause(bool on)
    {
        if (on)
        {
            UberManager.Instance.SpellManager.HideSpellButtons();

            enemyInfoUI.OnChange();
            playerActionPoints.gameObject.SetActive(false);

            wizard.gameObject.SetActive(false);
        }
        else
        {
            if (UberManager.Instance.SpellManager.SpellButtonsActive)
            {
                UberManager.Instance.SpellManager.ShowSpellButtons();
            }

            playerActionPoints.gameObject.SetActive(true);

            wizard.gameObject.SetActive(true);
        }
    }

    public override void Update()
    {
        // update button positions every frame the're active
        if (UberManager.Instance.SpellManager.SpellButtonsActive)
        {
            UberManager.Instance.SpellManager.UpdateButtonPositions();
        }
    }

    public void BeginPlayerTurn()
    {
        playerActionPoints.SetAPText();
    }

    public void EndPlayerTurn()
    {
        playerActionPoints.SetAPText();
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

    public void SpellIsCast()
    {
        if (wizard == null) return;
        wizardAnimController.SetTrigger("CastSpell");
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
}
