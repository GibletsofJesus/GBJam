using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HuDManager : MonoBehaviour {

    public Text speedText,killText;
    public int kills;
    public static HuDManager instance;

    [Header("Minimap")]
    public Image playerCursor;

    public Color altColor;
    public Text TimerText;
    public Image SpeedBarA, SpeedBarB,mainSpeedBar;
    public float gameTimer=0;

    void Start()
    {
        instance = this;
    }

    public void UpdateSpeedBar(float speed)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateSpeedBooora(speed));
    }

    IEnumerator UpdateSpeedBooora(float f)
    {
        yield return new WaitForEndOfFrame();
        float startA = SpeedBarA.fillAmount, StartB = SpeedBarB.fillAmount;

        float lerpy = 0;
        while (lerpy < 1)
        {            //mainSpeedBar.color = Color.white;
            lerpy += Time.deltaTime * 5;
            SpeedBarA.fillAmount = Mathf.Lerp(startA, f / 25, lerpy);
            SpeedBarB.fillAmount = Mathf.Lerp(StartB, (f - 25) / 25, lerpy);
            yield return new WaitForEndOfFrame();
        }
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
            gameTimer += Time.deltaTime/frameHolder.instance.normalSpeed;

        string seconds = "" + (int)gameTimer % 60;
        if (seconds.Length < 2)
            seconds = "0" + seconds;

        TimerText.text = ((int)gameTimer / 60) % 60 + ":" + seconds;
    }
}
