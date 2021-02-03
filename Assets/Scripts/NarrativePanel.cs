using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using DG.Tweening;

public class NarrativePanel : MonoBehaviour
{
    public static NarrativePanel Instance => m_instance;
    private static NarrativePanel m_instance = null;

    [SerializeField] private Image m_itemImage;
    [SerializeField] private TextMeshProUGUI m_itemDescription;
    [SerializeField] private CanvasGroup m_group;

    private bool m_isOpen => m_group.blocksRaycasts;

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

    void Start()
    {
        m_group.alpha = 0;
        m_group.blocksRaycasts = false;
        m_group.interactable = false;
    }

    public void ShowItem(InventoryItem p_item)
    {
        if (GameManager.Instance.State != GameState.Room)
            return;

        if (m_isOpen)
            return;

        m_itemImage.sprite = p_item.m_image.sprite;
        m_itemDescription.text = p_item.m_descriptionText;
        m_group.blocksRaycasts = true;
        m_group.interactable = true;
        m_group.DOFade(1f, 0.25f);
    }

    public void Close()
    {
        StartCoroutine(_DelayedClose());
    }

    private IEnumerator _DelayedClose()
    {
        yield return null;
        m_group.blocksRaycasts = false;
        m_group.interactable = false;
        m_group.DOFade(0f, 0.25f);
    }
}