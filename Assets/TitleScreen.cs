using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreen : MonoBehaviour {

    int menuIndex;
    [SerializeField]
    public Text[] MenuItems;
    [SerializeField]
    Image indicator,sweeper;
    [SerializeField]
    GameObject Credits;
    public float scrollSpeed;
    float scrollCD;

    [SerializeField]
    TextTyper Goofers;
    PaletteSwapLookup paletteSwapper;

    void Start()
    {
        paletteSwapper = GameObject.Find("PixelCamera2D").GetComponent<PaletteSwapLookup>();
        StartCoroutine(coolText());
    }

    [SerializeField]
    Text[] TitleTexts;
    [SerializeField]
    GameObject swapBox;
    [SerializeField]
    Text PaletteSwapText;

    IEnumerator StartGame()
    {
        //speed off
        yield return new  WaitForEndOfFrame();

        Application.LoadLevel(1);
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
        
        Vector2 normalPosition = new Vector2(0, 60);

        int posIndex = 0;
        while (posIndex < 99)
        {
            for (int i = 0; i < TitleTexts.Length; i++)
            {
                TitleTexts[i].rectTransform.anchoredPosition = normalPosition + (positions[posIndex]* (i+1));
            }
            posIndex++;
            if (posIndex > positions.Length - 1)
                posIndex = 0;
            yield return new WaitForSeconds(0.03333f);

            while (Credits.activeSelf)
                yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollCD > 0)
            scrollCD -= Time.deltaTime;

        if (!Credits.activeSelf && !Goofers.gameObject.activeSelf)
        {
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 && scrollCD <= 0)
            {
                scrollCD = scrollSpeed;

                if (!swapBox.activeSelf)
                {
                    if (Input.GetAxis("Vertical") > 0)
                        ChangeSelection(-1);
                    else
                        ChangeSelection(1);
                }
                else
                {
                    paletteSwapper.enabled = true;
                    if (Input.GetAxis("Vertical") > 0)
                        paletteSwapper.SetPaletteIndex(1, PaletteSwapText);
                    else
                        paletteSwapper.SetPaletteIndex(-1, PaletteSwapText);
                }
            }
        }
        if (Input.GetButtonDown("Fire1") && !Goofers.gameObject.activeSelf)
        {
            if (!swapBox.activeSelf)
            {
                switch (menuIndex)
                {
                    case 0:
                        StartCoroutine(StartGame());
                        break;
                    case 1:
                        //Get goofed on
                        Goofers.gameObject.SetActive(true);
                        Goofers.TutorialTime();
                        break;
                    case 2:
                        swapBox.SetActive(true);
                        break;
                    case 3:
                        indicator.gameObject.SetActive(Credits.activeSelf);
                        Credits.SetActive(!Credits.activeSelf);
                        break;
                    case 4:
                        Application.Quit();
                        break;
                }
            }
            else
            {
                swapBox.SetActive(false);
            }
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
