﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReputationUIManager : MonoBehaviour
{
    private List<Image> stars = new List<Image>();

    public void Initialize()
    {
        stars = new List<Image>(transform.GetComponentsInChildren<Image>());
        SetStars();
    }

    public void SetStars()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < UberManager.Instance.PlayerData.ReputationLevel) stars[i].color = Color.white;
            else stars[i].color = Color.black; 
        }
    }
}
