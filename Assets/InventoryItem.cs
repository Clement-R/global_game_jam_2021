using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_image;
    [SerializeField] private Inventory m_inventory;
    [SerializeField] private SpriteRenderer m_inBounds;
    [SerializeField] public Vector3Int m_cellIndex;

    public bool[,] m_shape = new bool[5, 5];
    //le point de rotation est celui du centre
    public Inventory.Objet_id m_id;
    
    void Start()
    {

    }
    
    void rotate() //the main difficulty
    {
        bool[,] newShape = new bool[5, 5];
        
        for (int x = 0; x < m_shape.GetLength(0); x += 1) {
            for (int y = 0; y < m_shape.GetLength(1); y += 1) 
            {
                Vector2Int posRelative = new Vector2Int(2-x, 2-y);
                System.Numerics.Complex posCo = new System.Numerics.Complex(posRelative.x, posRelative.y);
                posCo *= new System.Numerics.Complex(0, 1);
                //rotation de 90 degres autour du point 2, 2
                Vector2Int posRelativeFinal = new Vector2Int((int)posCo.Real, (int)posCo.Imaginary);
                
                newShape[x, y] = m_shape[posRelativeFinal.x + 2, posRelativeFinal.y + 2];
            }
        }
        m_shape = newShape;
    }

    void Update()
    {
        // Mouse follow
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(point.x, point.y, 0f);

        // Check if over inventory
        var inventoryBounds = m_inventory.GetBounds();
        var overInventory = inventoryBounds.Contains(transform.position);

        m_inBounds.gameObject.SetActive(overInventory);

        if (overInventory)
        {
            m_cellIndex = m_inventory.GetCellIndexFromPosition(transform.position);
            transform.position = m_inventory.GetPositionFromCellIndex(m_cellIndex);
        }
    }
}
