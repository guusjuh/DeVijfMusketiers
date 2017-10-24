using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurroundingPushButton : MonoBehaviour
{
    private MovableObject source;
    private Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition;} }

    private RectTransform rect;
    public void Initialize(Coordinate gridPosition)

    {
        this.gridPosition = gridPosition;

        rect = GetComponent<RectTransform>();
    }

    public void Activate(WorldObject source)
    {
        this.source = (MovableObject)source;
        GameManager.Instance.TileManager.GetNodeReference(gridPosition).HighlightTile(true, Color.green);

        SetPosition();

        gameObject.SetActive(true);
    }

    public void SetPosition()
    {
        Vector3 worldPos = GameManager.Instance.TileManager.GetWorldPosition(gridPosition);
        rect.anchoredPosition = UberManager.Instance.UiManager.InGameUI.WorldToCanvas(worldPos);

        float sideLength = GameManager.Instance.TileManager.FromTileToTileInCanvasSpace * 0.8f;
        rect.sizeDelta = new Vector2(sideLength, sideLength);
    }

    public void Deactivate()
    {
        // currently, this button is a non relevant button!
        TileNode node = GameManager.Instance.TileManager.GetNodeReference(gridPosition);
        if (node == null) return;

        this.source = null;
        node.HighlightTile(false);

        gameObject.SetActive(false);
    }

    public void Destory()
    {
        source = null;
        rect = null;
        GameObject.Destroy(this.gameObject);
    }

    public void TeleportSource()
    {
        if(GameManager.Instance.Paused) return;
        
        source.Teleport(gridPosition);
        GameManager.Instance.LevelManager.CheckForExtraAP();
        GameManager.Instance.LevelManager.EndPlayerMove(3);
        GameManager.Instance.LevelManager.Player.SetCooldown(GameManager.SpellType.Teleport);
    }

    public void SetPosition(Vector3 worldPos)
    {
        rect.anchoredPosition = UIManager.Instance.InGameUI.WorldToCanvas(worldPos);
    }
}
