using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameUIManager : SubUIManager
{
    private RectTransform anchorCenter;
    private RectTransform anchorTopMid;
    private RectTransform anchorBottomRight;

    private GameObject postGameInfoPanel;
    private GameObject backButton;
    private GameObject nextButton;

    public override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("PostGameCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();

        postGameInfoPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/PostGame/PostGameInfoPanel"), Vector3.zero, Quaternion.identity,
                anchorCenter.transform);
        postGameInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        
        //TODO: create class for postgame info panel that sets all info
        //TODO: set victory/defeat text
        //TODO: set amount of hoomans lost

        GameObject buttonParent = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, anchorBottomRight);
        buttonParent.AddComponent<RectTransform>();
        buttonParent.GetComponent<RectTransform>().sizeDelta = new Vector2(600.0f, 100.0f);
        buttonParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300.0f, 0.0f);

        backButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Button"), Vector3.zero, Quaternion.identity,
                buttonParent.transform);
        backButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(175.0f, 0.0f);
        backButton.GetComponentInChildren<Text>().text = "Back to world";

        nextButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Button"), Vector3.zero, Quaternion.identity,
                buttonParent.transform);
        nextButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-175.0f, 0.0f);
        nextButton.GetComponentInChildren<Text>().text = "Next level";

        //TODO: actually check if the next level button should be deactivated
        //TODO: button script that handles active state 
        nextButton.GetComponent<Button>().interactable = false;
    }

    public override void Restart()
    {

    }

    public override void Clear()
    {

    }
}
