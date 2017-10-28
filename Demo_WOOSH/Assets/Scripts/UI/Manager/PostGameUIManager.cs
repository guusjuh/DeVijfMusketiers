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

    //------------------ TUTORIAL VARS ------------------------
    private GameObject guidanceArrow;
    private Text guidanceText;
    //---------------------------------------------------------

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
        noClickPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/NoClickPanel", Vector2.zero, canvas.transform).GetComponent<RectTransform>();
        onlyButton = noClickPanel.Find("OnlyButton").GetComponent<Button>();
        clickToContinue = noClickPanel.Find("ClickToContinue").GetComponent<Button>();
        onlyButton.gameObject.SetActive(false);
        clickToContinue.gameObject.SetActive(false);

        backButton.GetComponent<Button>().onClick.AddListener(UberManager.Instance.TutorialManager.Next);

        guidanceArrow = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceArrow", Vector2.zero, canvas.transform);
        guidanceText = UIManager.Instance.CreateUIElement("Prefabs/UI/Tutorial/GuidanceText", Vector2.zero, canvas.transform).GetComponent<Text>();
    }

    protected override void InitializeInGame()
    {
        
    }

    public override void DeactivateNoClickPanel()
    {
        base.DeactivateNoClickPanel();

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
        postGameInfoPanel.Restart();
    }

    public override void Clear()
    {
        postGameInfoPanel.Clear();
        if (UberManager.Instance.Tutorial) DeactivateNoClickPanel();
        base.Clear();
    }

    public void BackToWorld()
    {
        UberManager.Instance.GotoState(UberManager.GameStates.LevelSelection);
    }
}
