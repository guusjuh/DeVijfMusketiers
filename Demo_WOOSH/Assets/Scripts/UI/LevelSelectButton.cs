using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField]private int levelID;

    public void Initialize(int id)
    {
        levelID = id;
        //GetComponentInChildren<Text>().text = "" + (levelID + 1);
    }

    public void OnClick()
    {
        UberManager.Instance.PreGameManager.SelectedLevel = levelID;
        UberManager.Instance.GotoState(UberManager.GameStates.PreGame);
    }
}
