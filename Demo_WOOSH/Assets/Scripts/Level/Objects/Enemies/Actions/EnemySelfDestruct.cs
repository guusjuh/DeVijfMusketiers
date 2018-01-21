
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelfDestruct : Action
{
    private const int DUST_AMOUNT = 7;
    private GameObject dustPrefab;

    private GameObject[] dusts;

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        dustPrefab = Resources.Load<GameObject>("Prefabs/Actions/Dust");

        dusts = new GameObject[DUST_AMOUNT];

        for (int i = 0; i < dusts.Length; i++)
        {
            dusts[i] = GameObject.Instantiate(dustPrefab, parent.transform);
            dusts[i].transform.localPosition = new Vector3(0, 0, 0);
            dusts[i].SetActive(false);
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();

        if(parent.HealthPercentage <= 10)
        {
            List<LevelTileNode> nodes = GameManager.Instance.TileManager.GetNodeReference(parent.GridPosition).NeightBours;
            foreach (var node in nodes)
            {
                node.Content.KillTileContent();
            }
            UberManager.Instance.StartCoroutine(Visual());
            parent.EndMove(parent.CurrentActionPoints);
        }
    }

    private IEnumerator Visual()
    {
        LevelTileNode node = GameManager.Instance.TileManager.GetNodeReference(parent.GridPosition);
        Vector2 worldPos;
        for (int i = 0; i < node.NeightBours.Count; i++)
        {
            dusts[i].SetActive(true);
            worldPos = GameManager.Instance.TileManager.GetWorldPosition(new Vector2(node.NeightBours[i].GridPosition.x, node.NeightBours[i].GridPosition.y));
            dusts[i].transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        }

        yield return new WaitForSeconds(0.5f);

        parent.Kill();

        yield break;
    }
}
