using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private Image m_image;
    [SerializeField] private Button m_button;
    [SerializeField] private Sprite m_enSprite;
    [SerializeField] private Sprite m_frSprite;

    void Start()
    {
        m_button.onClick.AddListener(ToggleLanguage);

        Lean.Localization.LeanLocalization.OnLocalizationChanged += UpdateIcon;
    }

    private void ToggleLanguage()
    {
        if (Lean.Localization.LeanLocalization.CurrentLanguage == "English")
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "French";
        }
        else
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "English";
        }
    }

    private void UpdateIcon()
    {
        if (Lean.Localization.LeanLocalization.CurrentLanguage == "English")
        {
            m_image.sprite = m_enSprite;
        }
        else
        {
            m_image.sprite = m_frSprite;
        }
    }
}