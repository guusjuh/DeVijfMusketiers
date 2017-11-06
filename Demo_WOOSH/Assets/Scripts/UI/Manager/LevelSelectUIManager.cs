using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUIManager : SubUIManager
{
    private RectTransform anchorCenter;
    public RectTransform AnchorCenter {get { return anchorCenter; } }
    private RectTransform anchorBottomRight;
    private RectTransform anchorTopMid;
    private RectTransform anchorBottomCenter;

    private GameObject levelSelectPanel;

    private ReputationUIManager reputationParent;
    public ReputationUIManager ReputationParent { get { return reputationParent; } }

    private SelectContractWindow selectContractWindow;

    public SelectContractWindow SelectContractWindow
    {
        get
        {
            if (UberManager.Instance.Tutorial) return tutorialSelectContractWindow;
            else return selectContractWindow;
        }
    }

    private List<City> cities;
    public List<City> Cities { get { return cities; } }

    private ReputationUpUI repUpUI;
    private int lastRep = 1;            //TODO: get from player account

    //------------------ TUTORIAL VARS ------------------------
    private GameObject tutorialPanel;

    private GameObject guidanceArrow;
    private Text guidanceText;

    private SelectContractWindow tutorialSelectContractWindow;

    private City tutorialCity;
    public City TutorialCity { get { return tutorialCity; } }

    private Path tutorialPath;
    public Path TutorialPath { get { return tutorialPath; } }

    private GameObject wizardsHat;
    public GameObject WizardsHat { get { return wizardsHat; } }

    private DialogScript dialog;
    public DialogScript Dialog { get { return dialog;  } }
    //---------------------------------------------------------
    
    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("LevelSelectCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomCenter = canvas.gameObject.transform.Find("Anchor_BottomCenter").GetComponent<RectTransform>();

        if (!UberManager.Instance.Tutorial && !initializedInGame)
        {
            InitializeInGame();
            initializedInGame = true;
        }
        else
        {
            InitializeTutorial();
        }
    }

    protected override void InitializeTutorial()
    {
        tutorialPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/TutorialPanel", Vector2.zero, anchorCenter);

        tutorialSelectContractWindow = new SelectContractWindow(UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/SelectContractMenuPanel", Vector2.zero, canvas.transform).transform.GetChild(0).gameObject);

        tutorialCity = tutorialPanel.GetComponentInChildren<City>();
        tutorialCity.Initiliaze();

        tutorialCity.Reached();

        tutorialPath = tutorialCity.Paths[0];

        wizardsHat = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/Hat", new Vector2(0, -50.0f), tutorialPanel.transform).gameObject;
        wizardsHat.transform.SetSiblingIndex(1);

        dialog = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/DialogPopup", Vector2.zero, anchorBottomCenter).GetComponent<DialogScript>();
        dialog.Initialize();

        noClickPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/NoClickPanel", Vector2.zero, canvas.transform).GetComponent<RectTransform>();
        onlyButton = noClickPanel.Find("OnlyButton").GetComponent<Button>();
        clickToContinue = noClickPanel.Find("ClickToContinue").GetComponent<Button>();
        onlyButton.gameObject.SetActive(false);
        clickToContinue.gameObject.SetActive(false);
        tutorialIndicator = new NewContractIndicator(onlyButton.transform.Find("Indicator").gameObject);
        tutorialIndicator.SetState(City.ContractState.Nothing);

        guidanceArrow = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceArrow", Vector2.zero, canvas.transform);
        guidanceText = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceText", Vector2.zero, canvas.transform).GetComponent<Text>();
        DeactivateNoClickPanel();
        ActivateNoClickPanel(wizardsHat.GetComponent<RectTransform>().anchoredPosition, wizardsHat.GetComponentInChildren<Image>().sprite);
    }

    protected override void InitializeInGame()
    {
        levelSelectPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/LevelSelectScrollView", Vector2.zero, anchorCenter).transform.Find("LevelSelectPanel").gameObject;
        cities = new List<City>(levelSelectPanel.GetComponentsInChildren<City>());
        cities.HandleAction(c => c.Initiliaze());

        cities[0].Reached();

        reputationParent = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/ReputationParent", new Vector2(-20, -20), anchorTopMid).GetComponent<ReputationUIManager>();
        reputationParent.Initialize();

        selectContractWindow = new SelectContractWindow(UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/SelectContractMenuPanel", Vector2.zero, canvas.transform).transform.GetChild(0).gameObject);

        repUpUI = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/RepUpPanel", Vector2.zero, canvas.transform).GetComponent<ReputationUpUI>();
        repUpUI.Initialze();
        lastRep = UberManager.Instance.PlayerData.ReputationLevel;

        initializedInGame = true;
    }

    public override void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, float width = 100, float height = 100)
    {
        base.ActivateNoClickPanel(onlyButtonPos, buttonSprite, width, height);

        tutorialIndicator.SetState(City.ContractState.New);
    }

    public void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, bool indicatorOn, float width = 100, float height = 100)
    {
        base.ActivateNoClickPanel(onlyButtonPos, buttonSprite, width, height);

        if (indicatorOn) tutorialIndicator.SetState(City.ContractState.New);
        else tutorialIndicator.SetState(City.ContractState.Nothing);
    }

    public override void DeactivateNoClickPanel()
    {
        base.DeactivateNoClickPanel();

        tutorialIndicator.SetState(City.ContractState.Nothing);
        guidanceArrow.SetActive(false);
        guidanceArrow.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
        guidanceText.gameObject.SetActive(false);
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

    protected override void Restart()
    {
        if (!UberManager.Instance.Tutorial && !initializedInGame)
        {
            InitializeInGame();
        }

        if (!UberManager.Instance.Tutorial && initializedInGame)
        {
            cities.HandleAction(c => c.Restart());
            if (lastRep != UberManager.Instance.PlayerData.ReputationLevel)
            {
                lastRep = UberManager.Instance.PlayerData.ReputationLevel;
                repUpUI.gameObject.SetActive(true);
                repUpUI.Activate();
            }
            else
            {
                // the rep parent will be updated by repupui if rep up
                reputationParent.SetStars();
            }
        }

        if (UberManager.Instance.Tutorial)
        {
            tutorialPath.Restart();
        }
    }

    public override void Clear()
    {
        if (UberManager.Instance.Tutorial)
        {
            tutorialPath.Clear();
            DeactivateNoClickPanel();
        }
        else if (tutorialPanel.activeInHierarchy)
        {
            tutorialPanel.SetActive(false);
        }

        cities.HandleAction(c => c.Clear());

        base.Clear();
    }

    public void ActivateDialog(string[] conversation)
    {
        dialog.Activate(conversation);
    }
}
