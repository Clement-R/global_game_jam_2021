using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection Instance => m_instance;
    public InventoryItem SelectedItem => m_selectedItem;
    public bool HasASelectedItem => m_selectedItem != null || m_fakeSelectedItem != null;

    public bool isOverUI = false;

    public LayerMask Layer;

    [SerializeField] private SpriteRenderer m_cursor;
    [SerializeField] private Sprite m_flatHand;
    [SerializeField] private Sprite m_grabHand;
    [SerializeField] private Sprite m_pointHand;

    private static PlayerSelection m_instance = null;
    private InventoryItem m_selectedItem = null;
    private GameObject m_fakeSelectedItem = null;

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
        Cursor.visible = false;
        Layer = LayerMask.GetMask("InventoryItem");
    }

    private void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
        transform.position = mousePosition;

        if (HasASelectedItem)
        {
            // Change to grab
            m_cursor.sprite = m_grabHand;
        }
        else
        {
            if (IsMouseOverAnItem() || isOverUI)
            {
                // change to pointer
                m_cursor.sprite = m_pointHand;
            }
            else
            {
                // change to default
                m_cursor.sprite = m_flatHand;
            }
        }
    }

    public void IsOverUI(bool b) {
        isOverUI = b;
    }

    public void SetSelectedItem(InventoryItem p_item)
    {
        m_selectedItem = p_item;
    }

    public void SetFakeSelectedItem(GameObject p_item)
    {
        m_fakeSelectedItem = p_item;
    }

    private bool IsMouseOverAnItem()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
        return Physics2D.OverlapPointAll(mousePosition, Layer).Length > 0;
    }
}