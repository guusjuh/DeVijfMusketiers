using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameUIManager : SubUIManager {
    private PreGameInfoPanel preGameInfoPanel;
    public PreGameInfoPanel PreGameInfoPanel { get { return preGameInfoPanel; } }

    private VersusPanel versusPanel;

    private GameObject startButton;
    public RectTransform StartButton { get { return startButton.GetComponent<RectTransform>(); } }
    private GameObject backButton;
    public Vector2 BackButtonPos { get { return backButton.GetComponent<RectTransform>().anchoredPosition; } }

    //------------------ TUTORIAL VARS ------------------------
    private GameObject guidanceArrow;
    private Text guidanceText;
    //---------------------------------------------------------

    protected override void Initialize()
    {
        canvasName = "PreGameCanvas";
        base.Initialize();

        preGameInfoPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/PreGame/PreGameInfoPanel", Vector2.zero, anchorCenter).GetComponent<PreGameInfoPanel>();
        preGameInfoPanel.Initialize();

        GameObject buttonParent = UIManager.Instance.CreateUIElement(new Vector2(-275.0f, 10.0f), new Vector2(600.0f, 100.0f), anchorBottomRight);

        startButton = UIManager.Instance.CreateUIElement("Prefabs/UI/StartLevelButton", new Vector2(175.0f, 0.0f), buttonParent.transform);

        backButton = UIManager.Instance.CreateUIElement("Prefabs/UI/PreGame/BackButton", new Vector2(75, -75), anchorTopRight);
        backButton.GetComponent<Button>().onClick.AddListener(BackToLevelSelect);

        GameObject background = UIManager.Instance.CreateUIElement(Resources.Load<GameObject>("Prefabs/UI/PreGame/BackgroundPreGame"), Vector2.zero, canvas.transform);

        background.transform.SetAsFirstSibling();

        FinishInitialize();
    }

    protected override void InitializeTutorial()
    {
        noClickPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/NoClickPanel", Vector2.zero, canvas.transform).GetComponent<RectTransform>();
        onlyButton = noClickPanel.Find("OnlyButton").GetComponent<Button>();
        clickToContinue = noClickPanel.Find("ClickToContinue").GetComponent<Button>();
        onlyButton.gameObject.SetActive(false);
        clickToContinue.gameObject.SetActive(false);

        backButton.gameObject.SetActive(false);

        Button startButtonComponent = startButton.GetComponent<Button>();
        startButtonComponent.onClick.AddListener(StartGame);
        startButtonComponent.interactable = false;

        guidanceArrow = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceArrow", Vector2.zero, canvas.transform);
        guidanceText = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceText", Vector2.zero, canvas.transform).GetComponent<Text>();
    }

    protected override void InitializeInGame()
    {
        versusPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/PreGame/VersusPanel", Vector2.zero, canvas.transform).GetComponent<VersusPanel>();
        versusPanel.Initialize();

        Button startButtonComponent = startButton.GetComponent<Button>();
        startButtonComponent.onClick.RemoveAllListeners();
        startButtonComponent.onClick.AddListener(versusPanel.Activate);
        startButtonComponent.interactable = false;

        backButton.gameObject.SetActive(true);
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

    protected override void Restart()
    {
        if (!UberManager.Instance.Tutorial && !initializedInGame)
        {
            InitializeInGame();
        }

        preGameInfoPanel.Restart();
    }

    public override void Clear()
    {
        preGameInfoPanel.Clear();

        CanStart(false);

        if (UberManager.Instance.Tutorial) DeactivateNoClickPanel();

        base.Clear();
    }

    public void CanStart(bool canStart)
    {
        startButton.GetComponent<Button>().interactable = canStart;
    }

    public void StartGame()
    {
        UberManager.Instance.GameManager.SetLevelInfo(UberManager.Instance.PreGameManager.SelectedLevel,
            preGameInfoPanel.GetSelectedContracts());

        preGameInfoPanel.GetSelectedContracts().HandleAction(c => c.SetActive(true));

        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        UberManager.Instance.GotoState(UberManager.GameStates.InGame);
    }

    public override void Update()
    {
        base.Update();

        versusPanel.UpdateTimer();
    }

    public void BackToLevelSelect()
    {
        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        UberManager.Instance.GotoState(UberManager.GameStates.LevelSelection);
    }
}
