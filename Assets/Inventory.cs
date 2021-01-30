using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Vector2Int m_gridSize;
    [SerializeField] private float m_cellSize;
    [SerializeField] private List<Vector2> m_coords = new List<Vector2>();

    private void Start()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        foreach (var point in m_coords)
        {
            var go = new GameObject();
            go.transform.position = transform.position + new Vector3(
                point.x * m_cellSize,
                point.y * m_cellSize,
                0f
            );

            List<Vector2> positions = new List<Vector2>()
            {
                Vector2.right,
                Vector2.up,
                Vector2.left,
                Vector2.down
            };

            for (int i = 0; i < 4; i++)
            {
                var line = new GameObject();
                line.transform.SetParent(go.transform);
                var lr = line.AddComponent<LineRenderer>();
                // lr.SetPositions();
            }
        }
    }

    public Bounds GetBounds()
    {
        return new Bounds(
            new Vector3(transform.position.x, transform.position.y, 0f),
            new Vector3(
                m_gridSize.x * m_cellSize,
                m_gridSize.y * m_cellSize,
                0f
            )
        );
    }

    public Vector3Int GetCellIndexFromPosition(Vector3 p_position)
    {
        var localPos = transform.InverseTransformPoint(p_position);
        var gridIndex = new Vector3Int(
            (int) Mathf.Floor(localPos.x / m_cellSize) + 1,
            (int) Mathf.Floor(localPos.y / m_cellSize) + 1,
            0
        );
        return gridIndex;
    }

    public Vector3 GetPositionFromCellIndex(Vector3Int p_cellIndex)
    {
        return new Vector3(
            transform.position.x + (p_cellIndex.x * m_cellSize) - m_cellSize * 0.5f,
            transform.position.y + (p_cellIndex.y * m_cellSize) - m_cellSize * 0.5f,
            0f
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        int halfWidth = m_gridSize.x / 2;
        int halfHeight = m_gridSize.y / 2;

        var halfCellSize = m_cellSize / 2f;

        m_coords.Clear();

        for (int i = -halfWidth; i <= halfWidth; i++)
        {
            for (int j = -halfHeight; j <= halfHeight; j++)
            {
                if (i == 0 || j == 0) continue;

                float x = Mathf.Abs(i) == 1 ? i * halfCellSize : (-Mathf.Sign(i) * halfCellSize) + i * m_cellSize;
                float y = Mathf.Abs(j) == 1 ? j * halfCellSize : (-Mathf.Sign(j) * halfCellSize) + j * m_cellSize;

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position + new Vector3(x, y, 0f), 0.25f);

                m_coords.Add(new Vector2(x, y));

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(transform.position + new Vector3(x, y, 0f), Vector3.one * m_cellSize);
            }
        }

        var bounds = GetBounds();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2f);
    }

    private Color RandomColor()
    {
        return new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            1f
        );
    }
}