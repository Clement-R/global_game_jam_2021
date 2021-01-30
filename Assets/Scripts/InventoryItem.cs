using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_image;
    [SerializeField] private Inventory m_inventory;
    [SerializeField] private SpriteRenderer m_inBounds;
    [SerializeField] public Vector3Int m_cellIndex;
    [SerializeField] public Collider2D m_collider;

    public bool[, ] m_shape = new bool[5, 5];
    //le point de rotation est celui du centre
    public Inventory.Objet_id m_id;
    private bool m_selected;
    private bool m_isColliding => m_inCollisionWith.Count > 0;
    private HashSet<GameObject> m_inCollisionWith = new HashSet<GameObject>();

    void Start()
    {

    }

    void rotate() //the main difficulty
    {
        bool[, ] newShape = new bool[5, 5];

        for (int x = 0; x < m_shape.GetLength(0); x += 1)
        {
            for (int y = 0; y < m_shape.GetLength(1); y += 1)
            {
                Vector2Int posRelative = new Vector2Int(2 - x, 2 - y);
                System.Numerics.Complex posCo = new System.Numerics.Complex(posRelative.x, posRelative.y);
                posCo *= new System.Numerics.Complex(0, 1);
                //rotation de 90 degres autour du point 2, 2
                Vector2Int posRelativeFinal = new Vector2Int((int) posCo.Real, (int) posCo.Imaginary);

                newShape[x, y] = m_shape[posRelativeFinal.x + 2, posRelativeFinal.y + 2];
            }
        }
        m_shape = newShape;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        m_inCollisionWith.Add(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        m_inCollisionWith.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        m_inCollisionWith.Remove(other.gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var flatPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

            var collisions = Physics2D.OverlapPointAll(flatPosition);
            if (collisions.Contains(m_collider))
            {
                m_selected = true;
            }
        }

        if (m_isColliding)
        {
            m_image.color = Color.red;
        }
        else
        {
            m_image.color = Color.white;
        }

        if (!m_selected)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(new Vector3(0f, 0f, -90f));
        }

        // Mouse follow
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(point.x, point.y, 0f);

        // Check if over inventory
        var inventoryBounds = m_inventory.GetBounds();
        var overInventory = inventoryBounds.Contains(transform.position);

        if (Input.GetMouseButtonDown(1))
        {
            if (m_selected)
            {
                bool canUnselect = true;
                if (overInventory && m_isColliding)
                {
                    canUnselect = false;
                }

                if (!inventoryBounds.ContainBounds(m_collider.bounds))
                    canUnselect = false;

                if (canUnselect)
                    m_selected = false;
            }
        }

        m_inBounds.gameObject.SetActive(overInventory);

        if (overInventory)
        {
            m_cellIndex = m_inventory.GetCellIndexFromPosition(transform.position);
            transform.position = m_inventory.GetPositionFromCellIndex(m_cellIndex);
        }
    }
}

public static class BoundsExtension
{
    public static bool ContainBounds(this Bounds bounds, Bounds target)
    {
        return bounds.Contains(target.min) && bounds.Contains(target.max);
    }
}