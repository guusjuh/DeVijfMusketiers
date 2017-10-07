using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

//TODO: begin and end turn 
//TODO: do a lot of stuff to its own script
// skipturn button enable/disable
public class UIManager
{
    private static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = UberManager.Instance.UiManager;
            return instance;
        }
    }

    public InGameUIManager InGameUI { get { return (InGameUIManager)uiManagers.Get(UberManager.GameStates.InGame); } }
    public PostGameUIManager PostGameUI { get { return (PostGameUIManager)uiManagers.Get(UberManager.GameStates.PostGame); } }

    private Dictionary<UberManager.GameStates, SubUIManager> uiManagers = new Dictionary<UberManager.GameStates, SubUIManager>();

    public void Initialize()
    {
        uiManagers.Add(UberManager.GameStates.InGame, new InGameUIManager());
        uiManagers.Add(UberManager.GameStates.PostGame, new PostGameUIManager());
    }

    public void RestartUI()
    {
        uiManagers.Get(UberManager.Instance.GameState).Start();
    }

    public void ClearUI()
    {
        uiManagers.Get(UberManager.Instance.GameState).Clear();
    }
}
