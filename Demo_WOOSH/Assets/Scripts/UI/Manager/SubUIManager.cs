using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubUIManager
{
    protected Canvas canvas;
    protected bool first = true;

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

        canvas.gameObject.SetActive(true);
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void Restart()
    {
    }

    public virtual void Clear()
    {
        canvas.gameObject.SetActive(false);
    }

    public Vector2 WorldToCanvas(Vector3 worldPosition)
    {
        //TODO: lol we don't have a camera script or ref
        Camera camera = Camera.main;

        var viewportPos = camera.WorldToViewportPoint(worldPosition);
        var canvasRect = canvas.GetComponent<RectTransform>();

        return new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
    }
}
