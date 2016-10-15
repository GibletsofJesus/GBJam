using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircleTransition : MonoBehaviour
{

    [SerializeField]
    Material mat;
    [SerializeField]
    AudioClip ZoomA, ZoomB;
    public Vector3 offset;

    // Use this for initialization
    void Start()
    {
        mat.SetFloat("_SliceAmount", 0);
        if (Player.instance)
            StartCoroutine(TransOut(Player.instance.transform.position));
        else
            StartCoroutine(TransOut(new Vector3(-51, -44, 0)));
    }

    void OnApplicationQuit()
    {
        mat.SetFloat("_SliceAmount", 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TransIn(Vector3 position)
    {
        float lerpy = 1;
        SoundManager.instance.playSound(ZoomA);
        while (lerpy > 0)
        {
            while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
                yield return null;

            transform.position = Camera.main.WorldToScreenPoint(position) + offset;
            lerpy -= Time.deltaTime;// *(lerpy+0.25f);
            //Do things
            mat.SetFloat("_SliceAmount", lerpy);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator TransOut(Vector3 position)
    {
        yield return new WaitForSeconds(1);
        float lerpy = 0;
        SoundManager.instance.playSound(ZoomB);

        while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
            yield return null;
        while (lerpy < 1)
        {
            while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
            {
                yield return null;
            }
            transform.position = Camera.main.WorldToScreenPoint(position) + offset;
            lerpy += Time.deltaTime * (lerpy + 0.5f);
            //Do things
            mat.SetFloat("_SliceAmount", lerpy);
            yield return new WaitForEndOfFrame();
            
            while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
                yield return null;
        }
    }
}
