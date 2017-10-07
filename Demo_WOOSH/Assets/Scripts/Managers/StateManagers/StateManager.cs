using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager
{
    public abstract void Initialize();
    public abstract void Restart();
    public abstract void Clear();
    public abstract void Update();
}
