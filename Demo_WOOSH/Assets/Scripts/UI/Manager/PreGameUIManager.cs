using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameUIManager : SubUIManager {
    private RectTransform anchorCenter;
    private RectTransform anchorTopRight;
    private RectTransform anchorBottomRight;

    private PreGameInfoPanel preGameInfoPanel;
    public PreGameInfoPanel PreGameInfoPanel { get { return preGameInfoPanel; } }

    private GameObject startButton;
    private GameObject backButton;

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("PreGameCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopRight = canvas.gameObject.transform.Find("Anchor_TopRight").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();

        preGameInfoPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/PreGame/PreGameInfoPanel", Vector2.zero, anchorCenter).GetComponent<PreGameInfoPanel>();
        preGameInfoPanel.Initialize();

        GameObject buttonParent = UIManager.Instance.CreateUIElement(new Vector2(-300.0f, 0.0f), new Vector2(600.0f, 100.0f), anchorBottomRight);

        startButton = UIManager.Instance.CreateUIElement("Prefabs/UI/Button", new Vector2(175.0f, 0.0f), buttonParent.transform);
        startButton.GetComponentInChildren<Text>().text = "Start level";
        startButton.GetComponent<Button>().onClick.AddListener(StartGame);
        startButton.GetComponent<Button>().interactable = false;

        backButton = UIManager.Instance.CreateUIElement("Prefabs/UI/PreGame/BackButton", new Vector2(75, -75), anchorTopRight);
    }

    protected override void Restart()
    {
        preGameInfoPanel.Restart();
    }

    public override void Clear()
    {
        preGameInfoPanel.Clear();

        CanStart(false);

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

        UberManager.Instance.GotoState(UberManager.GameStates.InGame);
    }
}
