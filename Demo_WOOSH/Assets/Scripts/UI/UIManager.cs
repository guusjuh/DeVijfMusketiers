using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private EnemyInfoUI enemyInfoUI;
    public EnemyInfoUI EnemyInfoUI { get { return enemyInfoUI; } }

    private Text playerAPText;

    private Canvas levelCanvas;
    private RectTransform anchorCenter;
    private RectTransform anchorTopMid;
    private RectTransform anchorBottomRight;

    private GameObject playerTurnBanner;
    private GameObject enemyTurnBanner;

    public void Initialize()
    {
        levelCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

        anchorCenter = levelCanvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorTopMid = levelCanvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();
        anchorBottomRight = levelCanvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();

        enemyInfoUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/EnemyInfo"), Vector3.zero, Quaternion.identity, anchorTopMid.transform).GetComponent<EnemyInfoUI>();
        enemyInfoUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -50.0f, 0.0f);
        enemyInfoUI.Initialize();
       
        playerAPText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlayerActionPoints"), Vector3.zero, Quaternion.identity, anchorBottomRight.transform).transform.GetComponentInChildren<Text>();
        playerAPText.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(-10.0f, 10.0f, 0.0f);

        playerTurnBanner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/YourTurn"), Vector3.zero, Quaternion.identity, anchorCenter.transform);
        playerTurnBanner.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        playerTurnBanner.SetActive(false);

        enemyTurnBanner = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/OthersTurn"), Vector3.zero, Quaternion.identity, anchorCenter.transform);
        enemyTurnBanner.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        enemyTurnBanner.SetActive(false);
    }

    public IEnumerator StartTurn(bool player)
    {
        if (player) playerTurnBanner.SetActive(true);
        else enemyTurnBanner.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        if (player) playerTurnBanner.SetActive(false);
        else enemyTurnBanner.SetActive(false);
    }

}
