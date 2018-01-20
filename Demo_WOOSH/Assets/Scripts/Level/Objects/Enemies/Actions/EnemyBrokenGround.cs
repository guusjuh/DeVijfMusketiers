using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrokenGround : Action
{
    private GameObject[] dusts;

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
        float distance = (parent.GridPosition.EuclideanDistance(parent.Target.GridPosition));
        float maxDistance = GameManager.Instance.TileManager.FromTileToTile;
        bool closeEnough = distance - maxDistance <= 0.01f;
        bool enoughAP = parent.CurrentActionPoints >= cost;

        if (closeEnough && enoughAP)
        {
            List<TileNode> nodes = GameManager.Instance.TileManager.GetNodeReference(parent.GridPosition).NeightBours;
            foreach (var node in nodes)
            {
                node.Content.KillTileContent();
            }
            UberManager.Instance.StartCoroutine(Visual());

            parent.EndMove(cost);

            return true;
        }
        return false;
    }

    private IEnumerator Visual()
    {
        TileNode node = GameManager.Instance.TileManager.GetNodeReference(parent.GridPosition);
        Vector2 worldPos;

        for (int i = 0; i < node.NeightBours.Count; i++)
        {
            dusts[i].SetActive(true);
            worldPos = GameManager.Instance.TileManager.GetWorldPosition(node.NeightBours[i].GridPosition.ToVector2());
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
