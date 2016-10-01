using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircleTransition : MonoBehaviour {
    
    [SerializeField]
    Material mat;
    [SerializeField]
    AudioClip ZoomA, ZoomB;
    public Vector3 offset;

	// Use this for initialization
	void Start ()
    {
        mat.SetFloat("_SliceAmount", 0);
        StartCoroutine(TransOut());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Transition(Vector2 start, Vector2 end)
    {
        StartCoroutine(TransIn());
        yield return new WaitForSeconds(1);
        StartCoroutine(TransOut());
    }
    IEnumerator TransIn()
    {
        float lerpy = 1;
        SoundManager.instance.playSound(ZoomA);
        while (lerpy > 0)
        {
            transform.position = Camera.main.WorldToScreenPoint(Player.instance.transform.position) + offset;
            lerpy -= Time.deltaTime;// *(lerpy+0.25f);
            //Do things
            mat.SetFloat("_SliceAmount", lerpy);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator TransOut()
    {
        yield return new WaitForSeconds(1);
        float lerpy = 0;
        SoundManager.instance.playSound(ZoomB);
        while (lerpy < 1)
        {
            transform.position = Camera.main.WorldToScreenPoint(Player.instance.transform.position) + offset;
            lerpy += Time.deltaTime * (lerpy + 0.5f);
            //Do things
            mat.SetFloat("_SliceAmount", lerpy);
            yield return new WaitForEndOfFrame();
        }
    }
}
