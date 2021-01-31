using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using DG.Tweening;

public class NarrativePanel : MonoBehaviour
{
    [SerializeField] private Image m_itemImage;
    [SerializeField] private TextMeshProUGUI m_itemDescription;
    [SerializeField] private CanvasGroup m_group;
    // Start is called before the first frame update
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

        m_itemImage.sprite = p_item.m_image.sprite;
        m_itemDescription.text = p_item.m_descriptionText;
        m_group.blocksRaycasts = true;
        m_group.interactable = true;
        m_group.DOFade(1f, 0.25f);

    }

    public void Close()
    {
        m_group.blocksRaycasts = false;
        m_group.interactable = false;
        m_group.DOFade(0f, 0.25f);
    }
}