using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worldscroll : MonoBehaviour
{
    public static Worldscroll instance;
    [SerializeField]
    List<SpriteRenderer> floorTiles = new List<SpriteRenderer>();
    [SerializeField]
    ParticleSystem[] bushes;

    public GameObject FinishLine;
    public float speedMultiplier, spawnMultiplier, levelDistance;


    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    //When the player has reached the end of 
    void Finish()
    {
        GameStateManager.instance.ChangeState(GameStateManager.GameState.finishLine);

        if (FinishLine.transform.localPosition.x < -88)
            FinishLine.transform.localPosition = new Vector3(0.5f, .5f, 0);

        foreach (ParticleSystem ps in bushes)
        {
            ps.Pause();
        }
    }

    void Update()
    {
        if (Player.instance.distanceCovered >= levelDistance && FinishLine.transform.position.x > 0)
        {
            FinishLine.transform.localPosition -= new Vector3(Mathf.FloorToInt(Player.instance.moveSpeed * speedMultiplier * 2)/2, .5f, 0);
            if (FinishLine.transform.localPosition.x < 0)
                Finish();//time to end
        }
        
        float speed = Player.instance.moveSpeed;

        HuDManager.instance.UpdatePlayerCursor(Player.instance.distanceCovered / levelDistance);

        if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
        {
            //Scroll floor
            for (int i = 0; i < floorTiles.Count; i++)
            {
                floorTiles[i].transform.position += Vector3.left * speed;
                if (floorTiles[i].transform.position.x < -96)
                {
                    SpriteRenderer sr = floorTiles[i];

                    floorTiles.Remove(sr);

                    sr.transform.position = new Vector3(floorTiles[floorTiles.Count - 1].transform.position.x + 32, Random.Range(-74, -74), 0);
                    floorTiles.Add(sr);
                }
            }
            //Scroll bushes
            foreach (ParticleSystem ps in bushes)
            {
                ps.transform.position = new Vector3(Mathf.Lerp(128, 256, speed / 5), 6, 0);
                ps.emissionRate = Player.instance.moveSpeed * spawnMultiplier;
                ps.startSpeed = Player.instance.moveSpeed * speedMultiplier;
            }
        }
        else if (GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
        {
            SoundManager.instance.managedAudioSources[0].AudioSrc.volume -= Time.deltaTime * (Player.instance.moveSpeed/5);
        }
    }
}
