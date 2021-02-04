using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using DG.Tweening;

public class InteractiveMusic : MonoBehaviour
{
    [SerializeField] AudioMixer m_mainMixer;

    private const string AMB_GENERAL_VOLUME = "Volume AmbGeneral";
    private const string AMB_MENU_VOLUME = "Volume AmbMenu";
    private const string BASS_VOLUME = "Volume Bass";
    private const string PIANO_VOLUME = "Volume Piano";
    private const string STRINGS_01_VOLUME = "Volume Strings 01";
    private const string STRINGS_02_VOLUME = "Volume Strings 02";
    private const string WIND_CLAP_VOLUME = "Volume WindClap";

    private Tween m_pianoTween;
    private Tween m_bassTween;
    private Tween m_windClapTween;

    void Start()
    {
        GameManager.Instance.OnGameStateChange += GameStateChange;
    }

    private void Update()
    {
        var itemsInInventory = Inventory.Instance.GetItemsInInventory().Count;
        if (itemsInInventory > 0)
        {
            InventoryUpdate(itemsInInventory);
        }
    }

    private void GameStateChange(GameState p_state)
    {
        switch (p_state)
        {
            case GameState.Intro:
                Intro();
                break;
            case GameState.Room:
                Game();
                break;
            case GameState.Outro:
                Outro();
                break;
        }
    }

    private void ResetVolumes()
    {
        m_mainMixer.SetFloat(STRINGS_01_VOLUME, -80f);
        m_mainMixer.SetFloat(STRINGS_02_VOLUME, -80f);
        m_mainMixer.SetFloat(PIANO_VOLUME, -80f);
        m_mainMixer.SetFloat(BASS_VOLUME, -80f);
        m_mainMixer.SetFloat(WIND_CLAP_VOLUME, -80f);
        m_mainMixer.SetFloat(AMB_GENERAL_VOLUME, -80f);
        m_mainMixer.SetFloat(AMB_MENU_VOLUME, -80f);
    }

    private void Intro()
    {
        ResetVolumes();

        m_mainMixer.DOSetFloat(STRINGS_01_VOLUME, 0f, 8f).From(-80f);
        m_mainMixer.DOSetFloat(AMB_MENU_VOLUME, 0f, 8f).From(-80f);
    }

    private void Game()
    {
        m_mainMixer.DOSetFloat(STRINGS_02_VOLUME, 0f, 15f).From(-80f);
        m_mainMixer.DOSetFloat(AMB_MENU_VOLUME, -80f, 12f).From(0f);
        m_mainMixer.DOSetFloat(AMB_GENERAL_VOLUME, 0f, 12f).From(-80f);
    }

    private void InventoryUpdate(int p_numOfItems)
    {
        if (p_numOfItems == 1)
        {
            m_mainMixer.GetFloat(PIANO_VOLUME, out float volume);
            if (m_pianoTween == null && volume != 0f)
            {
                m_pianoTween = m_mainMixer.DOSetFloat(PIANO_VOLUME, 0f, 15f).From(-80f).OnComplete(
                    () =>
                    {
                        m_pianoTween = null;
                    }
                );
            }
        }
        else
        {
            m_mainMixer.GetFloat(BASS_VOLUME, out float bassVolume);
            if (m_bassTween == null && bassVolume != 0f)
            {
                m_bassTween = m_mainMixer.DOSetFloat(BASS_VOLUME, 0f, 15f).From(-80f).OnComplete(
                    () =>
                    {
                        m_bassTween = null;
                    }
                );
            }

            m_mainMixer.GetFloat(WIND_CLAP_VOLUME, out float windClapVolume);
            if (m_windClapTween == null && windClapVolume != 0f)
            {
                m_windClapTween = m_mainMixer.DOSetFloat(WIND_CLAP_VOLUME, 0f, 15f).From(-80f).OnComplete(
                    () =>
                    {
                        m_windClapTween = null;
                    }
                );
            }
        }
    }

    private void Outro()
    {
        m_mainMixer.DOSetFloat(STRINGS_01_VOLUME, -80f, 4f).From(0f);
        m_mainMixer.DOSetFloat(STRINGS_02_VOLUME, -80f, 7.5f).From(0f);
        m_mainMixer.DOSetFloat(PIANO_VOLUME, -80f, 15f).From(0f);

        m_mainMixer.DOSetFloat(AMB_MENU_VOLUME, 0f, 8f).From(-80f);
        m_mainMixer.DOSetFloat(AMB_GENERAL_VOLUME, -80f, 8f).From(0f);
    }
}