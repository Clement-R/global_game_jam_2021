using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Lean.Localization;

using DG.Tweening;

public class InventoryItem : MonoBehaviour
{
    [Header("Translation")]
    [LeanTranslationName] public string DescriptionKey;

    [Header("Item Info")]
    [HideInInspector] public Vector3Int CellIndex;
    [SerializeField] public SpriteRenderer m_image;

    [SerializeField] public string m_descriptionText;

    [SerializeField] private string m_baseLayer;
    [SerializeField] private string m_grabLayer;
    [SerializeField] private float m_minDistance;

    [Header("Shape outline")]
    [SerializeField] private SpriteRenderer m_shapeOutline;
    [SerializeField] private Transform m_shapeContainer;
    [SerializeField] private Color m_shapeBaseColor;
    [SerializeField] private Color m_shapeInvalidColor;

    [Header("Outline")]
    [SerializeField] private SpriteRenderer m_outline;
    [SerializeField] private List<Sprite> m_outlineFrames;

    public bool[, ] m_shape = new bool[5, 5];
    //le point de rotation est celui du centre
    public Inventory.Objet_id m_id;

    [SerializeField] private Collider2D m_collider;
    private bool m_isColliding => m_inCollisionWith.Count > 0;
    private HashSet<GameObject> m_inCollisionWith = new HashSet<GameObject>();

    private Vector3 m_basePosition;
    private Quaternion m_baseRotation;
    private Transform m_baseParent;
    private Inventory m_inventory;
    private Sequence m_outlineTween = null;

    void Start()
    {
        m_inventory = Inventory.Instance;

        m_basePosition = transform.position;
        m_baseRotation = transform.rotation;
        m_baseParent = transform.parent;

        if (m_collider == null)
        {
            m_collider = GetComponent<Collider2D>();
        }
        m_collider.isTrigger = true;
    }

    void ChangeSpriteLayerToGrab(bool p_grab)
    {
        if (p_grab)
        {
            m_image.sortingLayerName = m_grabLayer;
            m_outline.sortingLayerName = m_grabLayer;
            m_shapeOutline.sortingLayerName = m_grabLayer;
        }
        else
        {
            m_image.sortingLayerName = m_baseLayer;
            m_outline.sortingLayerName = m_baseLayer;
            m_shapeOutline.sortingLayerName = m_baseLayer;
        }
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
        ChangeSpriteLayerToGrab(false);

        UpdateShapeOutline(false, true);
    }

    private void OnTriggerStay2D(Collider2D p_other)
    {
        if (p_other.transform.parent == null)
            return;

        if (p_other.transform.parent.parent == m_inventory.transform)
        {
            m_inCollisionWith.Add(p_other.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D p_other)
    {
        if (p_other.transform.parent == null)
            return;

        if (p_other.transform.parent.parent == m_inventory.transform)
        {
            m_inCollisionWith.Add(p_other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D p_other)
    {
        if (p_other.transform.parent == null)
            return;

        if (p_other.transform.parent.parent == m_inventory.transform)
        {
            m_inCollisionWith.Remove(p_other.gameObject);
        }
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
        if (p_outlined)
        {
            // Start animation
            if (m_outlineTween == null)
            {
                m_outlineTween = DOTween.Sequence();
                foreach (var sprite in m_outlineFrames)
                {
                    m_outlineTween
                        .AppendCallback(
                            () =>
                            {
                                m_outline.sprite = sprite;
                            }
                        )
                        .AppendInterval(0.15f);
                }
                m_outlineTween.SetLoops(-1, LoopType.Restart);
            }
        }
        else
        {
            // Stop animation
            if (m_outlineTween != null)
            {
                m_outlineTween.Kill();
                m_outlineTween = null;
            }
        }

        m_outline.gameObject.SetActive(p_outlined);
    }

    private void UpdateShapeOutline(bool p_isVisible, bool p_isValid)
    {
        m_shapeOutline.gameObject.SetActive(p_isVisible);
        m_shapeOutline.color = p_isValid ? m_shapeBaseColor : m_shapeInvalidColor;
    }

    private void GrabObject()
    {
        PlayerSelection.Instance.SetSelectedItem(this);
        ChangeSpriteLayerToGrab(true);
        FindObjectOfType<AudioManager>().Play("grab");
    }

    private Coroutine m_grabCheckRoutine;

    private IEnumerator _StartGrabCheck(Vector3 p_startPosition)
    {
        bool isGrabbed = false;
        while (!isGrabbed)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
            if (Vector3.Distance(p_startPosition, mousePosition) > m_minDistance)
            {
                isGrabbed = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_grabCheckRoutine != null)
                {
                    m_grabCheckRoutine = null;
                }

                yield break;
            }

            yield return null;
        }

        GrabObject();
        m_grabCheckRoutine = null;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameState.Room)
            return;

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
            if (!PlayerSelection.Instance.HasASelectedItem && mouseOverObject && m_grabCheckRoutine == null)
                m_grabCheckRoutine = StartCoroutine(_StartGrabCheck(mousePosition));
        }

        // If item is not selected, stop there
        if (PlayerSelection.Instance.SelectedItem != this)
        {
            UpdateOutline(mouseOverObject);
            UpdateShapeOutline(false, true);
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
            m_shapeContainer.transform.position = m_inventory.GetPositionFromCellIndex(CellIndex);
        }
        else
        {
            m_shapeContainer.transform.localPosition = Vector3.zero;
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
                m_shapeContainer.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.Log($"Can't put in inventory Not in box: {notInBox} - Colliding: {colliding}");
                if (colliding)
                {
                    foreach (var col in m_inCollisionWith)
                    {
                        Debug.Log($"{col.name} - {col.transform.parent.name}");
                    }
                }
                ResetPositionAndRotation();
            }

            PlayerSelection.Instance.SetSelectedItem(null);

            FindObjectOfType<AudioManager>().Play("drop");
        }
        else
        {
            // Mouse follow
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
        }

        UpdateShapeOutline(overInventory, canPutInInventory && !colliding);
        if (overInventory)
        {
            UpdateOutline(false);
        }
    }
}