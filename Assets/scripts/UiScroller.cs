using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UiScroller : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer[] pointImages;

    [Header("Menu creation")]
    [SerializeField]
    GameObject SampleMenuElement;
    public int DesiredMenuSize;
    List<UpgradeMenuItem> UpgradeElements = new List<UpgradeMenuItem>();
    UpgradeManager.Upgrade[] options;
    [Header("Transition preferences")]
    public float ScrollSpeed, transitionSpeed;
    public Vector3 maxItemScale = Vector3.one;
    public Vector3 minItemScale = Vector3.one / 2;
    public Vector2 elementOffset = new Vector2(0, 150),offset;
    public AnimationCurve curve;
    [SerializeField]
    int menuIndex;
    [SerializeField]
    List<Animator> shimmers = new List<Animator>();

    [SerializeField]
    Renderer sceneTransition;

    IEnumerator SceneTransition(bool b)
    {
        float lerpy = 0;

        while (lerpy < 1)
        {
            lerpy += Time.deltaTime;
            sceneTransition.material.SetFloat("_SliceAmount", b ? lerpy: 1-lerpy);
            yield return new WaitForEndOfFrame();
        }
        if (!b)
            Application.LoadLevel(2);
    }

    void OnEnable()
    {
        StartCoroutine(SceneTransition(true));
        foreach(SpriteRenderer sr in pointImages)
        {
            shimmers.Add(sr.GetComponentInParent<Animator>());
        }
        menuIndex = (((DesiredMenuSize - 1) / 2)) % UpgradeManager.instance.upgrades.Length;

        for (int i = 0; i < DesiredMenuSize-1; i++)
        {
            GameObject newThing = Instantiate(SampleMenuElement) as GameObject;
            newThing.SetActive(true);
            newThing.transform.parent = transform;
            newThing.transform.localRotation = Quaternion.Euler(Vector3.zero);
            newThing.transform.localScale = (i == (DesiredMenuSize - 1) / 2) ? maxItemScale : minItemScale;
            UpgradeMenuItem item = newThing.GetComponent<UpgradeMenuItem>();

            item.rect.anchoredPosition3D = (i * -elementOffset) + (((DesiredMenuSize - 1) * elementOffset) / 2);

            UpgradeElements.Add(item);
            UpgradeManager.Upgrade up = UpgradeManager.instance.upgrades[i % UpgradeManager.instance.upgrades.Length];
            UpgradeElements[i].Title.text = up.name.Replace('_',' ');
            UpgradeElements[i].TitleOutline.text = up.name.Replace('_', ' ');
            UpgradeElements[i].Description.text = up.description;
            UpgradeElements[i].points = up.points;
            UpgradeElements[i].ManagePoints(up.points);
            UpgradeManager.instance.upgrades[i % UpgradeManager.instance.upgrades.Length].AssociatedItem = UpgradeElements[i];
        }

        ReDrawPoints();
    }

    float moveCD;

    public AudioClip[] MoveSounds;

    // Update is called once per frame
    void Update()
    {
        #region Scrolling left/right
        if (Input.GetAxis("Horizontal") > 0 && moveCD <= 0)
        {
            SoundManager.instance.playSound(MoveSounds[0],1,1.5f);
            moveCD = ScrollSpeed;
            //If index less than 0, loop back to the top.
            menuIndex = (menuIndex - 1) < 0 ? UpgradeElements.Count - 1 : menuIndex - 1;
            MenuScroll(-1);
        }
        if (Input.GetAxis("Horizontal") < 0 && moveCD <= 0)
        {
            SoundManager.instance.playSound(MoveSounds[0], 1, 1.5f);
            moveCD = ScrollSpeed;
            //If index larger than max, go to 0.
            menuIndex = (menuIndex + 1) > UpgradeElements.Count - 1 ? 0 : menuIndex + 1;
            MenuScroll(1);
        }
        #endregion

        #region Assign points
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 && moveCD <= 0)
        {
            int polarity = Input.GetAxis("Vertical") > 0 ? 1 : -1;
            ;            //Add points to upgrade if there's space.
            //And remove points from available points;

            //So
            //If available is less than 1 and polarity is positive, don't allow.
            //If available max and polarity is negative, don't allow.
            

            if (!(UpgradeManager.instance.upgradePoints < 1 && polarity > 0) &&
                !(UpgradeManager.instance.upgradePoints == UpgradeManager.instance.maxPoints && polarity < 0))
            {
                if (UpgradeManager.instance.upgrades[menuIndex].AssociatedItem.ManagePoints(UpgradeManager.instance.upgrades[menuIndex].points + polarity))
                {
                    SoundManager.instance.playSound(MoveSounds[0], 1, Random.Range(2.3f,2.7f));
                    UpgradeManager.instance.upgrades[menuIndex].points += polarity;
                    UpgradeManager.instance.upgradePoints -= polarity;
                    //Redo available points
                    ReDrawPoints();
                }
            }
            moveCD = ScrollSpeed;
        }
        #endregion

        if (Input.GetButtonDown("Start"))
        {
            UpgradeManager.instance.SaveToFile();

            //Mb some sort of transition effect
            StartCoroutine(SceneTransition(false));
        }

        moveCD = moveCD > 0 ? moveCD - Time.deltaTime : 0;
    } 

    void ReDrawPoints()
    {
        for (int i = 0; i < pointImages.Length; i++)
        {
            if (i < UpgradeManager.instance.upgradePoints)
            {
                pointImages[i].enabled = true;
                shimmers[i].enabled = true;
            }
            else
            {
                shimmers[i].Play("shimmer_idle");
                shimmers[i].enabled = false;
                pointImages[i].enabled = false;
            }
        }
    }

    void MenuScroll(int direction)
    {
        /*mid += direction;
        if (mid > TextElements.Count-1)
            mid = 0;
        if (mid < 0)
            mid = TextElements.Count - 1;*/

        #region swap bottom / top text elements for looping effect
        UpgradeMenuItem last = UpgradeElements[UpgradeElements.Count - 1];
        UpgradeMenuItem first = UpgradeElements[0];
        if (direction > 0)
        {
            //Items are going to be moving UP since selection is DOWN
            //i.e. the first item in the array goes to the last position
            UpgradeElements.Remove(UpgradeElements[0]);
            UpgradeElements.Add(first);
            UpgradeElements[UpgradeElements.Count - 1].rect.anchoredPosition = last.rect.anchoredPosition - (elementOffset+offset);
        }
        else
        {
            UpgradeElements.Remove(UpgradeElements[UpgradeElements.Count - 1]);
            UpgradeElements.Insert(0, last);
            UpgradeElements[0].rect.anchoredPosition = first.rect.anchoredPosition + (elementOffset+offset);
        }
        #endregion
        
        for (int i = 0; i < UpgradeElements.Count; i++)
        {
            StartCoroutine(moveItem(UpgradeElements[i], offset+ elementOffset * direction, transitionSpeed));       
        }
    }
    
    IEnumerator moveItem(UpgradeMenuItem _text, Vector2 movement, float _time)
    {
        Vector2 start = _text.rect.anchoredPosition;
        float currentTime = 0;
        yield return new WaitForEndOfFrame();
        while (currentTime < _time)
        {
            currentTime += Time.deltaTime;
            if (currentTime > _time)
                currentTime = _time;
            _text.rect.anchoredPosition = Vector2.Lerp(start, start + movement, curve.Evaluate(currentTime / _time));
            yield return new WaitForEndOfFrame();
        }
    }
}
