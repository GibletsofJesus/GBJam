using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

    public static CameraMover instance;
    Vector3 lowerLimit;
    public Vector3 standardPosition;

    void Start()
    {
        instance = this;
        lowerLimit = transform.position;
    }

    void Update()
    {
        //If player is on ground, stay still,
        //If player moves above the top 75% of the screen, move the camera up

        while (Player.instance.transform.position.y > Camera.main.ViewportToWorldPoint(new Vector3(0, .75f, 0)).y)
        {
            //Move camera up
            //Do it until the if statement is resolved
            transform.position += Vector3.up;
        }

        while (Player.instance.transform.position.y < Camera.main.ViewportToWorldPoint(new Vector3(0, 0.25f, 0)).y)
        {
            //move cam down
            transform.position += Vector3.down;

        }
        if (transform.position.y < 0)
            transform.position = lowerLimit;

        standardPosition = transform.position;

    }
}
