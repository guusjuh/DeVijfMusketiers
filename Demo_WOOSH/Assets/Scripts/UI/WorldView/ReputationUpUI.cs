using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReputationUpUI : MonoBehaviour
{
    private GameObject filledStarParent;

    private List<RectTransform> filledStars;
    private List<Vector2> originalScales;

    private float startSize = 3000.0f;
    private float shrinkSize = 35.0f;
    private bool doneShrinking = false;

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
        doneShrinking = false;

        float currentSize = startSize;
        float endSize = originalScales[indexStar].x;

        filledStars[indexStar].sizeDelta = new Vector2(startSize, startSize);

        while (currentSize > endSize)
        {
            currentSize -= shrinkSize;
            filledStars[indexStar].sizeDelta = new Vector2(currentSize, currentSize);

            yield return new WaitForEndOfFrame();
        }

        filledStars[indexStar].sizeDelta = new Vector2(endSize, endSize);
        doneShrinking = true;

        yield return null;
    }

    public void OnClick()
    {
        if (!doneShrinking) return;
        gameObject.SetActive(false);
    }
}
