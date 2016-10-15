using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foggy : MonoBehaviour
{
    public Texture2D[] CircleSprites;
    [SerializeField]
    Renderer uncoverMe;
    //[SerializeField]
    //Collider2D colliderComponent;
    //Bounds boundingBox;
    Texture2D circleTex;
    [SerializeField]
    Texture2D DefaultTex;
    [SerializeField]
    fogLight[] followMe;
    [SerializeField]
    fogLight[] lights;
    Texture2D updatedTex;
    bool flicker;

    //After a light has passed over an area, how bright should the area be?
    public float brightnessResetValue;

    [SerializeField]
    float interval;
    public enum lightSize
    {
        small,
        medium,
        large,
    }

    void Start()
    {
        StartCoroutine(flickering());
        updatedTex = DefaultTex;
        //boundingBox = colliderComponent.bounds;
    }


    //Why not just make a fucking circle texture in the first place?
    void MakeCircleTexture(Sprite s)
    {
        circleTex = new Texture2D(s.texture.width, s.texture.height);
        circleTex.SetPixels(s.texture.GetPixels());
        circleTex.filterMode = FilterMode.Point;
        circleTex.Apply();
    }

    Texture2D RemovePixelsFromTexture(Texture2D startTex, Vector2 Pos,fogLight fLight)
    {
        Texture2D newTex = new Texture2D(startTex.width, startTex.height);
        newTex.SetPixels(startTex.GetPixels());
        newTex.filterMode = FilterMode.Point;

        circleTex = CircleSprites[(int)fLight.size * 2 + (flicker ? 1 : 0)];

        Vector2 offset = transform.position;
        for (int y = 0; y < circleTex.height; y++)
        {
            for (int x = 0; x < circleTex.width; x++)
            {
                Color newPixel;
                if (circleTex.GetPixel(x, y).a > 0)
                {
                    newPixel = new Color(0, 0, 0,1-fLight.brightness);
                    newTex.SetPixel(
                        (int)(Pos.x * newTex.width) + x - (int)offset.x - (circleTex.width/2),
                        (int)(Pos.y * newTex.height) + y - (int)offset.y - (circleTex.height/2),
                        newPixel);
                }
            }
        }
        newTex.Apply();
        return newTex;
    }

    Texture2D ResetTex(Texture2D newTex)
    {
        for (int y = 0; y < newTex.height; y++)
        {
            for (int x = 0; x < newTex.width; x++)
            {
                if (newTex.GetPixel(x, y).a < 1)
                    newTex.SetPixel(x, y, new Color(0, 0, 0, brightnessResetValue));
            }
        }
        newTex.Apply();
        return newTex;
    }
    
    void Update()
    {
        UpdateFog();
    }

    //Used to toggle sprites
    IEnumerator flickering()
    {
        while (CircleSprites[0] != CircleSprites[1])
        {
            //circleTex = b ? CircleSprites[0] : CircleSprites[1];
            yield return new WaitForSeconds(interval);
            flicker = !flicker;
        }
    }
    
    public void UpdateFog()
    {
        updatedTex =  ResetTex(updatedTex);
        foreach (fogLight l in lights)
        {
            updatedTex = RemovePixelsFromTexture(updatedTex, Camera.main.WorldToViewportPoint(l.transform.position), l);
        }

        //make sure to draw the player light last since it should be over the top of everything
        updatedTex = RemovePixelsFromTexture(updatedTex,
            Camera.main.WorldToViewportPoint(followMe[1].transform.position), followMe[1]);
        updatedTex = RemovePixelsFromTexture(updatedTex,
            Camera.main.WorldToViewportPoint(followMe[0].transform.position), followMe[0]);


        uncoverMe.material.mainTexture = updatedTex;
    }
}


