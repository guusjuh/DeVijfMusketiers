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

        postGameInfoPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/PostGame/PostGameInfoPanel"), Vector3.zero, Quaternion.identity,
                anchorCenter.transform).GetComponent<PostGameInfoPanel>();
        postGameInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        postGameInfoPanel.Initialize();

        GameObject buttonParent = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, anchorBottomRight);
        buttonParent.AddComponent<RectTransform>();
        buttonParent.GetComponent<RectTransform>().sizeDelta = new Vector2(600.0f, 100.0f);
        buttonParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300.0f, 0.0f);

        backButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Button"), Vector3.zero, Quaternion.identity,
                buttonParent.transform);
        backButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(175.0f, 0.0f);
        //TODO: change text tó 'back to levelselection' as soon as that state exists
        backButton.GetComponentInChildren<Text>().text = "Replay level";
        backButton.GetComponent<Button>().onClick.AddListener(BackToWorld);

        nextButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Button"), Vector3.zero, Quaternion.identity,
                buttonParent.transform);
        nextButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-175.0f, 0.0f);
        nextButton.GetComponentInChildren<Text>().text = "Next level";

        //TODO: actually check if the next level button should be deactivated
        //TODO: button script that handles active state 
        nextButton.GetComponent<Button>().interactable = false;
    }

    protected override void Restart()
    {
        postGameInfoPanel.Initialize();

        //TODO: actually check if the next level button should be deactivated
        //TODO: button script that handles active state 
        nextButton.GetComponent<Button>().interactable = false;
    }

    public override void Clear()
    {
        base.Clear();
    }

    public void BackToWorld()
    {
        UberManager.Instance.GotoState(UberManager.GameStates.InGame);
    }
}
