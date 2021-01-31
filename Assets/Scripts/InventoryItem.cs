using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class InventoryItem : MonoBehaviour
{
    [HideInInspector] public Vector3Int CellIndex;
    [SerializeField] private SpriteRenderer m_image;
    [SerializeField] private SpriteRenderer m_outline;

    [Header("Shape outline")]
    [SerializeField] private SpriteRenderer m_shapeOutline;
    [SerializeField] private Color m_shapeBaseColor;
    [SerializeField] private Color m_shapeInvalidColor;

    public bool[, ] m_shape = new bool[5, 5];
    //le point de rotation est celui du centre
    public Inventory.Objet_id m_id;

    private Collider2D m_collider;
    private bool m_isColliding => m_inCollisionWith.Count > 0;
    private HashSet<GameObject> m_inCollisionWith = new HashSet<GameObject>();

    private Vector3 m_basePosition;
    private Quaternion m_baseRotation;
    private Transform m_baseParent;
    private Inventory m_inventory;

    void Start()
    {
        m_inventory = Inventory.Instance;

        m_basePosition = transform.position;
        m_baseRotation = transform.rotation;
        m_baseParent = transform.parent;

        m_collider = GetComponent<Collider2D>();
        m_collider.isTrigger = true;
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

    private void ResetPositionAndRotation()
    {
        StartCoroutine(_DelayedReset());
    }

    private IEnumerator _DelayedReset()
    {
        yield return null;

        transform.SetParent(m_baseParent);
        transform.position = m_basePosition;
        transform.rotation = m_baseRotation;
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

    private void UpdateColorToState()
    {
        if (m_isColliding)
        {
            m_image.color = Color.red;
        }
        else
        {
            m_image.color = Color.white;
        }
    }

    private void UpdateOutline(bool p_outlined)
    {
        m_outline.gameObject.SetActive(p_outlined);
    }

    private void UpdateShapeOutline(bool p_isVisible, bool p_isValid)
    {
        m_shapeOutline.gameObject.SetActive(p_isVisible);
        m_shapeOutline.color = p_isValid ? m_shapeBaseColor : m_shapeInvalidColor;
    }
    
    void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        var collisions = Physics2D.OverlapPointAll(mousePosition);
        bool mouseOverObject = false;
        if (collisions.Contains(m_collider))
        {
            mouseOverObject = true;
        }

        if (Input.GetMouseButton(0))
        {
            if (!PlayerSelection.Instance.HasASelectedItem && mouseOverObject)
            {
                PlayerSelection.Instance.SetSelectedItem(this);
                
                System.Random rnd = new System.Random();
                int nbr = rnd.Next() % 6 + 1;
                string file = "prendre_" + nbr.ToString();
                FindObjectOfType<AudioManager>().Play(file);
            }
        }

        // If item is not selected, stop there
        if (PlayerSelection.Instance.SelectedItem != this)
        {
            UpdateOutline(mouseOverObject);
            return;
        }

        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        if (Input.GetMouseButtonDown(1))
        {
            transform.Rotate(new Vector3(0f, 0f, -90f));
        }

        // Check if over inventory
        var inventoryBounds = m_inventory.GetBounds();
        var overInventory = inventoryBounds.Contains(transform.position);

        bool canPutInInventory = true;

        bool colliding = false;
        if (overInventory && m_isColliding)
        {
            canPutInInventory = false;
            colliding = true;
        }

        if (overInventory)
        {
            CellIndex = m_inventory.GetCellIndexFromPosition(transform.position);
            m_shapeOutline.transform.position = m_inventory.GetPositionFromCellIndex(CellIndex);
        }
        else
        {
            m_shapeOutline.transform.position = Vector3.zero;
        }

        bool notInBox = false;
        CellIndex = m_inventory.GetCellIndexFromPosition(transform.position);
        transform.position = m_inventory.GetPositionFromCellIndex(CellIndex);
        var bounds = m_collider.bounds;

        Debug.DrawLine(
            bounds.center + Vector3.right * 0.25f - Vector3.up * 0.25f,
            bounds.center - Vector3.right * 0.25f + Vector3.up * 0.25f,
            Color.green
        );
        Debug.DrawLine(
            bounds.center - Vector3.right * 0.25f - Vector3.up * 0.25f,
            bounds.center + Vector3.right * 0.25f + Vector3.up * 0.25f,
            Color.magenta
        );

        Debug.DrawLine(
            bounds.center - bounds.size / 2f,
            bounds.center + bounds.size / 2f,
            Color.blue
        );
        Debug.DrawLine(
            bounds.center + new Vector3(-bounds.size.x / 2f, bounds.size.y / 2f),
            bounds.center + new Vector3(bounds.size.x / 2f, -bounds.size.y / 2f),
            Color.red
        );

        if (!inventoryBounds.ContainBounds(bounds))
        {
            canPutInInventory = false;
            notInBox = true;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            if (canPutInInventory)
            {
                CellIndex = m_inventory.GetCellIndexFromPosition(transform.position);
                transform.position = m_inventory.GetPositionFromCellIndex(CellIndex);
                transform.SetParent(m_inventory.transform);
                m_shapeOutline.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.Log($"Can't put in inventory Not in box : {notInBox} - Colliding : {colliding}");
                ResetPositionAndRotation();
            }

            PlayerSelection.Instance.SetSelectedItem(null);
            
            System.Random rnd = new System.Random();
            int nbr = rnd.Next() % 6 + 1;
            string file = "poser_" + nbr.ToString();
            FindObjectOfType<AudioManager>().Play(file);
        }
        else
        {
            // Mouse follow
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
        }

        UpdateShapeOutline(overInventory, canPutInInventory && !colliding);
        if (overInventory)
            UpdateOutline(false);
    }
}
