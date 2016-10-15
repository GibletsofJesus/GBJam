using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modeManager : MonoBehaviour {

    public static modeManager instance;
    public mode selectedMode;

    public enum mode
    {
        _small,
        _medium,
        _long,
        endless

    };
    //250
    //750
    //2500
    
    void Start()
    {
        instance = this;
    }
    
    void Update()
    {

    }
}
