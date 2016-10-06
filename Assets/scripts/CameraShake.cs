using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    public static CameraShake instance;
    // How long the object should shake for.
    public float shakeDuration = 0f;

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
                Vector3 newPos = -(Player.instance.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)) / 3;
                newPos = Player.instance.transform.position + (newPos / 1.5f);

                Camera.main.transform.localPosition = (Vector3.up * shakeAmount) + CameraMover.instance.standardPosition + Random.insideUnitSphere * shakeAmount;
                shakeDuration -= Time.deltaTime * decreaseFactor;

                if (shakeDuration <= 0)
                {
                    Camera.main.transform.position = CameraMover.instance.standardPosition;
                }
            }
        }
    }
}
