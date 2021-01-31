using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using DG.Tweening;

public class InventoryState : MonoBehaviour
{
    [SerializeField] private Clickable m_clickable;

    [SerializeField] private float m_travelDuration;
    [SerializeField] private Ease m_travelEase;

    [SerializeField] private Vector3 m_openPosition;
    [SerializeField] private Vector3 m_closePosition;

    private bool m_lockedOpen = false;
    private bool m_open = false;
    private Collider2D m_collider;
    private bool m_animationPlaying;

    void Start()
    {
        m_collider = GetComponent<Collider2D>();
        m_clickable.OnClick.AddListener(Click);

        SetStateInstant(false);
    }

    private void Update()
    {
        // Show if mouse over and not opened or locked
        if (m_clickable.IsMouseOver() && !m_open && !m_lockedOpen)
        {
            SetState(true);
        }

        // Hide if mouse out and not  or locked
        if (!m_clickable.IsMouseOver() && m_open && !m_lockedOpen)
        {
            SetState(false);
        }

        if (m_lockedOpen && !m_open)
        {
            SetState(true);
        }

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
        m_lockedOpen = !m_lockedOpen;
        SetState(m_lockedOpen);
    }

    private void SetState(bool p_isOpen, bool p_reset = false)
    {
        if (m_animationPlaying)
            return;

        if (m_open == p_isOpen && !p_reset)
            return;

        m_open = p_isOpen;
        if (m_open)
        {
            m_animationPlaying = true;
            transform.DOMove(m_openPosition, m_travelDuration).SetEase(m_travelEase).OnComplete(
                () =>
                {
                    m_animationPlaying = false;
                }
            );
        }
        else
        {
            m_animationPlaying = true;
            transform.DOMove(m_closePosition, m_travelDuration).SetEase(m_travelEase).OnComplete(
                () =>
                {
                    m_animationPlaying = false;
                }
            );
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