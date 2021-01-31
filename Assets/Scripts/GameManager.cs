using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Action<GameState> OnGameStateChange;

    public GameState State
    {
        get;
        private set;
    } = GameState.Intro;

    public static GameManager Instance => m_instance;
    private static GameManager m_instance = null;

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

    private IEnumerator Start()
    {
        yield return null;
        SetGameState(GameState.Intro);
    }

    public void SetGameState(GameState p_gameState)
    {
        State = p_gameState;
        OnGameStateChange?.Invoke(p_gameState);
    }
}

public enum GameState
{
    Intro = 0,
    Room = 1,
    Outro = 2
}