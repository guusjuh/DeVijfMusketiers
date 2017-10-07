using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameManager : StateManager
{
    protected override void Initialize()
    {
        UIManager.Instance.RestartUI();
    }

    protected override void Restart()
    {
        UIManager.Instance.RestartUI();

    }

    public override void Clear()
    {
        UIManager.Instance.ClearUI();

    }

    public override void Update()
    {

    }
}
