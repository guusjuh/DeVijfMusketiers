using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : EnemyTarget
{
    private bool active = false;
    private bool gaveAP = false;

    private static Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private static Color normalColor = new Color(0.55f, 0.55f, 0.55f, 1.0f);

    private SpriteRenderer sprRender;

    public bool Active {
        get { return active; }
        private set {
            active = value;
            sprRender.color = active ? activeColor : normalColor;
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        sprRender = GetComponent<SpriteRenderer>();
        type = SecContentType.Shrine;
    }

    public override void Clear()
    {
        GameManager.Instance.LevelManager.RemoveObject(this);
    }

    public override bool Hit()
    {
        canBeTargeted = false;
        active = false;

        sprRender.color = normalColor;

        GameManager.Instance.LevelManager.RemoveObject(this);

        return true;
    }

    public void CheckForActive()
    {
        List<Coordinate> neighbourCoordinates = new List<Coordinate>(GameManager.Instance.TileManager.Directions(GridPosition));

        // each neighbouring node
        for (int i = 0; i < neighbourCoordinates.Count; i++)
        {
            if (GameManager.Instance.TileManager.GetNodeReference(neighbourCoordinates[i]).ContainsHuman())
            {
                Active = true;

                if (!gaveAP)
                {
                    gaveAP = true;
                    GameManager.Instance.LevelManager.Player.IncreaseActionPoints();
                }

                break;
            }
        }

        Active = false;
    }

    public void EndPlayerTurn()
    {
        gaveAP = false;
    }

    public override bool IsShrine() { return true; }
}
