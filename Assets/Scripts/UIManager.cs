using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Lean.Localization;

using TMPro;

using DG.Tweening;
using Random = UnityEngine.Random;
using System.Text;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_fader;
    [SerializeField] private CanvasGroup m_intro;
    [SerializeField] private CanvasGroup m_outro;

    [Header("Intro")]
    [SerializeField] private Clickable m_introEnvelopeClickable;
    [SerializeField] private TextRevealer m_clickToOpenText;
    [SerializeField] private float m_delayBeforeShowText = 5f;
    [SerializeField] private CanvasGroup m_introLetter;
    [SerializeField] private float m_introLetterFadeInDuration = 0.25f;
    [SerializeField] private Vector3 m_envelopeOutScreenPosition;
    [SerializeField] private GameObject m_envelope;
    [SerializeField] private float m_envelopeOutScreenDuration = 0.25f;
    [SerializeField] private TextRevealer m_letterText;
    [SerializeField] private float m_waitBetweenParagraphs = 0.1f;
    [SerializeField] private GameObject m_fakeBox;
    [SerializeField] private InventoryState m_inventoryState;
    [SerializeField] private GameObject m_handDragAnimation;
    [SerializeField] private Vector3 m_dragStart;
    [SerializeField] private Vector3 m_dragEnd;
    [SerializeField] private float m_dragAnimDuration;
    public bool RevealIntroDone
    {
        get;
        private set;
    } = false;

    [Header("Outro")]
    [SerializeField] private TextMeshProUGUI m_outroLetter;
    [SerializeField, LeanTranslationName] private string m_outroTradKey;

    [Header("Room")]
    [SerializeField] private Button m_toOutroButton;

    private bool m_clickToOpenVisible = false;
    private bool m_introLetterOpen = false;
    private Coroutine m_introRevealRoutine = null;
    private InventoryItem m_randomItem;

    private IEnumerator Start()
    {
        GameManager.Instance.OnGameStateChange += GameStateChanged;

        m_toOutroButton.onClick.AddListener(ToOutro);
        m_introEnvelopeClickable.OnClick.AddListener(IntroEnvelopeClick);

        LeanLocalization.OnLocalizationChanged += LanguageChanged;

        yield return null;

        m_clickToOpenText.Hide();
        m_handDragAnimation.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LeanLocalization.OnLocalizationChanged -= LanguageChanged;
    }

    private void ToOutro()
    {
        var items = Inventory.Instance.GetItemsInInventory();
        m_randomItem = items[Random.Range(0, items.Count)];
        SetOutroLetterText();

        GameManager.Instance.SetGameState(GameState.Outro);
    }

    private void SetOutroLetterText()
    {
        var item = LeanLocalization.GetTranslationText($"L_{m_randomItem.DescriptionKey}");
        var outro = LeanLocalization.GetTranslationText(m_outroTradKey);

        m_outroLetter.text = string.Format(outro, item);
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameState.Intro)
        {
            if (Time.time > m_delayBeforeShowText && !m_clickToOpenVisible && !m_introLetterOpen)
            {
                var loc = m_clickToOpenText.GetComponent<LeanLocalizedTextMeshProUGUI>();
                var text = Lean.Localization.LeanLocalization.GetTranslationText(loc.TranslationName);
                m_clickToOpenText.SetText(text);

                m_clickToOpenText.Restart();
                m_clickToOpenVisible = true;
            }
        }

        if (Inventory.Instance.IsFullEnough)
        {
            m_toOutroButton.gameObject.SetActive(true);
        }
        else
        {
            m_toOutroButton.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.SetGameState(GameState.Room);
        }
#endif
    }
    private void GameStateChanged(GameState p_newState)
    {
        if (p_newState == GameState.Intro)
        {
            // Show intro
            Show(m_intro);
            m_intro.alpha = 1f;

            PlayerSelection.Instance.Layer = LayerMask.GetMask("UI");
        }
        else if (p_newState == GameState.Outro)
        {
            // Show outro
            Show(m_outro);
            m_outro.DOFade(1f, 1f);
        }
        else
        {
            // Hide open screens
            if (m_intro.alpha == 1f)
            {
                Hide(m_intro);
                m_intro.alpha = 0f;
            }

            if (m_outro.alpha == 1f)
            {
                Hide(m_outro);
                m_outro.alpha = 0f;
            }

            PlayerSelection.Instance.Layer = LayerMask.GetMask("InventoryItem");
        }
    }

    private void IntroEnvelopeClick()
    {
        if (m_introLetterOpen)
            return;

        m_introLetterOpen = true;
        m_letterText.Hide();
        m_clickToOpenText.Hide();

        // Move envelope away and fade letter in
        m_envelope.transform.DOMove(m_envelopeOutScreenPosition, m_envelopeOutScreenDuration);
        m_introLetter.DOFade(1f, m_introLetterFadeInDuration).OnComplete(
            () =>
            {
                m_introLetter.blocksRaycasts = true;
                m_introLetter.interactable = true;

                m_introRevealRoutine = StartCoroutine(_RevealIntroLetter());
            }
        );

    }

    private void LanguageChanged()
    {
        if (!Application.isPlaying)
            return;

        if (!RevealIntroDone && m_introLetterOpen)
        {
            if (m_introRevealRoutine != null)
                StopCoroutine(m_introRevealRoutine);

            var loc = m_letterText.GetComponent<LeanLocalizedTextMeshProUGUI>();
            var text = Lean.Localization.LeanLocalization.GetTranslationText(loc.TranslationName);
            m_letterText.RestartWithText(text);
            m_introRevealRoutine = StartCoroutine(_RevealIntroLetter());
        }

        if (GameManager.Instance.State == GameState.Outro)
        {
            SetOutroLetterText();
        }
    }

    private IEnumerator _RevealIntroLetter()
    {
        var loc = m_letterText.GetComponent<LeanLocalizedTextMeshProUGUI>();
        var text = Lean.Localization.LeanLocalization.GetTranslationText(loc.TranslationName);
        m_letterText.SetText(text);

        RevealIntroDone = false;

        while (!m_letterText.IsAllRevealed())
        {
            yield return m_letterText.RevealNextParagraph();
            yield return new WaitForSeconds(m_waitBetweenParagraphs);
        }

        RevealIntroDone = true;
        m_fakeBox.transform.position = m_inventoryState.ClosePosition;

        m_handDragAnimation.gameObject.SetActive(true);
        m_handDragAnimation.transform.position = m_dragStart;
        m_handDragAnimation.transform.DOMove(m_dragEnd, m_dragAnimDuration)
            .SetLoops(-1, LoopType.Restart);
    }

    public void DropLetterInBox()
    {
        m_handDragAnimation.SetActive(false);

        GameManager.Instance.SetGameState(GameState.Room);
        m_fakeBox.gameObject.SetActive(false);
        m_fakeBox.gameObject.transform.position = Vector3.down * 100f;
        m_introEnvelopeClickable.gameObject.SetActive(false);

        FindObjectOfType<AudioManager>().Play("menu_begin");
    }

    private void Show(CanvasGroup p_group)
    {
        p_group.interactable = true;
        p_group.blocksRaycasts = true;
    }

    private void Hide(CanvasGroup p_group)
    {
        p_group.interactable = false;
        p_group.blocksRaycasts = false;
    }
}