using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAPSkipButton : IUIElement {
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
    private float speed = 15.0f;

    private Coroutine rotateCoroutine;

    public override void Initialize()
    {
        rtAP = transform.Find("AP").GetComponent<RectTransform>();
        rtSkip = transform.Find("Skip").GetComponent<RectTransform>();

        transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(125.0f, 125.0f);

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

    public void Reset()
    {
        gameObject.SetActive(true);

        playerAPBackground.color = dissabledColorPlayerAP;

        rtAP.sizeDelta = new Vector2(size, size);
        rtSkip.sizeDelta = new Vector2(0, size);
        playerAPText.transform.localScale = new Vector3(1,1,1);
        playerSkipText.transform.localScale = new Vector3(0,1,1);
        skipOpen = false;
        rotateCoroutine = null;
    }

    public void OnClick()
    {
        //only react in the player turn
        if (!GameManager.Instance.LevelManager.PlayersTurn) return;

        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        
        if (skipOpen)
        {
            SkipTurn();
            return;
        }
        if (rotateCoroutine == null)
        {
            rotateCoroutine = StartCoroutine(RotateButton(speed, true)); //rotate the button towards the skip side
        }
    }

    public void CloseSkipButton()
    {
        if (!skipOpen)
        {
            return;
        }
        else
        {
            if (rotateCoroutine == null)
            {
                rotateCoroutine = StartCoroutine(RotateButton(speed, false));
            }
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
        rotateCoroutine = null;
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
            yield return new WaitForEndOfFrame();
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
