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

    public menuState state = menuState.main;

    public enum menuState
    {
        paletteSwap,
        main,
        mode,
        credits,
        tutorial,
        noInput,
    }

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

    [Header("Mode selection")]
    [SerializeField]
    Image[] modeIndicators;
    [SerializeField]
    GameObject modeBox, recordBox;
    [SerializeField]
    Text TimePB, speedPB;
    [SerializeField]
    Text[] modeOptions;
    int modeIndex;

    [SerializeField]
    Renderer sceneTransition;
    
    IEnumerator SceneTransition()
    {
        float lerpy = 0;

        while (lerpy < 1)
        {
            lerpy += Time.deltaTime*2;
            sceneTransition.material.SetColor("_TintColour", Color.Lerp(Color.white, Color.black, lerpy));
            sceneTransition.material.SetFloat("_SliceAmount", 1-lerpy);
            yield return new WaitForEndOfFrame();
        }
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

    public AudioClip moveSound;

    // Update is called once per frame
    void Update()
    {
        if (scrollCD > 0)
            scrollCD -= Time.deltaTime;

        #region vertical things
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 && scrollCD <= 0)
        {
            if (state == menuState.main)
            {
                SoundManager.instance.playSound(moveSound, 1, 0.25f + ((float)(MenuItems.Length - menuIndex) / (float)MenuItems.Length));
                scrollCD = scrollSpeed;

                if (Input.GetAxis("Vertical") > 0)
                    ChangeSelection(-1);
                else
                    ChangeSelection(1);
            }
            else if (state == menuState.paletteSwap)
            {
                SoundManager.instance.playSound(moveSound, 1, 0.25f + ((float)(MenuItems.Length - menuIndex) / (float)MenuItems.Length));
                scrollCD = scrollSpeed;

                paletteSwapper.enabled = true;
                if (Input.GetAxis("Vertical") > 0)
                    paletteSwapper.SetPaletteIndex(1, PaletteSwapText);
                else
                    paletteSwapper.SetPaletteIndex(-1, PaletteSwapText);
            }
            else if (state == menuState.mode)
            {
                SoundManager.instance.playSound(moveSound, 1, 0.25f + ((float)(modeOptions.Length - modeIndex) / (float)modeOptions.Length));
                scrollCD = scrollSpeed;

                if (Input.GetAxis("Vertical") > 0)
                    modeScroll(-1);
                else
                    modeScroll(1);
            }
        }
#endregion

        #region pressing A
        if (Input.GetButtonDown("A"))
        {
            if (state == menuState.main)
            {
                switch (menuIndex)
                {
                    case 0:
                        //mode select goes here
                        state = menuState.mode;
                        modeScroll(0);

                        modeBox.SetActive(true);
                        recordBox.SetActive(true);
                        break;
                    case 1:
                        //Get goofed on
                        state = menuState.tutorial;
                        Goofers.gameObject.SetActive(true);
                        Goofers.TutorialTime(this);
                        break;
                    case 2:
                        state = menuState.paletteSwap;
                        swapBox.SetActive(true);
                        break;
                    case 3:
                        state = menuState.credits;
                        indicator.gameObject.SetActive(Credits.activeSelf);
                        Credits.SetActive(!Credits.activeSelf);
                        break;
                    case 4:
                        Application.Quit();
                        break;
                }
            }
            else if (state == menuState.paletteSwap)
            {
                state = menuState.main;
                swapBox.SetActive(false);
            }
            else if (state == menuState.credits)
            {
                state = menuState.main;
                indicator.gameObject.SetActive(Credits.activeSelf);
                Credits.SetActive(!Credits.activeSelf);
            }
            else if (state == menuState.mode)
            {
                PlayerPrefs.SetInt("mode", modeIndex);
                StartCoroutine(SceneTransition());
                state = menuState.noInput;
            }

        }
        #endregion

        #region pressing B
        if (Input.GetButtonDown("B"))
        {
            if (state == menuState.mode)
            {
                state = menuState.main;
                recordBox.SetActive(false);
                modeBox.SetActive(false);
                modeIndex = 0;
            }
        }
        #endregion
    }

    public Vector2 offset,offsetB;

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

    void modeScroll(int dir)
    {
        modeIndex += dir;

        if (modeIndex == modeOptions.Length)
            modeIndex = 0;
        if (modeIndex < 0)
            modeIndex = modeOptions.Length - 1;

        float x = offsetB.x + modeOptions[modeIndex].rectTransform.sizeDelta.x / 2;
        float y = offsetB.y + modeOptions[modeIndex].rectTransform.anchoredPosition.y;
        
        modeIndicators[0].rectTransform.anchoredPosition = new Vector2(-x, y);
        modeIndicators[1].rectTransform.anchoredPosition = new Vector2(x, y);

        if (!PlayerPrefs.HasKey(modeIndex + "_t"))
            TimePB.text = "-";
        else
        {
            float bestTime = PlayerPrefs.GetFloat(modeIndex + "_t");

            //Time to do some formatting.
            string seconds = "" + (int)bestTime % 60;
            if (seconds.Length < 2)
                seconds = "0" + seconds;
            
            TimePB.text = ((int)bestTime / 60) % 60 + ":" + seconds;
        }

        if (!PlayerPrefs.HasKey(modeIndex + "_s"))
            speedPB.text = "-";
        else
            speedPB.text = ""+(PlayerPrefs.GetInt(modeIndex + "_s")*15);

    }

    //Convention for records

    //short     0
    //medium    1
    //long      2

    //Time      t
    //Speed     s
}
