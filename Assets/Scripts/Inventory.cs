using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance => m_instance;
    public bool IsFullEnough => GetItemsInInventory().Count >= MinNumOfItems;

    [SerializeField] private int MinNumOfItems = 2;
    [SerializeField] private Vector2Int m_gridSize;
    [SerializeField] private float m_cellSize;
    [SerializeField] private LineRenderer m_linePrefab;
    [SerializeField] private List<Vector3> m_gridCellsPositions = new List<Vector3>();
    [SerializeField] private Objet_id[, ] m_inventory = new Objet_id[4, 4]; //the dimentions are adjustable

    private static Inventory m_instance = null;

    public enum Objet_id
    {
        empty,
        Obj1,
        Obj2,
        autre,
        encoreUnAutre
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DrawGrid();
    }

    public List<InventoryItem> GetItemsInInventory()
    {
        var items = new List<InventoryItem>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent<InventoryItem>(out InventoryItem comp))
                items.Add(comp);
            
        }

        return items;
    }

    private void DrawGrid()
    {
        var halfCellSize = m_cellSize / 2f;
        foreach (var point in m_gridCellsPositions)
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
                    Vector3Int pos = cellPosition(p_object.CellIndex, x, y);
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
                        Vector3Int pos = cellPosition(p_object.CellIndex, x, y);
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

    private void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var flatPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
        MouseCell = GetCellIndexFromPosition(flatPosition);
        MousePos = flatPosition;
    }
    public Vector3Int MouseCell;
    public Vector3 MousePos;

    public Vector3Int GetCellIndexFromPosition(Vector3 p_position)
    {
        var localPos = transform.InverseTransformPoint(p_position);
        localPos = new Vector3(localPos.x, localPos.y, 0);

        // Get closest point
        float minDistance = float.PositiveInfinity;
        Vector3 closest = Vector3.zero;
        foreach (var position in m_gridCellsPositions)
        {
            var distance = Vector3.Distance(localPos, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = position;
            }
        }

        int x = closest.x > 0 ? Mathf.CeilToInt(closest.x) : Mathf.FloorToInt(closest.x);
        if (closest.x >= 0 && closest.x <= m_cellSize / 2f)
        {
            x = 0;
        }

        int y = closest.y > 0 ? Mathf.CeilToInt(closest.y) : Mathf.FloorToInt(closest.y);
        if (closest.y >= 0 && closest.y <= m_cellSize / 2f)
        {
            y = 0;
        }

        var gridIndex = new Vector3Int(
            x,
            y,
            0
        );

        return gridIndex;
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
        m_gridCellsPositions.Clear();

        int width = m_gridSize.x;
        int height = m_gridSize.y;

        Vector3 topLeft = transform.position + new Vector3(
            m_gridSize.x / 2f * m_cellSize * -1f,
            m_gridSize.y / 2f * m_cellSize,
            0f
        );

        Gizmos.DrawWireSphere(topLeft, m_cellSize / 2f);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = (i * m_cellSize) + (m_cellSize / 2f);
                float y = (j * -m_cellSize) - (m_cellSize / 2f);

                var position = topLeft + new Vector3(
                    x,
                    y,
                    0f
                );

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(position, m_cellSize / 4f);

                var localPosition = transform.InverseTransformPoint(position);
                m_gridCellsPositions.Add(localPosition);

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
