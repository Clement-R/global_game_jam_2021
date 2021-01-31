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

    [SerializeField] private List<StateToCameraPosition> m_cameraPositions;

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

    private void Start()
    {
        SetGameState(GameState.Intro);
    }

    public void SetGameState(GameState p_gameState)
    {
        State = p_gameState;
        var cameraPos = m_cameraPositions.First(e => e.State == p_gameState);
        Camera.main.transform.position = new Vector3(
            cameraPos.CameraPosition.x,
            cameraPos.CameraPosition.y,
            Camera.main.transform.position.z
        );
        OnGameStateChange?.Invoke(p_gameState);
    }
}

[System.Serializable]
public class StateToCameraPosition
{
    public GameState State;
    public Vector3 CameraPosition;
}

public enum GameState
{
    Intro = 0,
    Room = 1,
    Outro = 2
}