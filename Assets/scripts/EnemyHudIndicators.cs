using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorPairing
{
    public Enemy associatedEnemy = null;
    public Image indicator = null;
    public Text distanceText = null;
    public Text distanceTextOutline = null;
    public Transform trackMe;
    public float timer;
    public IndicatorPairing(Enemy e, Image i,Text t,Text t2)
    {
        associatedEnemy = e;
        indicator = i;
        distanceText = t;
        distanceTextOutline = t2;
    }
}

public class EnemyHudIndicators : MonoBehaviour
{

    public static EnemyHudIndicators instance;
    [SerializeField]
    Image[] indicators;

    IndicatorPairing[] pairings;
    
    // Use this for initialization
    void Start()
    {
        pairings = new IndicatorPairing[indicators.Length];
        for (int i=0;i<indicators.Length;i++)
        {
            indicators[i].rectTransform.anchoredPosition = new Vector2(8, 0);
            pairings[i] = new IndicatorPairing(null, indicators[i], indicators[i].GetComponentsInChildren<Text>()[0], indicators[i].GetComponentsInChildren<Text>()[1]);
            pairings[i].trackMe = null;
        }
        instance = this;
    }

    public void SetIndicator(Vector3 pos, Enemy enemyRef)
    {
        foreach (IndicatorPairing ip in pairings)
        {
            if (ip.associatedEnemy == null)
            {
                ip.associatedEnemy = enemyRef;
                ip.indicator.rectTransform.anchoredPosition = new Vector2(-17,
            Camera.main.WorldToScreenPoint(Vector3.one * pos.y).y - 72);
                ip.trackMe = enemyRef.transform;
            }
        }
    }

    public void ResetIndicator(IndicatorPairing ip)
    {
        ip.associatedEnemy = null;
        ip.indicator.rectTransform.anchoredPosition = new Vector2(8, 0);
    }

    // Update is called once per frame
    void Update()
{
        foreach (IndicatorPairing ip in pairings)
        {
            if (ip.trackMe != null)
            {
                if (ip.distanceText.text == "" + (ip.trackMe.position.x - 90))
                    ip.timer += Time.deltaTime;

                if (ip.timer > 0.1f)
                    ResetIndicator(ip);

                ip.indicator.rectTransform.anchoredPosition = new Vector2(-17,
            Camera.main.WorldToScreenPoint(Vector3.one * ip.trackMe.position.y).y
            - 72);

                if (ip.trackMe.position.x - 90 < 0)
                {
                    ResetIndicator(ip);
                    break;
                }
                else
                {
                    ip.distanceText.text = "" + (ip.trackMe.position.x - 90);
                    ip.distanceTextOutline.text = "" + (ip.trackMe.position.x - 90);
                }
            }
        }

    }
}
