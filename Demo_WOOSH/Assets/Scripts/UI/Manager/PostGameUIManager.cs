using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameUIManager : SubUIManager
{
    private RectTransform anchorCenter;
    private RectTransform anchorTopMid;
    private RectTransform anchorBottomRight;

    private PostGameInfoPanel postGameInfoPanel;
    private GameObject backButton;
    private GameObject nextButton;

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("PostGameCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();

        postGameInfoPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/PostGame/PostGameInfoPanel", Vector2.zero, anchorCenter).GetComponent<PostGameInfoPanel>();
        postGameInfoPanel.Initialize();

        GameObject buttonParent = UIManager.Instance.CreateUIElement(new Vector2(-300.0f, 0.0f), new Vector2(600.0f, 100.0f), anchorBottomRight);

        backButton = UIManager.Instance.CreateUIElement("Prefabs/UI/Button", new Vector2(175.0f, 0.0f), buttonParent.transform);
        backButton.GetComponentInChildren<Text>().text = "Back to level select";
        backButton.GetComponent<Button>().onClick.AddListener(BackToWorld);
    }

    protected override void Restart()
    {
        postGameInfoPanel.Restart();
    }

    public override void Clear()
    {
        postGameInfoPanel.Clear();
        base.Clear();
    }

    public void BackToWorld()
    {
        UberManager.Instance.GotoState(UberManager.GameStates.LevelSelection);
    }
}
