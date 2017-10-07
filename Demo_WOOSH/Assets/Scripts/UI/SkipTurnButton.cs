using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipTurnButton : MonoBehaviour
{
    private Button button;

    private bool active = true;
    public bool Active
    {
        get { return active; }
        set
        {
            if (active == value) return;
            active = value;
            button.interactable = active;
        }
    }

    public void Initialize()
    {
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(10.0f, 10.0f, 0.0f);
        button = GetComponent<Button>();
        button.onClick.AddListener(GameManager.Instance.SkipPlayerTurn);
    }
}
