using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class IntroLetterBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private UIManager m_uiManager;
    [SerializeField] private RectTransform m_transform;
    [SerializeField] private RectTransform m_canvasRect;
    [SerializeField] private Collider2D m_box;

    private Vector3 m_basePosition;
    private Vector2 m_diff;

    private void Start()
    {
        m_transform = GetComponent<RectTransform>();
        m_basePosition = m_transform.anchoredPosition;
        m_uiManager = GameObject.FindObjectOfType<UIManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 canvasPosition = WorldToCanvas(Input.mousePosition);
        var diff = canvasPosition - m_transform.anchoredPosition;
        m_diff = diff;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_uiManager.RevealIntroDone)
            return;

        PlayerSelection.Instance.SetFakeSelectedItem(gameObject);

        Vector2 canvasPosition = WorldToCanvas(Input.mousePosition);
        m_transform.anchoredPosition = canvasPosition - m_diff;
    }

    public Vector2 WorldToCanvas(Vector3 p_position)
    {
        var world = Camera.main.ScreenToWorldPoint(p_position);
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(world);
        return new Vector2(
            (ViewportPosition.x * m_canvasRect.sizeDelta.x) - (m_canvasRect.sizeDelta.x * 0.5f),
            (ViewportPosition.y * m_canvasRect.sizeDelta.y) - (m_canvasRect.sizeDelta.y * 0.5f));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        world = new Vector3(world.x, world.y, 0f);

        if (m_box.bounds.Contains(world))
        {
            m_uiManager.DropLetterInBox();
        }
        else
        {
            m_transform.anchoredPosition = m_basePosition;
        }

        PlayerSelection.Instance.SetFakeSelectedItem(null);
    }
}