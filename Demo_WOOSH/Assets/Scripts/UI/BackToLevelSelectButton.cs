using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToLevelSelectButton : MonoBehaviour {
    public void OnClick()
    {
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        UberManager.Instance.GotoState(UberManager.GameStates.LevelSelection);
    }
}
