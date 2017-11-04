using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReputationUpUI : MonoBehaviour
{
    private GameObject filledStarParent;

    private List<RectTransform> filledStars;
    private List<Vector2> originalScales;

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
        gameObject.SetActive(true);

        int level = UberManager.Instance.PlayerData.ReputationLevel;

        for (int i = 0; i < level; i++)
        {
            filledStars[i].gameObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        gameObject.SetActive(false);
    }
}
