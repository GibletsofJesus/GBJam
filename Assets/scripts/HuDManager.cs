using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HuDManager : MonoBehaviour {

    public Text speedText,killText;
    public int kills;
    public static HuDManager instance;

    [Header("Minimap")]
    public Image playerCursor;

    public Text TimerText;
    float gameTimer=74;

    void Start()
    {
        instance = this;
    }

    public void UpdatePlayerCursor(float distance)
    {
        int newOffset = (int)Mathf.Lerp(-43f,47f,distance);
        playerCursor.rectTransform.anchoredPosition = new Vector2(newOffset+0.5f, -64);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
            gameTimer -= Time.deltaTime;

        string seconds = "" + (int)gameTimer % 60;
        if (seconds.Length < 2)
            seconds = "0" + seconds;

        TimerText.text = ((int)gameTimer / 60) % 60 + ":" + seconds;
    }
}
