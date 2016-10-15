using UnityEngine;
using System.Collections;

//Create tiny pauses for certain events
//i.e. clashing of beaks, defeat of geese etc.
public class frameHolder : MonoBehaviour {

    public static frameHolder instance;
    public float normalSpeed=1;
    public bool enableFrameholding = true;

    public void holdFrame(float time)
    {
        if (enableFrameholding)
        {
            holdDuration += time;
            if (Time.timeScale > 0)
                StartCoroutine(hold());
        }
    }

    bool running;
    float holdDuration;
    IEnumerator hold()
    {
        while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
        {
            Time.timeScale = 0;
            yield return null;
        }

        if (enableFrameholding)
        {
            Time.timeScale = 0.1f;

            while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
            {
                Time.timeScale = 0;
                yield return null;
            }

            while (holdDuration > 0)
            {
                holdDuration -= Time.fixedDeltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
        {
            Time.timeScale = 0;
            yield return null;
        }
        Time.timeScale = normalSpeed;
    }

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(fadeMusic(true));
        instance = this;
	}

    void Update()
    {
        if (holdDuration <= 0 && Time.timeScale != normalSpeed)
        {
            StopAllCoroutines();
            StartCoroutine(fadePitch(normalSpeed == 1 ? true : false));
            //SoundManager.instance.managedAudioSources[2].AudioSrc.pitch = normalSpeed;
            Time.timeScale = normalSpeed;
        }
    }

    IEnumerator fadePitch(bool b)
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime*5;
            SoundManager.instance.managedAudioSources[2].AudioSrc.pitch = Mathf.Lerp(0.25f, 1, b ? lerpy : 1 - lerpy);
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator fadeMusic(bool b)
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime;
            SoundManager.instance.managedAudioSources[2].AudioSrc.volume = Mathf.Lerp(0, 1, b ? lerpy : 1 - lerpy);
            yield return new WaitForEndOfFrame();
        }
    }
}