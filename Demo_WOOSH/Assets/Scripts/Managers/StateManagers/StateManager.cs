using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager
{
    private bool first = true;

    public void Start()
    {
        if (first)
        {
            Initialize();
            first = false;
        }
        else
        {
            Restart();
        }
    }

    protected abstract void Initialize();
    protected abstract void Restart();
    public abstract void Clear();
    public abstract void Update();
}
