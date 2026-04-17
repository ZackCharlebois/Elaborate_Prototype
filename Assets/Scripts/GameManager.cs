using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    Starting = 1,
    Playing = 10,
    Paused = 15,
    FailScreen = 20,
    VictoryDance = 25
}

public class GameManager : Singleton<GameManager>
{
    private Animator _fadeAnimator;
    
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    
    public GameState State { get; private set; }

    private GameState _previousState = GameState.Starting;
    void Start()
    {
        _fadeAnimator = GetComponentInChildren<Animator>();
        ChangeState(GameState.Starting);
    }

    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        _previousState = State;
        
        State = newState;
        
        //if we were just paused, handle resuming
        if (_previousState == GameState.Paused)
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
        }

        switch (newState)
        {
            case GameState.Starting:
                StartCoroutine(HandleStarting());
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                AudioListener.pause = true;
                break;
            case GameState.FailScreen:
                break;
            case GameState.VictoryDance:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        
        OnAfterStateChanged?.Invoke(newState);
        
    }

    private IEnumerator HandleStarting()
    {
        Time.timeScale = 0f;
        _fadeAnimator.Play("Fade In");
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1f;
        ChangeState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (State == GameState.Paused)
            ChangeState(_previousState);
        else
            ChangeState(GameState.Paused);
    }
    
}