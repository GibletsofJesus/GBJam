﻿using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

    public List<Enemy> AllEnemies = new List<Enemy>();

    private ObjectPool<Enemy> objectPool = null;
    [SerializeField]
    private Enemy EnemyPrefab = null;
    float spawnCooldown,spawnTimer;

    void Start()
    {
        objectPool = new ObjectPool<Enemy>(EnemyPrefab, 15, transform);
    }

    //Idea for spawning
    //Have lower level enemies (literally and figuratively) spawn in more regular, telegraphed patterns such as lines.

    //         |                                   00000
    //         |
    //      >> |           OOOOO
    //       > |                        OOOOO
    //  .      |
    //-----------------------------------------------------

    int waveSize=4; //Number of enemies in each clump
    float waveFrequency=0.25f; //How many 'blocks' should go unfilled after the wave
    void Update()
    {
        if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
        {
            //ok so size of enemy
            //divided by thier movespeed
            //Gives the amount of time it takes for it to travel away
            spawnCooldown = (EnemyPrefab.width * (waveSize* waveFrequency)) / ((Player.instance.GetSpeed()) + EnemyPrefab.moveSpeed);
            if (spawnTimer <= 0)
            {
                float yPos = 1 + (20 * Mathf.FloorToInt(Random.Range(1, 7)));
                for (int i = 0; i < waveSize; i++)
                {
                    PoolEnemy(transform.position + new Vector3(i * (EnemyPrefab.width + 2), yPos, 0));
                }
                spawnTimer = spawnCooldown;
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }
        }
    }

    public Enemy PoolEnemy(Vector3 startPos)
    {
        Enemy newEnemy = objectPool.GetPooledObject(transform);
        newEnemy.OnPooled(startPos);
        AllEnemies.Add(newEnemy);
        return newEnemy;
    }
}
