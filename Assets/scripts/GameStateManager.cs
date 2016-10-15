using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour {

    public static GameStateManager instance;

    public ParticleSystem[] systems;

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
        if (Input.GetButtonDown("Start") && previousState != GameState.finishLine && currentState == GameState.Gameplay)
        {
            if (currentState == GameState.Paused)
            {
                foreach (ParticleSystem ps in systems)
                {
                    ps.Play();
                }
                SoundManager.instance.managedAudioSources[2].AudioSrc.UnPause();
                //SoundManager.instance.PauseEvyerthing(false);
                PauseMenu.instance.gameObject.SetActive(false);
                currentState = previousState;
            }
            else
            {
                foreach (ParticleSystem ps in systems)
                {
                    ps.Pause();
                }
                SoundManager.instance.managedAudioSources[2].AudioSrc.Pause();
                PauseMenu.instance.OpenPauseMenu();
                previousState = currentState;
                currentState = GameState.Paused;
            }
        }
    }
}
