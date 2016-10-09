using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public static PauseMenu instance;

    int menuIndex;
    public float scrollSpeed;
    float scrollCD;
    [Header("Main Menu")]
    [SerializeField]
    Text[] MenuItems;
    [SerializeField]
    Image[] selectionIndicators;
    public Vector2 offset;

    [Header("Options menu")]
    [SerializeField]
    Text[] OptionItems;
    [SerializeField]
    GameObject swapBox;
    [SerializeField]
    Text PaletteSwapText;

    menuState currentState;
    enum menuState
    {
        main,
        option,
        paletteSwap,
    }
    
    // Use this for initialization
    void Start()
    {
        instance = this;
        gameObject.SetActive(false);
        paletteSwapper.SetPaletteIndex(0, PaletteSwapText);
    }

    [SerializeField]
    PaletteSwapLookup paletteSwapper;

    // Update is called once per frame
    void Update()
    {
        if (scrollCD > 0)
            scrollCD -= Time.deltaTime;

        if (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
        {
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 && scrollCD <= 0)
            {
                scrollCD = scrollSpeed;
                if (currentState != menuState.paletteSwap)
                {
                    if (Input.GetAxis("Vertical") > 0)
                        menuIndex--;
                    else
                        menuIndex++;

                    if (menuIndex == MenuItems.Length)
                        menuIndex = 0;

                    if (menuIndex < 0)
                        menuIndex = MenuItems.Length - 1;

                    ChangeSelection();
                }
                else
                {
                    paletteSwapper.enabled = true;
                    if (Input.GetAxis("Vertical") > 0)
                        paletteSwapper.SetPaletteIndex(1,PaletteSwapText);
                    else
                        paletteSwapper.SetPaletteIndex(-1,PaletteSwapText);
                }
            }

            if (Input.GetButtonDown("B"))
            {
                switch (currentState)
                {
                    case menuState.main:                        
                        switch (menuIndex)
                        {
                            case 0:
                                GameStateManager.instance.ChangeState(GameStateManager.instance.previousState);
                                gameObject.SetActive(false);
                                break;
                            case 1:
                                Application.LoadLevel(1);
                                break;
                            case 2:
                                currentState = menuState.option;
                                menuIndex = 0;
                                ChangeSelection();
                                foreach (Text t in OptionItems)
                                    t.enabled = true;
                                foreach (Text t in MenuItems)
                                    t.enabled = false;
                                break;
                            case 3:
                                //Quit
                                Application.Quit();
                                break;
                        }
                        break;
                    case menuState.option:
                        switch (menuIndex)
                        {
                            case 0:
                                //Yeah I'll do some shit here at some point
                                break;
                            case 1:
                                currentState = menuState.paletteSwap;
                                swapBox.SetActive(true);
                                break;
                            case 2:
                                if (SoundManager.instance.volumeMultiplayer != 1)
                                    SoundManager.instance.changeVolume(SoundManager.instance.volumeMultiplayer + 0.1f);
                                else
                                    SoundManager.instance.changeVolume(0);

                                OptionItems[2].text = "Sound FX - " +SoundManager.instance.volumeMultiplayer*100f;
                                break;
                            case 3:
                                //Back
                                currentState = menuState.main;
                                menuIndex = 0;
                                ChangeSelection();
                                foreach (Text t in OptionItems)
                                    t.enabled = false;
                                foreach (Text t in MenuItems)
                                    t.enabled = true;
                                break;
                        }
                        break;
                    case menuState.paletteSwap:
                        currentState = menuState.option;
                        swapBox.SetActive(false);
                        break;
                }
            }
        }
    }

    public void OpenPauseMenu()
    {
        gameObject.SetActive(true);
        menuIndex = 0;
        foreach (Text t in OptionItems)
            t.enabled = false;
        foreach (Text t in MenuItems)
            t.enabled = true;
        swapBox.SetActive(false);
        currentState = menuState.main;
        ChangeSelection();
    }

    public void ChangeSelection()
    {

        float x=0;
        float y=0;

        switch (currentState)
        {
            case menuState.main:
                x = offset.x + MenuItems[menuIndex].rectTransform.sizeDelta.x / 2;
                y = offset.y + MenuItems[menuIndex].rectTransform.anchoredPosition.y;
                break;
            case menuState.option:
                x = offset.x + OptionItems[menuIndex].rectTransform.sizeDelta.x / 2;
                y = offset.y + OptionItems[menuIndex].rectTransform.anchoredPosition.y;
                break;
        }
        

        selectionIndicators[0].rectTransform.anchoredPosition = new Vector2(-x, y);
        selectionIndicators[1].rectTransform.anchoredPosition = new Vector2(x, y);
    }
}
