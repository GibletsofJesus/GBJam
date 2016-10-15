using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    public static CameraShake instance;
    // How long the object should shake for.
    public float shakeDuration = 0f;
    public float shakeMultiplier=1;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    void OnEnable()
    {
        instance = this;
    }

    public void Shake()
    {
        shakeDuration += .1f;
    }

    void Update()
    {
        if (GameStateManager.instance.currentState != GameStateManager.GameState.Paused)
        {
            if (shakeDuration > 0)
            {
                Camera.main.transform.localPosition = (Vector3.up * shakeAmount*shakeMultiplier) + CameraMover.instance.standardPosition + Random.insideUnitSphere * shakeAmount*shakeMultiplier;
                shakeDuration -= Time.deltaTime * decreaseFactor;

                if (shakeDuration <= 0)
                {
                    Camera.main.transform.position = CameraMover.instance.standardPosition;
                }
            }
        }
    }
}
