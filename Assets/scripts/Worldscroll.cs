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
            FinishLine.transform.localPosition -= new Vector3(Mathf.FloorToInt(Player.instance.GetSpeed()), 0, 0);
            if (FinishLine.transform.localPosition.x < 0)
                Finish();//time to end
        }
        
        float speed = Player.instance.GetSpeed();

        HuDManager.instance.UpdatePlayerCursor(Player.instance.distanceCovered / levelDistance);

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

}
