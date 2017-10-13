using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurroundingPushButton : MonoBehaviour
{
    private MovableObject source;
    private Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition;} }
    private Coordinate relativePosition;

    private RectTransform rect;

    public void Initialize(Coordinate gridPosition, Coordinate relativePosition)
    {
        this.gridPosition = gridPosition;
        this.relativePosition = relativePosition;

        rect = GetComponent<RectTransform>();
    }

    public void Activate(MovableObject source)
    {
        this.source = source;
        GameManager.Instance.TileManager.GetNodeReference(gridPosition).HighlightTile(true, Color.green);

        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        // currently, this button is a non relevant button!
        if (GameManager.Instance.TileManager.GetNodeReference(gridPosition) == null) return;

        this.source = null;
        GameManager.Instance.TileManager.GetNodeReference(gridPosition).HighlightTile(false);

        gameObject.SetActive(false);
    }

    public void PushSource()
    {
        source.Push(relativePosition);
        GameManager.Instance.LevelManager.CheckForExtraAP();
        GameManager.Instance.LevelManager.EndPlayerMove(1);
    }

    public void SetPosition(Vector3 worldPos)
    {
        rect.anchoredPosition = UIManager.Instance.InGameUI.WorldToCanvas(worldPos);
    }
}
