using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreen : MonoBehaviour {

    int menuIndex;
    [SerializeField]
    public Text[] MenuItems;
    [SerializeField]
    Image indicator,sweeper;
    public float scrollSpeed;
    float scrollCD;
    
    void Start()
    {
        StartCoroutine(coolText());
    }

    [SerializeField]
    Text[] TitleTexts;

    IEnumerator StartGame()
    {
        //speed off
        yield return new  WaitForEndOfFrame();
    }

    IEnumerator coolText()
    {
        Vector2[] positions = new Vector2[]
        {
            new Vector2(1,1),
            new Vector2(0,1),
            new Vector2(-1,1),
            new Vector2(-1,0),
            new Vector2(-1,-1),
            new Vector2(0,-1),
            new Vector2(1,-1),
            new Vector2(1,0),
        };
        
        Vector2 normalPosition = new Vector2(0, 56);

        int posIndex = 0;
        while (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
        {
            for (int i = 0; i < TitleTexts.Length; i++)
            {
                TitleTexts[i].rectTransform.anchoredPosition = normalPosition + (positions[posIndex]* (i+1));
            }
            posIndex++;
            if (posIndex > positions.Length - 1)
                posIndex = 0;
            yield return new WaitForSeconds(0.03333f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollCD > 0)
            scrollCD -= Time.deltaTime;

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 && scrollCD <= 0)
        {
            scrollCD = scrollSpeed;

            if (Input.GetAxis("Vertical") > 0)
                ChangeSelection(-1);
            else
                ChangeSelection(1);
        }
    }

    public Vector2 offset;

    void ChangeSelection(int dir)
    {
        MenuItems[menuIndex].rectTransform.anchoredPosition -= new Vector2(-1, -1);

        menuIndex += dir;

        if (menuIndex == MenuItems.Length)
            menuIndex = 0;
        if (menuIndex < 0)
            menuIndex = MenuItems.Length - 1;

        float x = offset.x + MenuItems[menuIndex].rectTransform.sizeDelta.x / 2;
        float y = offset.y + MenuItems[menuIndex].rectTransform.anchoredPosition.y;

        MenuItems[menuIndex].rectTransform.anchoredPosition += new Vector2(-1, -1);

        indicator.rectTransform.anchoredPosition = new Vector2(-x, y);
    }
}
