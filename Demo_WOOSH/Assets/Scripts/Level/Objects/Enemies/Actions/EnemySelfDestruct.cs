
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelfDestruct : Action
{
    GameObject[] dusts;

    public override void StartTurn()
    {
        base.StartTurn();

        if(parent.HealthPercentage <= 10)
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
            parent.EndMove(parent.CurrentActionPoints);
        }
    }

    private IEnumerator visual()
    {
        dusts = new GameObject[7];

        for (int i = 0; i < dusts.Length; i++)
        {
            dusts[i] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actions/Dust"), parent.transform);
            dusts[i].transform.localPosition = new Vector3(0, 0, 0);
            dusts[i].SetActive(true);
        }

        Vector2 pos = new Vector2(parent.transform.position.x, parent.transform.position.y);
        Coordinate coord = GameManager.Instance.TileManager.GetGridPosition(pos);
        TileNode node = GameManager.Instance.TileManager.GetNodeReference(coord);
        for (int i = 0; i < node.NeightBours.Count; i++)
        {
            Vector2 worldPos = GameManager.Instance.TileManager.GetWorldPosition(new Vector2(node.NeightBours[i].GridPosition.x, node.NeightBours[i].GridPosition.y));
            dusts[i].transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        }

        yield return new WaitForSeconds(0.5f);

        parent.Kill();

        yield break;
    }
}
