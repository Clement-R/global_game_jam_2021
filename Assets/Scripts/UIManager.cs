using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_fader;
    [SerializeField] private CanvasGroup m_intro;
    [SerializeField] private CanvasGroup m_outro;

    [Header("Intro")]
    [SerializeField] private Clickable m_introEnvelopeClickable;
    [SerializeField] private TextRevealer m_clickToOpenText;
    [SerializeField] private float m_delayBeforeShowText = 5f;

    [Header("Outro")]
    [SerializeField] private TextMeshProUGUI m_outroLetterText;

    [Header("Room")]
    [SerializeField] private Button m_toOutroButton;

    private bool m_clickToOpenVisible = false;
    private bool m_introLetterOpen = false;

    private void Start()
    {
        m_toOutroButton.onClick.AddListener(ToOutro);
        GameManager.Instance.OnGameStateChange += GameStateChanged;
        m_introEnvelopeClickable.OnClick.AddListener(IntroEnvelopeClick);

        m_clickToOpenText.Hide();
    }

    private void ToOutro()
    {
        GameManager.Instance.SetGameState(GameState.Outro);
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameState.Intro)
        {
            if (Time.time > m_delayBeforeShowText && !m_clickToOpenVisible)
            {
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
    }
    private void GameStateChanged(GameState p_newState)
    {
        if (p_newState == GameState.Intro)
        {
            // Show intro
            Show(m_intro);
            m_intro.alpha = 1f;
        }
        else if (p_newState == GameState.Outro)
        {
            // Show outro
            Show(m_outro);
            m_outro.alpha = 1f;
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
        }
    }

    private void IntroEnvelopeClick()
    {
        if (m_introLetterOpen)
            return;

        Debug.Log("Open intro letter");
        m_introLetterOpen = true;
        m_introEnvelopeClickable.gameObject.SetActive(false);
        //TODO: show letter
        //TODO: move envelope away

        GameManager.Instance.SetGameState(GameState.Room);
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