using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUIManager : SubUIManager
{
    private RectTransform anchorCenter;
    private GameObject levelSelectPanel;

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("LevelSelectCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();

        //TODO: level select panel script... or do we?
        levelSelectPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/LevelSelect/LevelSelectPanel"), Vector3.zero, Quaternion.identity,
                anchorCenter.transform);
        levelSelectPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
}
