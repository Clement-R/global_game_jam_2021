using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection Instance => m_instance;
    public InventoryItem SelectedItem => m_selectedItem;
    public bool HasASelectedItem => m_selectedItem != null;

    [SerializeField] private SpriteRenderer m_cursor;
    [SerializeField] private Sprite m_flatHand;
    [SerializeField] private Sprite m_grabHand;
    [SerializeField] private Sprite m_pointHand;

    private static PlayerSelection m_instance = null;
    private InventoryItem m_selectedItem = null;

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
            if (IsMouseOverAClickable())
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

    public void SetSelectedItem(InventoryItem p_item)
    {
        m_selectedItem = p_item;
    }

    private bool IsMouseOverAClickable()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        var collisions = Physics2D.OverlapPointAll(mousePosition);
        bool mouseOverObject = false;
        if (collisions.FirstOrDefault(e => e.TryGetComponent<Clickable>(out _)) != null)
        {
            mouseOverObject = true;
        }

        return mouseOverObject;
    }
}