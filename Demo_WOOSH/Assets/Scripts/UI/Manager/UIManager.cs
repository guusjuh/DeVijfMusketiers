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

    private InGameUIManager inGameUI= new InGameUIManager();
    public InGameUIManager InGameUI { get { return inGameUI; } }

    private PostGameUIManager postGameUI = new PostGameUIManager();
    public PostGameUIManager PostGameUI { get { return postGameUI; } }

    public void Initialize()
    {
        //postGameUI.Initialize();
    }

    public void Restart()
    {
        //postGameUI.Restart();
    }

    public void Clear()
    {
        //postGameUI.Clear();

    }
}
