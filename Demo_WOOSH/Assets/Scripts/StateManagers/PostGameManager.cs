using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameManager : StateManager {
    protected override void Initialize() {
        UIManager.Instance.RestartUI();
        UberManager.Instance.SoundManager.SetBackGroundMusic(SoundManager.Background.LevelSelection);
    }

    protected override void Restart() {
        UIManager.Instance.RestartUI();
        UberManager.Instance.SoundManager.SetBackGroundMusic(SoundManager.Background.LevelSelection);
    }

    public override void Clear() {
        UIManager.Instance.ClearUI();
    }

    public override void Update() { }
}
