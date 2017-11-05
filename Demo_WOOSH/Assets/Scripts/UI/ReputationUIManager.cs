using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReputationUIManager : MonoBehaviour
{
    private List<Image> stars = new List<Image>();

    private Sprite emptyStar;
    private Sprite fullStar;

    public void Initialize()
    {
        emptyStar = Resources.Load<Sprite>("Sprites/UI/Stars/GreyStar");
        fullStar = Resources.Load<Sprite>("Sprites/UI/Stars/YellowStar");
        stars = new List<Image>(transform.GetComponentsInChildren<Image>());
        SetStars();
    }

    public void SetStars()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < UberManager.Instance.PlayerData.ReputationLevel) stars[i].sprite = fullStar;
            else stars[i].sprite = emptyStar; 
        }
    }
}
