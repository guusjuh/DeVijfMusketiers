using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameManager : StateManager {
    protected override void Initialize() {
        UIManager.Instance.RestartUI();
        SetSounds();
    }

    protected override void Restart() {
        UIManager.Instance.RestartUI();
        SetSounds();
    }

    public override void Clear() {
        UIManager.Instance.ClearUI();
    }

    public override void Update() { }

    private void SetSounds()
    {
        UberManager.Instance.SoundManager.PlaySoundEffect(GameManager.Instance.Won ? SoundManager.SoundEffect.Victory : SoundManager.SoundEffect.Defeat);
        UberManager.Instance.SoundManager.SetBackGroundMusic(SoundManager.Background.LevelSelection);
    }
}
