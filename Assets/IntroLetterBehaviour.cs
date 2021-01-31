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

    private void Start()
    {
        m_transform = GetComponent<RectTransform>();
        m_basePosition = m_transform.anchoredPosition;
        m_uiManager = GameObject.FindObjectOfType<UIManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_uiManager.RevealIntroDone)
            return;

        PlayerSelection.Instance.SetFakeSelectedItem(gameObject);

        var pos = Camera.main.ScreenToViewportPoint(eventData.position);
        Vector3 screenPos = new Vector3(
            Screen.width * pos.x,
            Screen.height * pos.y,
            0f
        );

        m_transform.anchoredPosition = screenPos;

        var world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(world);
        Vector2 canvasPosition = new Vector2(
            ((ViewportPosition.x * m_canvasRect.sizeDelta.x) - (m_canvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * m_canvasRect.sizeDelta.y) - (m_canvasRect.sizeDelta.y * 0.5f)));
        m_transform.anchoredPosition = canvasPosition;

        Debug.DrawLine(
            m_box.bounds.center - m_box.bounds.size / 2f,
            m_box.bounds.center + m_box.bounds.size / 2f,
            Color.blue
        );
        Debug.DrawLine(
            m_box.bounds.center + new Vector3(-m_box.bounds.size.x / 2f, m_box.bounds.size.y / 2f),
            m_box.bounds.center + new Vector3(m_box.bounds.size.x / 2f, -m_box.bounds.size.y / 2f),
            Color.red
        );
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