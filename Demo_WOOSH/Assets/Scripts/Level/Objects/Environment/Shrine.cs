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
            if(value != active && value){
                UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.Shrine);
                MetricsDataClass.ActivatedShrines++;
            }
            active = value;
            sprRender.color = value ? activeColor : normalColor;
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);
        gaveAP = false;

        sprRender = GetComponent<SpriteRenderer>();
        type = SecContentType.Shrine;
    }

    public override void Reset()
    {
        base.Reset();
        Active = false;
        gaveAP = false;
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

    public bool CheckForActive(bool givePoints = true)
    {
        if (Active && givePoints && !gaveAP)
        {
            gaveAP = true;
            GameManager.Instance.LevelManager.Player.IncreaseActionPoints();
            return true;
        }

        List<Coordinate> neighbourCoordinates = new List<Coordinate>(GameManager.Instance.TileManager.Directions(GridPosition));

        // each neighbouring node
        for (int i = 0; i < neighbourCoordinates.Count; i++)
        {
            TileNode nodeRef = GameManager.Instance.TileManager.GetNodeReference(GridPosition + neighbourCoordinates[i]);
            if (nodeRef != null && nodeRef.ContainsHuman())
            {
                Active = true;

                if (givePoints && !gaveAP)
                {
                    gaveAP = true;
                    GameManager.Instance.LevelManager.Player.IncreaseActionPoints();
                    NewFloatingDmgNumber();
                }
                return true;
            }
        }
        return false;
    }

    public void EndPlayerTurn()
    {
        CheckForActive(false);
        gaveAP = false;
    }

    private void NewFloatingDmgNumber()
    {
        FloatingIndicator newFloatingIndicator = new FloatingIndicator();
        newFloatingIndicator.Initialize("+1 AP", Color.green, 2.0f, 1.0f, transform.position);
    }

    public override bool IsShrine() { return true; }
}
