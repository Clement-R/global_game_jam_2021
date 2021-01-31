using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class InventoryState : MonoBehaviour
{
    [SerializeField] private Clickable m_clickable;

    [SerializeField] private Vector3 m_openPosition;
    [SerializeField] private Vector3 m_closePosition;

    private bool m_open = false;
    private Collider2D m_collider;

    void Start()
    {
        m_collider = GetComponent<Collider2D>();
        m_clickable.OnClick.AddListener(Click);

        SetStateInstant(false);
    }

    private void Update()
    {
        // Detect player drag an object over if closed
        if (PlayerSelection.Instance.SelectedItem == null || m_open)
            return;

        if (m_clickable.IsMouseOver())
        {
            SetState(true);
        }
    }

    private void Click()
    {
        SetState(!m_open);
    }

    private void SetState(bool p_isOpen, bool p_reset = false)
    {
        if (m_open == p_isOpen && !p_reset)
            return;

        m_open = p_isOpen;
        if (m_open)
        {
            //TODO: play open tween
            transform.position = m_openPosition;
        }
        else
        {
            //TODO: play close tween
            transform.position = m_closePosition;
        }
    }

    private void SetStateInstant(bool p_isOpen)
    {
        m_open = p_isOpen;
        if (m_open)
        {
            transform.position = m_openPosition;
        }
        else
        {
            transform.position = m_closePosition;
        }
    }
}