using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Worldscroll : MonoBehaviour
{
    public static Worldscroll instance;
    [SerializeField]
    List<SpriteRenderer> floorTiles = new List<SpriteRenderer>();
    [SerializeField]
    ParticleSystem[] bushes;

    public GameObject FinishLine,FinishBox,pbBox,miniMap;
    public Text pbText;
    public Text[] EndGameTexts;
    public Image[] menuPointers;
    public float speedMultiplier, spawnMultiplier, levelDistance;
    int mode;
    // Use this for initialization
    void Start()
    {
        mode = PlayerPrefs.GetInt("mode");
        switch (mode)
        {
            case 0:
                levelDistance = 250;
                break;
            case 1:
                levelDistance = 750;
                break;
            case 2:
                levelDistance = 2000;
                break;
            case 3:
                levelDistance = 0;
                miniMap.SetActive(false);
                break;
        }
        instance = this;
    }

    public AudioClip boopSound;

    //When the player has reached the end
    IEnumerator Finish()
    {
        frameHolder.instance.StartCoroutine(frameHolder.instance.fadeMusic(false));
        GameStateManager.instance.ChangeState(GameStateManager.GameState.finishLine);

        if (FinishLine.transform.localPosition.x < -88)
            FinishLine.transform.localPosition = new Vector3(0.5f, .5f, 0);

        foreach (ParticleSystem ps in bushes)
        {
            ps.Pause();
        }
                
        yield return new WaitForSeconds(3);
        SoundManager.instance.playSound(boopSound, 1, 1.8f);
        FinishBox.SetActive(true);

        switch (PlayerPrefs.GetInt("mode"))
        {
            case 0:
                EndGameTexts[0].text = "2.5km        " + '\n' + "   ";
                break;
            case 1:
                EndGameTexts[0].text = "7.5km        " + '\n' + "   ";
                break;
            case 2:
                EndGameTexts[0].text = "20km         " + '\n' + "   ";
                break;
        }

        yield return new WaitForSeconds(1);
        SoundManager.instance.playSound(boopSound,1, 2.1f);
        EndGameTexts[0].text += HuDManager.instance.TimerText.text;

        if (HuDManager.instance.gameTimer < PlayerPrefs.GetFloat(mode+"_t") || !PlayerPrefs.HasKey(mode + "_t"))
        {
            yield return new WaitForSeconds(.66f);
            SoundManager.instance.playSound(pb);
            PlayerPrefs.SetFloat(mode + "_t", HuDManager.instance.gameTimer);
            pbBox.SetActive(true);
            pbText.text = "New best time!";
            while (!Input.GetButton("A"))
                yield return null;
            pbBox.SetActive(false);
        }

        yield return new WaitForSeconds(.5f);
        EndGameTexts[1].enabled = true;

        //Max speed
        yield return new WaitForSeconds(1);
        SoundManager.instance.playSound(boopSound,1,2.5f);
        EndGameTexts[2].text = ""+(Player.instance.topSpeed*15);

        if (Player.instance.topSpeed > PlayerPrefs.GetInt(mode+"_s") || !PlayerPrefs.HasKey(mode + "_s"))
        {
            yield return new WaitForSeconds(.66f);
            SoundManager.instance.playSound(pb);
            PlayerPrefs.SetInt(mode+"_s", Player.instance.topSpeed);
            pbBox.SetActive(true);
            pbText.text = "New best speed!";
            while (!Input.GetButton("A"))
                yield return null;
            pbBox.SetActive(false);
        }


        yield return new WaitForSeconds(1);
        //Display menu options
        EndGameTexts[3].enabled = true;
        EndGameTexts[4].enabled = true;
        menuPointers[0].enabled = true;
        menuPointers[1].enabled = true;
    }

    public AudioClip pb;

    void Update()
    {
        if (levelDistance > 0)
        {
            if (Player.instance.distanceCovered >= levelDistance && FinishLine.transform.position.x > 0)
            {
                FinishLine.transform.localPosition -= new Vector3(Mathf.FloorToInt(Player.instance.GetSpeed()), 0, 0);
                if (FinishLine.transform.localPosition.x < 0)
                    StartCoroutine(Finish());//time to end
            }
            HuDManager.instance.UpdatePlayerCursor(Player.instance.distanceCovered / levelDistance);
        }

        float speed = Player.instance.GetSpeed();


        if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
        {
            //Scroll floor
            for (int i = 0; i < floorTiles.Count; i++)
            {
                floorTiles[i].transform.position += Vector3.left * speed * Time.deltaTime*speedMultiplier ;
                if (floorTiles[i].transform.position.x < -96)
                {
                    SpriteRenderer sr = floorTiles[i];

                    floorTiles.Remove(sr);

                    sr.transform.position = new Vector3(floorTiles[floorTiles.Count - 1].transform.position.x + 32, Random.Range(-74, -74), 0);
                    floorTiles.Add(sr);
                }
            }
        }
        else if (GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
        {
            SoundManager.instance.managedAudioSources[0].AudioSrc.volume -= Time.deltaTime * (Player.instance.GetSpeed() / 5);

            if (EndGameTexts[4].enabled)
            {
                moveCD =  moveCD > 0 ? moveCD - Time.deltaTime : 0;
                if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 && moveCD <=0 )
                {
                    SoundManager.instance.playSound(boopSound,1,endIndex == 0 ? .8f : 1.2f);
                    moveCD = 0.2f;
                    endIndex += Input.GetAxis("Vertical") > 0 ? 1 : -1;
                    if (endIndex > 1)
                        endIndex = 0;
                    if (endIndex < 0)
                        endIndex = 1;

                    menuPointers[0].rectTransform.anchoredPosition = new Vector2(endIndex == 0 ? -18.5f : -23.5f, endIndex == 0 ? 10.5f : 22.5f);
                    menuPointers[1].rectTransform.anchoredPosition = new Vector2(endIndex == 0 ? 16.5f : 21.5f, endIndex == 0 ? 10.5f : 22.5f);
                }
                if (Input.GetButtonDown("A"))
                {
                    Application.LoadLevel(endIndex);
                }
            }
        }


        //Scroll bushes
        foreach (ParticleSystem ps in bushes)
        {
            ps.transform.position = new Vector3(Mathf.Lerp(128, 256, speed / 5), 6, 0);
            ps.emissionRate = Player.instance.GetSpeed() * spawnMultiplier;
            ps.startSpeed = Player.instance.GetSpeed() * speedMultiplier;

            ParticleSystem.Particle[] allParticles = new ParticleSystem.Particle[ps.particleCount];

            ps.GetParticles(allParticles);
            for (int i = 0; i < allParticles.Length; i++)
            {
                if (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
                    allParticles[i].velocity = new Vector2(0, 0);
                else
                    allParticles[i].velocity = new Vector2(-Player.instance.GetSpeed() * speedMultiplier, 0);
            }
            ps.SetParticles(allParticles, ps.particleCount);
        }
    }
    float moveCD;
    int endIndex=1;
}
