using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUIManager : SubUIManager
{
    private RectTransform anchorCenter;
    private RectTransform anchorBottomRight;
    private RectTransform anchorTopMid;
    private RectTransform anchorBottomCenter;

    private GameObject levelSelectPanel;

    private ReputationUIManager reputationParent;

    private SelectContractWindow selectContractWindow;
    public SelectContractWindow SelectContractWindow { get { return selectContractWindow;} }

    private List<City> cities;
    public List<City> Cities { get { return cities; } }

    //------------------ TUTORIAL VARS ------------------------
    private GameObject tutorialPanel;
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
        tutorialPath = new Path(tutorialPanel.transform.Find("Path"), new City(), -1, Destination.Red);
        tutorialPath.SpawnContract(UberManager.Instance.ContractManager.GenerateRandomContract(tutorialPath));

        wizardsHat = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/Hat", Vector2.zero, anchorCenter).gameObject;

        dialog = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/DialogPopup", Vector2.zero, anchorBottomCenter).GetComponent<DialogScript>();
        dialog.Initialize();

        noClickPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/NoClickPanel", Vector2.zero, canvas.transform).GetComponent<RectTransform>();
        onlyButton = noClickPanel.Find("OnlyButton").GetComponent<Button>();
        clickToContinue = noClickPanel.Find("ClickToContinue").GetComponent<Button>();
        onlyButton.gameObject.SetActive(false);
        clickToContinue.gameObject.SetActive(false);
        tutorialIndicator = new NewContractIndicator(onlyButton.transform.Find("Indicator").gameObject);
        tutorialIndicator.SetState(City.ContractState.Nothing);

        ActivateNoClickPanel(wizardsHat.GetComponent<RectTransform>().anchoredPosition, wizardsHat.GetComponentInChildren<Image>().sprite);
    }

    protected override void InitializeInGame()
    {
        levelSelectPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/LevelSelectPanel", Vector2.zero, anchorCenter.transform);
        cities = new List<City>(levelSelectPanel.GetComponentsInChildren<City>());
        cities.HandleAction(c => c.Initiliaze());

        reputationParent = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/ReputationParent", new Vector2(-20, -20), anchorTopMid.transform).GetComponent<ReputationUIManager>();
        reputationParent.Initialize();

        selectContractWindow = new SelectContractWindow(levelSelectPanel.transform.Find("SelectContractMenu").gameObject);

        initializedInGame = true;
    }

    public override void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, float width = 100, float height = 100)
    {
        base.ActivateNoClickPanel(onlyButtonPos, buttonSprite);

        tutorialIndicator.SetState(City.ContractState.New);
    }

    public override void DeactivateNoClickPanel()
    {
        base.DeactivateNoClickPanel();

        tutorialIndicator.SetState(City.ContractState.Nothing);
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
            reputationParent.SetStars();
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
        }

        cities.HandleAction(c => c.Clear());

        DeactivateNoClickPanel();

        base.Clear();
    }

    public void ActivateDialog(string[] conversation)
    {
        dialog.Activate(conversation);
    }
}
