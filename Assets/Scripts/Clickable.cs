using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(Collider2D))]
public class Clickable : MonoBehaviour
{
    public UnityEvent OnClick;
    [SerializeField] private int m_maxFramesBetweenDownAndUp = 10;
    [SerializeField] private Collider2D m_collider;

    private int m_lastMouseDownOver = 0;

    private void Start()
    {
        if (m_collider == null) {
            m_collider = GetComponent<Collider2D>();
        }
    }

    public bool IsMouseOver()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        var collisions = Physics2D.OverlapPointAll(mousePosition);
        bool mouseOverObject = false;
        if (collisions.Contains(m_collider))
        {
            mouseOverObject = true;
        }

        return mouseOverObject;
    }

    void Update()
    {
        bool mouseOverObject = IsMouseOver();
        if (!mouseOverObject)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            m_lastMouseDownOver = Time.frameCount;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_lastMouseDownOver + m_maxFramesBetweenDownAndUp >= Time.frameCount)
            {
                m_lastMouseDownOver = 0;
                OnClick?.Invoke();
            }
        }
    }
}