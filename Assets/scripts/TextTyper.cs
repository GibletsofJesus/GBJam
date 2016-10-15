using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour
{
    [SerializeField]
    Text TextComp;
    [SerializeField]
    Animator goofers;
    [SerializeField]
    AudioClip textBeep;
    public string[] messages;
    public float textSpeed;

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(DisplayMessages());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("A"))
        {
            goofers.SetFloat("speed", 2);
            fastText = true;
        }
        else
        {
            goofers.SetFloat("speed", 1);
            fastText = false;
        }
    }


    bool fastText;

    public void TutorialTime(TitleScreen ts)
    {
        goofers.Play("intro");
        StartCoroutine(DisplayMessages(ts));
    }

    IEnumerator DisplayMessages(TitleScreen ts)
    {
        //Make goofington appear

        yield return new WaitForSeconds(2.25f);

        for (int i = 0; i < messages.Length; i++)
        {
            TextComp.text = "";
            goofers.Play("face_flap");
            for (int j = 0; j < messages[i].Length; j++)
            {
                if (messages[i][j] == '\\')
                    TextComp.text += '\n';
                else
                    TextComp.text += messages[i][j];

                //SoundManager.instance.playSound(textBeep, SoundManager.instance.volumeMultiplayer, Random.Range(0.95f, 1.05f));

                SoundManager.instance.playSound(textBeep, SoundManager.instance.volumeMultiplayer, Random.Range(1.35f, 1.6f));

                float timer = 0;
                while (timer < textSpeed)
                {
                    timer += Time.deltaTime * (fastText ? 5 : 1);
                    yield return new WaitForEndOfFrame();
                }
            }

            goofers.Play("face_idle");
            while (!Input.GetButtonDown("A"))
                yield return null;
        }
        TextComp.text = "";
        goofers.Play("outro");
        yield return new WaitForSeconds(1.75f);
        goofers.gameObject.SetActive(false);
        ts.state = TitleScreen.menuState.main;
    }
}
