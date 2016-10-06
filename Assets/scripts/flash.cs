using UnityEngine;
using UnityEngine.UI;

public class flash : MonoBehaviour {

    public SpriteRenderer sr;
    public Image img;
    bool flashOn;
    int i=0;
    public int flashSpeed;
    void FixedUpdate()
    {
        if (GameStateManager.instance.currentState != GameStateManager.GameState.Paused)
        {
            if (flashOn)
            {
                if (sr)
                    sr.enabled = !sr.enabled;
                if (img)
                    img.enabled = !img.enabled;
                flashOn = false;
            }
            else
            {
                i++;
                if (i > flashSpeed)
                {
                    flashOn = true;
                    i = 0;
                }
            }
        }
    }
}
