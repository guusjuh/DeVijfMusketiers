using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelID;

    public void OnClick()
    {
        UberManager.Instance.PreGameManager.SelectedLevel = levelID;
        UberManager.Instance.GotoState(UberManager.GameStates.PreGame);
    }
}
