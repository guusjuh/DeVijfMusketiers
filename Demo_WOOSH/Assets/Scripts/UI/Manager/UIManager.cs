using System;
using System.Collections.Generic;
using UnityEngine;

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
    public LevelSelectUIManager LevelSelectUI { get { return (LevelSelectUIManager)uiManagers.Get(UberManager.GameStates.LevelSelection); } }
    public PreGameUIManager PreGameUI { get { return (PreGameUIManager)uiManagers.Get(UberManager.GameStates.PreGame); } }

    private Dictionary<UberManager.GameStates, SubUIManager> uiManagers = new Dictionary<UberManager.GameStates, SubUIManager>();

    public void Initialize()
    {
        if (UberManager.Instance.DevelopersMode)
        {
            uiManagers.Add(UberManager.GameStates.InGame, new InGameUIManager());
        }
        else
        {
            uiManagers.Add(UberManager.GameStates.InGame, new InGameUIManager());
            uiManagers.Add(UberManager.GameStates.PostGame, new PostGameUIManager());
            uiManagers.Add(UberManager.GameStates.LevelSelection, new LevelSelectUIManager());
            uiManagers.Add(UberManager.GameStates.PreGame, new PreGameUIManager());
        }
    }


    public void RestartUI()
    {
        uiManagers.Get(UberManager.Instance.GameState).Start();
    }

    public void ClearUI()
    {
        uiManagers.Get(UberManager.Instance.GameState).Clear();
    }

    public void UpdateUI()
    {
        uiManagers.Get(UberManager.Instance.GameState).Update();
    }

    public GameObject CreateUIElement(String prefabPath, Vector2 position, Transform parent)
    {
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(prefabPath), Vector3.zero, Quaternion.identity, parent);

        if (CheckForRTError(go)) return null;

        go.GetComponent<RectTransform>().anchoredPosition = position;

        return go;
    }

    public GameObject CreateUIElement(GameObject prefab, Vector2 position, Transform parent)
    {
        GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);

        if (CheckForRTError(go)) return null;

        go.GetComponent<RectTransform>().anchoredPosition = position;

        return go;
    }

    public GameObject CreateUIElement(Vector2 position, Vector2 sizeDelta, Transform parent)
    {
        GameObject go = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, parent);

        go.AddComponent<RectTransform>();

        go.GetComponent<RectTransform>().anchoredPosition = position;
        go.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        return go;
    }

    private bool CheckForRTError(GameObject go)
    {
        if (go.GetComponent<RectTransform>() == null)
        {
            Debug.LogError("New UI object misses rect transform " + go.name);
            GameObject.Destroy(go);
            return true;
        }

        return false;
    }
}
