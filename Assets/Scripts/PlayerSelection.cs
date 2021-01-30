using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection Instance => m_instance;
    public InventoryItem SelectedItem => m_selectedItem;
    public bool HasASelectedItem => m_selectedItem != null;

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

    public void SetSelectedItem(InventoryItem p_item)
    {
        m_selectedItem = p_item;
    }
}