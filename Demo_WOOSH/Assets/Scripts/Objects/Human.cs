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

    private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
    private const float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    private float inverseMoveTime;          //Used to make movement more efficient.

    private int totalFleePoints = 2;
    private int currentFleePoints;
    public int CurrentFleePoints { get {return currentFleePoints; } }
    private bool inPanic;
    private int viewDistance = 4;
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

    public IEnumerator Flee()
    {
        currentFleePoints--;

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
            float currentDist = 0;
            float neighbourDist = 0;
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
            List<TileNode> closeHoles = GameManager.Instance.TileManager.GetNodeWithGapReferences().FindAll(
                n => GameManager.Instance.TileManager.InRange(viewDistance/2, this.GridPosition, n.GridPosition));

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

        // if there is just no better tile to run to, just stay here
        if(fleeNodes.Count <= 0) yield break;

        //pick a random flee node 
        TileNode chosenNode = fleeNodes[UnityEngine.Random.Range(0, fleeNodes.Count)];

        //move to the chosen node
        GameManager.Instance.TileManager.MoveObject(gridPosition, chosenNode.GridPosition, type);
        gridPosition = chosenNode.GridPosition;

        yield return StartCoroutine(SmoothMovement(GameManager.Instance.TileManager.GetWorldPosition(chosenNode.GridPosition)));

        //Debug.Log(fleeNodes.Count);
        //fleeNodes.HandleAction(n => n.HighlightTile(true, Color.magenta));

        yield return null;
    }

    // co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter.
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        type = TileManager.ContentType.Human;

        inverseMoveTime = 1f / moveTime;
        rb2D = GetComponent<Rigidbody2D>();

        invisible = false;
        sprRender = GetComponent<SpriteRenderer>();
        normalColor = sprRender.color;

        currentFleePoints = totalFleePoints;
        possibleSpellTypes.Add(GameManager.SpellType.Teleport);
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

    public void ActivateTeleportButtons()
    {
        //TODO: activate buttons for each tile on the grid except for mine
        UberManager.Instance.UiManager.InGameUI.ActivateTeleportButtons(true, this);
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

    public void StartTurn()
    {
        currentFleePoints = totalFleePoints;
        DecreaseInvisiblePoints();
    }
}
