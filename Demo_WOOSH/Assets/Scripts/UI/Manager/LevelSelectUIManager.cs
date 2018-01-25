using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUIManager : SubUIManager
{
    public RectTransform AnchorCenter {get { return anchorCenter; } }

    private GameObject levelSelectPanel;

    private ReputationUIManager reputationParent;
    public ReputationUIManager ReputationParent { get { return reputationParent; } }

    private GameObject settingsButton;
    private SettingsMenu settingsMenu;
    public SettingsMenu SettingsMenu { get { return settingsMenu; } }

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
    public void UpdateLastRep() { lastRep = UberManager.Instance.PlayerData.ReputationLevel; }

    private GameObject background;

    //------------------ TUTORIAL VARS ------------------------
    private GameObject tutorialPanel;
    private GameObject tutorialBackground;

    private GameObject guidanceArrow;
    private Text guidanceText;

    private bool deadContract = false;
    private bool inDialog = false;
    private string[] deadContractDialog = new string[4];

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
        canvasName = "LevelSelectCanvas";
        base.Initialize();

        background = UIManager.Instance.CreateUIElement(Resources.Load<GameObject>("Prefabs/UI/LevelSelect/BackgroundLevelSelect"), Vector2.zero, canvas.transform);

        background.transform.SetAsFirstSibling();

        dialog = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/DialogPopup", Vector2.zero, anchorBottomCenter).GetComponent<DialogScript>();
        dialog.Initialize();

        FinishInitialize();
    }

    private void InitializeContractLeftDialog()
    {
        deadContractDialog = new string[4];
        deadContractDialog[0] = "Human: THIS IS ENOUGH!";
        deadContractDialog[1] = "Human: You've let me get caught by monsters again and again!";
        deadContractDialog[2] = "Human: I'll find a wizard who can get me across safely!";
        deadContractDialog[3] = "Human: goodbye sir!";
    }

    private void InitializeContractAngryDialog()
    {
        deadContractDialog = new string[10];
        deadContractDialog[0] = "Wizard's hat: Oh watch out!";
        deadContractDialog[1] = "Wizard's hat: One of your humans is sad.";
        deadContractDialog[2] = "Wizard's hat: If he dies once more...";
        deadContractDialog[3] = "Wizard's hat: He’ll leave you for another wizard!";
        deadContractDialog[4] = "Wizard's hat: You can make him happier though...";
        deadContractDialog[5] = "Wizard's hat: Just make sure he survives the level!";
        deadContractDialog[6] = "Wizard's hat: You can recognize the saddest humans";
        deadContractDialog[7] = "Wizard's hat: just look at their face!";
        deadContractDialog[8] = "Wizard's hat: You have to watch out if...";
        deadContractDialog[9] = "Wizard's hat: he looks red and is crying...";
    }

    public void SetUpDeadContractDialog(bool contractLeft)
    {
        deadContract = true;

        if (contractLeft)
            InitializeContractLeftDialog();
        else
            InitializeContractAngryDialog();

    }

    public void DeactivateDialog()
    {
        GameObject.Find("LevelSelectScrollView(Clone)").GetComponent<ScrollRect>().vertical = true;
        UnblockLevelButtons(true);
        inDialog = false;
    }

    protected override void InitializeTutorial()
    {
        tutorialPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/TutorialPanel", Vector2.zero, anchorCenter);

        GameObject selectContractWindowGO = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/SelectContractMenuPanel", Vector2.zero, canvas.transform);
        tutorialSelectContractWindow = new SelectContractWindow(selectContractWindowGO.transform.GetChild(0).gameObject);

        tutorialCity = tutorialPanel.GetComponentInChildren<City>();
        tutorialCity.Initiliaze();

        tutorialCity.Reached();

        tutorialPath = tutorialCity.Paths[0];

        wizardsHat = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/Hat", new Vector2(0, -50.0f), tutorialPanel.transform).gameObject;
        wizardsHat.transform.SetSiblingIndex(1);

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

        reputationParent = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/ReputationParent", new Vector2(-20, -20), anchorTopCenter).GetComponent<ReputationUIManager>();
        reputationParent.Initialize();

        settingsMenu = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/SettingsMenuPanel", Vector2.zero, canvas.transform).GetComponent<SettingsMenu>();
        settingsMenu.Initialize();

        settingsButton = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/SettingsButton", new Vector2(20, -20), anchorTopCenter).gameObject;
        settingsButton.GetComponentInChildren<Button>().onClick.AddListener(settingsMenu.Activate);

        GameObject selectContractWindowGO = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/SelectContractMenuPanel", Vector2.zero, canvas.transform);
        selectContractWindow = new SelectContractWindow(selectContractWindowGO.transform.GetChild(0).gameObject);

        repUpUI = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/RepUpPanel", Vector2.zero, canvas.transform).GetComponent<ReputationUpUI>();
        repUpUI.Initialze();
        lastRep = UberManager.Instance.PlayerData.ReputationLevel;

        initializedInGame = true;
    }

    public override void Start()
    {
        base.Start();
        if (deadContract)
        {
            deadContract = false;
            StartDialog(deadContractDialog);
        }
    }

    public override void Update()
    {
        if (inDialog)
            UberManager.Instance.InputManager.CatchInput();
    }

    public override void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, float width = 100, float height = 100)
    {
        base.ActivateNoClickPanel(onlyButtonPos, buttonSprite, width, height);

        tutorialIndicator.SetState(City.ContractState.New);
    }

    public override void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, Vector2 size)
    {
        base.ActivateNoClickPanel(onlyButtonPos, buttonSprite, size.x, size.y);

        tutorialIndicator.SetState(City.ContractState.New);
    }

    public void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, bool indicatorOn, float width = 100, float height = 100)
    {
        base.ActivateNoClickPanel(onlyButtonPos, buttonSprite, width, height);

        if (indicatorOn) tutorialIndicator.SetState(City.ContractState.New);
        else tutorialIndicator.SetState(City.ContractState.Nothing);
    }

    public void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, bool indicatorOn, Vector2 size)
    {
        ActivateNoClickPanel(onlyButtonPos, buttonSprite, indicatorOn, size.x, size.y);
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

    protected void StartDialog(string[] conversation)
    {
        dialog.Activate(conversation);
        UberManager.Instance.InputManager.ListenForDialog();
        GameObject.Find("LevelSelectScrollView(Clone)").GetComponent<ScrollRect>().vertical = false;

        inDialog = true;

        UnblockLevelButtons(false);
    }

    public void UnblockLevelButtons(bool value)
    {
        for (int i = 0; i < cities.Count; i++)
        {
            cities[i].ChangeLevelInteractability(value, true);
        }
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
            if (lastRep < UberManager.Instance.PlayerData.ReputationLevel)
            {
                repUpUI.gameObject.SetActive(true);
                repUpUI.Activate();
            }
            else
            {
                // the rep parent will be updated by repupui if rep up
                reputationParent.SetStars();
            }
            lastRep = UberManager.Instance.PlayerData.ReputationLevel;
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
        else {
            if (tutorialPanel != null && tutorialPanel.activeInHierarchy) { 
                tutorialPanel.SetActive(false);
            }
            if (tutorialBackground != null)
            {
                GameObject.Destroy(tutorialBackground);
                tutorialBackground = null;
            }
        }

        cities.HandleAction(c => c.Clear());

        base.Clear();
    }

    public void ActivateDialog(string[] conversation)
    {
        dialog.Activate(conversation);
    }
}
