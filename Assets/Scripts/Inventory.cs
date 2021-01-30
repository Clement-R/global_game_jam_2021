using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Vector2Int m_gridSize;
    [SerializeField] private float m_cellSize;
    [SerializeField] private LineRenderer m_linePrefab;
    [SerializeField] private List<Vector2> m_coords = new List<Vector2>();
    [SerializeField] private Objet_id[, ] m_inventory = new Objet_id[4, 4]; //the dimentions are adjustable

    public enum Objet_id
    {
        empty,
        Obj1,
        Obj2,
        autre,
        encoreUnAutre
    }

    private void Start()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        var halfCellSize = m_cellSize / 2f;
        foreach (var point in m_coords)
        {
            var lr = Instantiate(m_linePrefab);
            lr.transform.SetParent(transform);
            lr.transform.localPosition = point;

            List<Vector3> pos = new List<Vector3>();
            pos.Add(new Vector3(-halfCellSize, -halfCellSize, 0f));
            pos.Add(new Vector3(halfCellSize, -halfCellSize, 0f));
            pos.Add(new Vector3(halfCellSize, halfCellSize, 0f));
            pos.Add(new Vector3(-halfCellSize, halfCellSize, 0f));
            pos.Add(new Vector3(-halfCellSize, -halfCellSize - lr.endWidth / 2f, 0f));

            lr.positionCount = 0;
            lr.SetPositions(new Vector3[0]);

            lr.positionCount = pos.Count;
            lr.SetPositions(pos.ToArray());
        }
    }

    public Vector3Int cellPosition(Vector3Int p_cellIndex, int x, int y)
    {
        return new Vector3Int(
            p_cellIndex.x + x, //je suis pas sur que ca corresponde, faut aller voir
            p_cellIndex.y + y,
            0
        );
    }

    private bool isAddingPossible(InventoryItem p_object)
    {
        for (int x = 0; x < p_object.m_shape.GetLength(0); x += 1)
        {
            for (int y = 0; y < p_object.m_shape.GetLength(1); y += 1)
            {

                if (p_object.m_shape[x, y])
                {
                    Vector3Int pos = cellPosition(p_object.m_cellIndex, x, y);
                    if (m_inventory[pos.x, pos.y] != Objet_id.empty)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private void addObject(InventoryItem p_object)
    {
        if (isAddingPossible(p_object))
        {
            for (int x = 0; x < p_object.m_shape.GetLength(0); x += 1)
            {
                for (int y = 0; y < p_object.m_shape.GetLength(1); y += 1)
                {
                    if (p_object.m_shape[x, y])
                    {
                        Vector3Int pos = cellPosition(p_object.m_cellIndex, x, y);
                        m_inventory[pos.x, pos.y] = p_object.m_id;
                    }
                }
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
        var offsetlocalPos = localPos + new Vector3(Mathf.Sign(localPos.x), Mathf.Sign(localPos.y), 0f) * m_cellSize / 2f;

        var flooredPos = new Vector2(
            (int) Mathf.Sign(offsetlocalPos.x) * Mathf.FloorToInt(Mathf.Abs(offsetlocalPos.x)),
            (int) Mathf.Sign(offsetlocalPos.y) * Mathf.FloorToInt(Mathf.Abs(offsetlocalPos.y))
        );

        var gridIndex = new Vector3Int(
            (int) Mathf.Sign(flooredPos.x) * Mathf.FloorToInt(Mathf.Abs(flooredPos.x)),
            (int) Mathf.Sign(flooredPos.y) * Mathf.FloorToInt(Mathf.Abs(flooredPos.y)),
            0
        );

        return gridIndex;
    }

    public Vector3Int MouseCellIndex;
    private void Update()
    {
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        point = new Vector3(point.x, point.y, 0f);
        MouseCellIndex = GetCellIndexFromPosition(point);
    }

    public Vector3 GetPositionFromCellIndex(Vector3Int p_cellIndex)
    {
        return new Vector3(
            transform.position.x + (p_cellIndex.x * m_cellSize),
            transform.position.y + (p_cellIndex.y * m_cellSize),
            0f
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        var halfCellSize = m_cellSize / 2f;

        m_coords.Clear();

        int width = m_gridSize.x;
        int height = m_gridSize.y;

        Vector3 topLeft = transform.position + new Vector3(
            m_gridSize.x / 2f * m_cellSize * -1f,
            m_gridSize.y / 2f * m_cellSize,
            0f
        );

        // Gizmos.DrawWireSphere(topLeft, 0.5f);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = (i * m_cellSize) + m_cellSize / 2f;
                float y = (j * -m_cellSize) - m_cellSize / 2f;

                var position = topLeft + new Vector3(
                    x,
                    y,
                    0f
                );

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(position, m_cellSize / 4f);

                var localPosition = transform.InverseTransformPoint(position);

                m_coords.Add(new Vector2(Mathf.Round(localPosition.x), Mathf.Round(localPosition.y)));

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(position, Vector3.one * m_cellSize);
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