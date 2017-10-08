using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameUIManager : SubUIManager {
    private RectTransform anchorCenter;
    private RectTransform anchorTopRight;
    private RectTransform anchorBottomRight;

    private GameObject backButton;
    private PreGameInfoPanel preGameInfoPanel;
    public PreGameInfoPanel PreGameInfoPanel { get { return preGameInfoPanel; } }

    private GameObject startGameButton;
    private GameObject newHumanButton;

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("PreGameCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopRight = canvas.gameObject.transform.Find("Anchor_TopRight").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();

        backButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/PreGame/BackButton"), Vector3.zero, Quaternion.identity,
                anchorTopRight.transform);
        backButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(75, -75);

        preGameInfoPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/PreGame/PreGameInfoPanel"), Vector3.zero, Quaternion.identity,
                anchorCenter.transform).GetComponent<PreGameInfoPanel>();
        preGameInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        preGameInfoPanel.Initialize();

        GameObject buttonParent = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, anchorBottomRight);
        buttonParent.AddComponent<RectTransform>();
        buttonParent.GetComponent<RectTransform>().sizeDelta = new Vector2(600.0f, 100.0f);
        buttonParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300.0f, 0.0f);

        startGameButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Button"), Vector3.zero, Quaternion.identity,
                buttonParent.transform);
        startGameButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(175.0f, 0.0f);
        startGameButton.GetComponentInChildren<Text>().text = "Start level";
        startGameButton.GetComponent<Button>().onClick.AddListener(StartGame);
        startGameButton.GetComponent<Button>().interactable = false;
    }

    protected override void Restart()
    {
        preGameInfoPanel.Restart();
    }

    public override void Clear()
    {
        preGameInfoPanel.Clear();

        base.Clear();
    }

    public void CanStart(bool canStart)
    {
        startGameButton.GetComponent<Button>().interactable = canStart;
    }

    public void StartGame()
    {
        UberManager.Instance.GameManager.SetLevelInfo(UberManager.Instance.PreGameManager.SelectedLevel,
            preGameInfoPanel.GetSelectedContracts());

        preGameInfoPanel.GetSelectedContracts().HandleAction(c => c.SetActive(true));//, UberManager.Instance.PreGameManager.SelectedLevel));

        UberManager.Instance.GotoState(UberManager.GameStates.InGame);
    }
}
