using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrokenGround : Action
{
    GameObject[] dusts;

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        dusts = new GameObject[6];
        
        for (int i = 0; i < dusts.Length; i++)
        {
            dusts[i] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actions/Dust"), parent.transform);
            dusts[i].SetActive(false);
        }

        cost = 1;
    }

    public override bool DoAction()
    {
        float distance = (parent.GridPosition.EuclideanDistance(parent.target.GridPosition));
        float maxDistance = GameManager.Instance.TileManager.FromTileToTile;
        bool closeEnough = distance - maxDistance <= 0.01f;
        bool enoughAP = parent.CurrentActionPoints >= cost;

        if (closeEnough && enoughAP)
        {
            Vector2 position = new Vector2(parent.transform.position.x, parent.transform.position.y);
            Coordinate tempCoord = GameManager.Instance.TileManager.GetGridPosition(position);
            TileNode tile = GameManager.Instance.TileManager.GetNodeReference(tempCoord);
            List<TileNode> nodes = tile.NeightBours;
            foreach (var node in nodes)
            {
                node.Content.KillTileContent();
            }
            UberManager.Instance.StartCoroutine(visual());

            parent.EndMove(cost);

            return true;
        }
        return false;
    }

    private IEnumerator visual()
    {
        Vector2 pos = new Vector2(parent.transform.position.x, parent.transform.position.y);
        Coordinate coord = GameManager.Instance.TileManager.GetGridPosition(pos);
        TileNode node = GameManager.Instance.TileManager.GetNodeReference(coord);
        for (int i = 0; i < node.NeightBours.Count; i++)
        {
            dusts[i].SetActive(true);
            Vector2 worldPos = GameManager.Instance.TileManager.GetWorldPosition(new Vector2(node.NeightBours[i].GridPosition.x, node.NeightBours[i].GridPosition.y));
            dusts[i].transform.position = new Vector3(worldPos.x, worldPos.y, 0);

        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < dusts.Length; i++)
        {
            dusts[i].SetActive(false);
        }

        yield break;
    }
}
