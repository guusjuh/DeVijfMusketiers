using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAPSkipButton : MonoBehaviour {
    private Text playerAPText;
    private Text playerSkipText;

    private Image playerAPBackground;
    private RectTransform rtAP;
    private RectTransform rtSkip;

    private Color normalColorPlayerAP;
    private Color dissabledColorPlayerAP;

    private Button myButton;
    private bool skipOpen = false;

    private float size;
    private float speed = 10.5f;

    private bool active = false;

    public bool Active
    {
        get { return active; }
        set
        {
            if (active == value) return;
            active = value;
        }
    }

    public void Initialize()
    {
        rtAP = transform.Find("AP").GetComponent<RectTransform>();
        rtSkip = transform.Find("Skip").GetComponent<RectTransform>();

        transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(75.0f, 75.0f);

        playerAPText = rtAP.GetComponentInChildren<Text>();
        playerAPBackground = rtAP.GetComponentInChildren<Image>();

        playerSkipText = rtSkip.GetComponentInChildren<Text>();

        normalColorPlayerAP = playerAPBackground.color;
        dissabledColorPlayerAP = Color.grey;
        playerAPBackground.color = dissabledColorPlayerAP;

        myButton = gameObject.GetComponent<Button>();
        myButton.onClick.AddListener(OnClick);

        size = rtAP.sizeDelta.x;
    }

    public void OnClick()
    {
        //only react in the player turn
        if (!GameManager.Instance.LevelManager.PlayersTurn) return;
        
        if (skipOpen)
        {
            SkipTurn();
            return;
        }
        
        StartCoroutine(RotateButton(speed, true)); //rotate the button towards the skip side
    }

    public void CloseSkipButton()
    {
        if (!skipOpen)
        {
            return;
        }
        else
        {
            StartCoroutine(RotateButton(speed, false));
            skipOpen = false;
        }
    }

    private IEnumerator RotateButton(float speed, bool toSkip)
    {
        speed = (toSkip) ? -speed : speed;
        if (toSkip)
        {
            yield return SmoothRotate(rtAP, playerAPText, speed, toSkip);
            myButton.targetGraphic = rtSkip.gameObject.GetComponent<Image>();
        }
        yield return SmoothRotate(rtSkip, playerSkipText, -speed, !toSkip);

        if (!toSkip)
        {
            myButton.targetGraphic = rtAP.gameObject.GetComponent<Image>();
            yield return SmoothRotate(rtAP, playerAPText, speed, toSkip);
        }

        skipOpen = toSkip;
    }

    private IEnumerator SmoothRotate(RectTransform rt, Text t, float speed, bool grow)
    {
        float scaledSize = 0.0f;

        while (scaledSize < size)
        {
            if (scaledSize + Mathf.Abs(speed) <= size)
            {
                scaledSize += Mathf.Abs(speed);
                float normalized = (grow) ? 1.0f - scaledSize / size : scaledSize / size;
                
                rt.sizeDelta += new Vector2(speed, 0.0f);
                t.transform.localScale = new Vector3(normalized, 1, 1);
            }
            else
            {
                rt.sizeDelta = new Vector2((grow)? 0.0f : size, rt.sizeDelta.y);
                t.transform.localScale = new Vector3((grow) ? 0.0f : 1.0f, 1, 1);
                scaledSize = size;
                break;
            }
            yield return null;
        }
    }

    private void SkipTurn()
    {
        GameManager.Instance.LevelManager.SkipPlayerTurn();
        CloseSkipButton();
    }

    public void SetAPText()
    {
        playerAPText.text = GameManager.Instance.LevelManager.Player.CurrentActionPoints + "";

        if (GameManager.Instance.LevelManager.Player.CurrentActionPoints <= 0)
            playerAPBackground.color = dissabledColorPlayerAP;
        else
            playerAPBackground.color = normalColorPlayerAP;
    }
}
