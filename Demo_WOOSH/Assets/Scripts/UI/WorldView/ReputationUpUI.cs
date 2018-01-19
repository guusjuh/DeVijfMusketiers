﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReputationUpUI : MonoBehaviour
{
    private GameObject filledStarParent;

    private List<RectTransform> filledStars;
    private List<Vector2> originalScales;

    private const float START_SIZE = 3000.0f;
    private const float SHRINK_SPEED = 35.0f;
    private const float MOVE_SPEED = 15.0f;
    private const float TRANS_SPEED = 0.01f;

    private bool done = false;

    public void Initialze()
    {
        filledStarParent = transform.Find("FilledStarParent").gameObject;
        filledStars = new List<RectTransform>(filledStarParent.transform.GetComponentsInChildren<RectTransform>());
        filledStars.RemoveAt(0);        // remove self

        originalScales = new List<Vector2>();
        for (int i = 0; i < filledStars.Count; i++)
        {
            originalScales.Add(filledStars[i].sizeDelta);
            if (i != 0) filledStars[i].gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    public void Activate()
    {
        int level = UberManager.Instance.PlayerData.ReputationLevel;

        for (int i = 0; i < level; i++)
        {
            if (i == level - 1)
            {
                UberManager.Instance.StartCoroutine(ScaleDown(i));
            }

            filledStars[i].gameObject.SetActive(true);
        }
    }

    private IEnumerator ScaleDown(int indexStar)
    {
        done = false;

        float currentSize = START_SIZE;
        float endSize = originalScales[indexStar].x;

        filledStars[indexStar].sizeDelta = new Vector2(START_SIZE, START_SIZE);

        while (currentSize > endSize)
        {
            currentSize -= SHRINK_SPEED;
            filledStars[indexStar].sizeDelta = new Vector2(currentSize, currentSize);

            yield return new WaitForEndOfFrame();
        }

        filledStars[indexStar].sizeDelta = new Vector2(endSize, endSize);

        yield return UberManager.Instance.StartCoroutine(MoveToRepBar(indexStar));

        done = true;

        yield return null;
    }

    private IEnumerator MoveToRepBar(int indexStar)
    {
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = new Vector2(300, 960);
        Vector2 direction = (endPos - startPos).normalized; 

        Vector2 currentPos = startPos;
        RectTransform tempStar = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/ExtraStar",
            currentPos, UIManager.Instance.LevelSelectUI.AnchorCenter).GetComponent<RectTransform>();

        Image tempStarImag = tempStar.GetComponent<Image>();

        while ((currentPos - endPos).magnitude > 1.0f && currentPos.x < endPos.x & currentPos.y < endPos.y)
        {
            currentPos += direction * MOVE_SPEED;
            tempStar.anchoredPosition = currentPos;
            tempStarImag.color -= new Color(0, 0, 0, TRANS_SPEED);
            yield return new WaitForEndOfFrame();
        }

        Destroy(tempStar.gameObject);
        UIManager.Instance.LevelSelectUI.ReputationParent.SetStars();

        yield return null;
    }

    public void OnClick()
    {
        if (!done) return;
        filledStars.HandleAction(s => s.gameObject.SetActive(false));
        gameObject.SetActive(false);
    }
}
