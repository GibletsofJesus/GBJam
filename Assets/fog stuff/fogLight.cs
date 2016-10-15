using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class fogLight : Actor {

    public bool snap;
    public Foggy.lightSize size;
    [Range(0,1)]
    public float brightness;
    
    public override void LateUpdate()
    {
        if (snap)
            base.LateUpdate();
    }
}
