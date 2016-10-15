using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuItem : MonoBehaviour {

    public Text Title, TitleOutline, Description;
    public SpriteRenderer[] pointImgs;
    public RectTransform rect;
    public int points = 0;
    public Animator[] shimmerAnimators;
    // Use this for initialization
    void Start()
    {
        if (!rect)
            rect = GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public bool ManagePoints(int newPoints)
    {
        points = newPoints;
        if (points > 3)
        {
            points = 3;
            return false;
        }
        else if (points < 0)
        {
            points = 0;
            return false;
        }

        for (int i = 0; i < pointImgs.Length; i++)
        {
            if (i < points)
            {
                pointImgs[i].enabled = true;
                shimmerAnimators[i].enabled = true;
                shimmerAnimators[i].GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                pointImgs[i].enabled = false;
                shimmerAnimators[i].enabled = false;
                shimmerAnimators[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        //PlayerPrefs.SetInt(Title.text.Replace(' ', '_'), points);
        return true;
    }
}
