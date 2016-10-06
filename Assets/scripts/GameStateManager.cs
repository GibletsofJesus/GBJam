using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager instance;

    public enum GameState
    {
        Gameplay,
        finishLine,
        Paused,
    }

    public GameState currentState, previousState;

    void Awake()
    {
        instance = this;
    }

    public void ChangeState(GameState newSate)
    {
        previousState = currentState;
        currentState = newSate;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                PauseMenu.instance.gameObject.SetActive(false);
                currentState = previousState;
            }
            else
            {
                PauseMenu.instance.OpenPauseMenu();
                previousState = currentState;
                currentState = GameState.Paused;
            }
        }
    }
}
