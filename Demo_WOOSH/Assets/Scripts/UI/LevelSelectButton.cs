using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    private int levelID;
    private bool interactable = true;
    public bool Interactable { get { return interactable; } set { interactable = value; } }

    public void Initialize(int id)
    {
        levelID = id;
        //GetComponentInChildren<Text>().text = "" + (levelID + 1);
    }

    public void OnClick()
    {
        if (interactable)
        {
            UberManager.Instance.PreGameManager.SelectedLevel = levelID;
            SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
            UberManager.Instance.GotoState(UberManager.GameStates.PreGame);
        }
    }
}
