using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public string description;
        public int maxPoints; //How many points can the player put into the skill
        public int points;
        public UpgradeMenuItem AssociatedItem;
        public Upgrade(string n, string desc, int p)
        {
            name = n;
            description = desc;
            points = p;
        }
    }

    public Upgrade[] upgrades;

    public int upgradePoints,maxPoints;

    //Leftover points put into base acceleration?
    [SerializeField]
    UiScroller scroller;

    void Nuke()
    {
        PlayerPrefs.DeleteAll();
    }

    void Start()
    {
        //Nuke();
        if (PlayerPrefs.HasKey("Available_Points"))
            upgradePoints = PlayerPrefs.GetInt("Available_Points");
        else
            PlayerPrefs.SetInt("Available_Points", 7);

        instance = this;
        foreach (Upgrade u in upgrades)
        {
            if (PlayerPrefs.HasKey(u.name))
            {
                u.points = PlayerPrefs.GetInt(u.name);
            }
        }
        scroller.enabled = true;
    }
    public void SaveToFile()
    {
        foreach (Upgrade u in upgrades)
        {
            PlayerPrefs.SetInt(u.name,u.points);
        }
        PlayerPrefs.SetInt("Available_Points", upgradePoints);
        PlayerPrefs.Save();
    }

    void Update()
    {

    }
}
