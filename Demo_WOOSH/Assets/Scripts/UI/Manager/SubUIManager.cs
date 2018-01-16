using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubUIManager
{
    protected string canvasName = "Canvas";
    protected Canvas canvas;
    protected RectTransform anchorCenter;
    protected RectTransform anchorTopCenter;
    protected RectTransform anchorTopRight;
    protected RectTransform anchorBottomCenter;
    protected RectTransform anchorBottomRight;
    protected RectTransform anchorBottomLeft;

    public Canvas Canvas { get { return canvas; } }

    protected bool first = true;

    protected RectTransform noClickPanel;
    private Color panelDissabledColor = new Color(0.37f, 0.37f, 0.37f, 0.57f);

    protected Button onlyButton;
    public Button OnlyButton { get { return onlyButton; } }

    protected Button clickToContinue;
    protected NewContractIndicator tutorialIndicator;

    protected bool initializedInGame = false;

    public void Start()
    {
        if (first)
        {
            Initialize();
            first = false;
        }
        else
        {
            Restart();
        }

        canvas.gameObject.SetActive(true);
    }

    protected virtual void Initialize()
    {
        SetUpAnchors();
    }

    private void SetUpAnchors()
    {
        canvas = GameObject.FindGameObjectWithTag(canvasName).GetComponent<Canvas>();
        anchorCenter = GetAnchor("Anchor_Center");
        anchorTopCenter = GetAnchor("Anchor_TopCenter");
        anchorTopRight = GetAnchor("Anchor_TopRight");
        anchorBottomCenter = GetAnchor("Anchor_BottomCenter");
        anchorBottomRight = GetAnchor("Anchor_BottomRight");
        anchorBottomLeft = GetAnchor("Anchor_BottomLeft");
    }

    private RectTransform GetAnchor(string anchorName)
    {
        Transform gO = canvas.gameObject.transform.Find(anchorName);
        if (gO != null)
        {
            return gO.GetComponent<RectTransform>();
        }
        return null;
    }

    protected virtual void InitializeTutorial() { }
    protected virtual void InitializeInGame() { }

    protected virtual void Restart() { }

    public virtual void Update() { }

    public virtual void Clear()
    {
        canvas.gameObject.SetActive(false);
    }

    public virtual void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, float width = 100, float height = 100)
    {
        noClickPanel.gameObject.SetActive(true);
        noClickPanel.GetComponent<Image>().color = panelDissabledColor;

        onlyButton.gameObject.SetActive(true);
        clickToContinue.gameObject.SetActive(false);

        onlyButton.GetComponent<Image>().sprite = buttonSprite;
        onlyButton.GetComponent<RectTransform>().anchoredPosition = onlyButtonPos;
        onlyButton.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        onlyButton.onClick.AddListener(UberManager.Instance.TutorialManager.Next);
        onlyButton.onClick.AddListener(UberManager.Instance.SoundManager.PlaySoundEffect);
    }

    public virtual void ActivateNoClickPanel(Vector2 onlyButtonPos, Sprite buttonSprite, Vector2 size)
    {
        ActivateNoClickPanel(onlyButtonPos, buttonSprite, size.x, size.y);
    }

    public void ActivateNoClickPanel()
    {
        noClickPanel.gameObject.SetActive(true);
        noClickPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        onlyButton.gameObject.SetActive(false);
        clickToContinue.gameObject.SetActive(true);

        clickToContinue.onClick.AddListener(UberManager.Instance.TutorialManager.Next);
        clickToContinue.onClick.AddListener(UberManager.Instance.SoundManager.PlaySoundEffect);
    }

    public virtual void DeactivateNoClickPanel()
    {
        noClickPanel.gameObject.SetActive(false);
        onlyButton.gameObject.SetActive(false);
        clickToContinue.gameObject.SetActive(false);

        onlyButton.onClick.RemoveAllListeners();
        clickToContinue.onClick.RemoveAllListeners();
    }

    public Vector2 WorldToCanvas(Vector3 worldPosition)
    {
        Camera camera = Camera.main;

        var viewportPos = camera.WorldToViewportPoint(worldPosition);
        var canvasRect = canvas.GetComponent<RectTransform>();

        return new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
    }
}
