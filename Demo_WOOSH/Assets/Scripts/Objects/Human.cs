using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Human : MovableObject {

    private Color shieldColor = new Color(0.0f, 0.0f, 0.5f, 0.5f);
    private Color normalColor;

    private bool invisible;
    private static int totalInvisiblePoints = 2;
    private int currInvisiblePoints;
    public bool Inivisible { get { return invisible;} }

    private Contract contractRef;

    public Contract ContractRef
    {
        get { return contractRef; }
        set
        {
            contractRef = value;
            sprRender.sprite = contractRef.InWorld;
        }
    }

    private SpriteRenderer sprRender;

    private bool inPanic;
    private int viewDistance = 3;

    public bool InPanic
    {
        get
        {
            CheckInPanic();
            return inPanic;
        }
    }

    private void CheckInPanic()
    {
        // find all enemies in the level
        List<Enemy> enemies = GameManager.Instance.LevelManager.Enemies;

        // check for each being in range
        for (int i = 0; i < enemies.Count; i++)
        {
            if (GameManager.Instance.TileManager.InRange(viewDistance, this, enemies[i]))
            {
                inPanic = true;
                return;
            }
        }
    }

    public IEnumerator Flee()
    {
        // if you want humans to run further, more possible node should be obtained from TileManager
        // since I can only flee one step, find the best neighbour to flee to

        List<TileNode> neighbours = GameManager.Instance.TileManager.GetNodeReference(gridPosition).NeightBours;
        List<TileNode> fleeNodes = new List<TileNode>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            // 1. if the neighbour is occupied or not an enterable tile, continue
            // also continue if its a corner tile
            if (!neighbours[i].Content.CompletelyEmpty() || 
                GameManager.Instance.TileManager.Corners.Contains(neighbours[i]))
                    goto NOT_THIS_NEIGHBOUR;

            // 2. not towards an enemy
            // find all enemies in view distance
            List<Enemy> closeEnemies = GameManager.Instance.LevelManager.Enemies.FindAll(
                e => GameManager.Instance.TileManager.InRange(viewDistance, this, e));

            // if the distance between my tile and the enemy is greater than the distance betweent the neighbouring tile and the enemy
            // the hooman will be getting closer, so don't go to that tile
            int currentDist = 0;
            int neighbourDist = 0;
            for (int j = 0; j < closeEnemies.Count; j++)
            {
                currentDist = gridPosition.ManhattanDistance(closeEnemies[j].GridPosition);
                neighbourDist = neighbours[i].GridPosition.ManhattanDistance(closeEnemies[j].GridPosition);

                if (neighbourDist <= currentDist)
                {
                    goto NOT_THIS_NEIGHBOUR;
                }
            }

            // 3. not directly next to a hole
            // find all holes in view distance
            List<TileNode> closeHoles = GameManager.Instance.TileManager.GetNodeWithGooReferences().FindAll(
                n => GameManager.Instance.TileManager.InRange(viewDistance, this.GridPosition, n.GridPosition));

            // check for the distance being smaller (just like with enemies)
            for (int j = 0; j < closeHoles.Count; j++)
            {
                currentDist = gridPosition.ManhattanDistance(closeHoles[j].GridPosition);
                neighbourDist = neighbours[i].GridPosition.ManhattanDistance(closeHoles[j].GridPosition);

                if (neighbourDist < currentDist)
                {
                    goto NOT_THIS_NEIGHBOUR;
                }
            }

            // if we get to this point, add to the possible flee tiles
            fleeNodes.Add(neighbours[i]);

            NOT_THIS_NEIGHBOUR: continue;
        }

        //TODO: pick a random flee node an run towards it
        Debug.Log(fleeNodes.Count);
        fleeNodes.HandleAction(n => n.HighlightTile(true, Color.magenta));

        yield return null;
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        invisible = false;
        sprRender = GetComponent<SpriteRenderer>();
        type = TileManager.ContentType.Human;
        normalColor = sprRender.color;
    }

    public override void Clear()
    {
        GameManager.Instance.LevelManager.RemoveHuman(this);
    }

    public void MakeInvisible()
    {
        GetComponent<SpriteRenderer>().color = shieldColor;
        invisible = true;
        currInvisiblePoints = totalInvisiblePoints;

        GameManager.Instance.TileManager.SwitchStateTile(type, gridPosition);
        type = TileManager.ContentType.InivisbleHuman;
        gameObject.layer = 0;
    }

    public void DecreaseInvisiblePoints()
    {
        if (invisible)
        {
            currInvisiblePoints--;

            //TODO: shield color change

            if (currInvisiblePoints <= 0)
            {
                sprRender.color = normalColor;
                invisible = false;
                GameManager.Instance.TileManager.SwitchStateTile(type, gridPosition);
                type = TileManager.ContentType.Human;
                gameObject.layer = 8;
            }
        }
    }

    public override bool Hit()
    {
        canBeTargeted = false;

        contractRef.Die();

        GameManager.Instance.LevelManager.RemoveHuman(this, true);

        return true;
    }
}
