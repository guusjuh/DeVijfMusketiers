using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    private bool init = false;

    private int rows;
    private int columns;

    public void Initialize()
    {
        init = true;
        rows = UberManager.Instance.LevelEditor.Rows;
        columns = UberManager.Instance.LevelEditor.Columns;
    }

    void OnDrawGizmosSelected()
    {
        if (init)
        {
            // if the amount of rows/columns changed, update accordingly, but only if they are not 0
            if ((rows != UberManager.Instance.LevelEditor.Rows || columns != UberManager.Instance.LevelEditor.Columns) 
                && (UberManager.Instance.LevelEditor.Rows > 0 && UberManager.Instance.LevelEditor.Columns > 0))
            {
                rows = UberManager.Instance.LevelEditor.Rows;
                columns = UberManager.Instance.LevelEditor.Columns;
            }

            // set gizmos color
            Gizmos.color = Color.white;

            // draw all hexagons
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    DrawHex(GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(i, j)));
                }
            }
        }
    }

    /// <summary>
    /// Draws the hexagon.
    /// </summary>
    /// <param name="worldPosition">The world position, center of the hexagon.</param>
    private void DrawHex(Vector2 worldPosition)
    {
        float radius = GameManager.Instance.TileManager.HexagonScale / 2.0f;
        float angleRadians = (float)(0 * 2.0f * Math.PI / 6.0f);

        Vector2 prevPos;
        Vector2 position;

        prevPos.x = (float)Math.Round(worldPosition.x + radius * Mathf.Cos(angleRadians), 2);
        prevPos.y = (float)Math.Round(worldPosition.y + radius * Mathf.Sin(angleRadians), 2);

        for (int i = 1; i <= 6; i++)
        {
            angleRadians = (float)(i * 2.0f * Math.PI / 6.0f);

            position.x = (float)Math.Round(worldPosition.x + radius * Mathf.Cos(angleRadians), 2);
            position.y = (float)Math.Round(worldPosition.y + radius * Mathf.Sin(angleRadians), 2);

            Gizmos.DrawLine(prevPos, position);

            prevPos = position;
        }
    }
}
