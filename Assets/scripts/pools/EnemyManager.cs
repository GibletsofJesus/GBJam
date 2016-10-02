using UnityEngine;
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

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
        {
            spawnCooldown = EnemyPrefab.width / ((Player.instance.moveSpeed * 50) + EnemyPrefab.moveSpeed);
            if (spawnTimer <= 0)
            {
                //Do this based of cooldown
                PoolEnemy(transform.position + new Vector3(0, 1 + (20 * Mathf.FloorToInt(Random.Range(1, 7))), 0));

                //ok so size of enemy
                //divided by thier movespeed
                //Gives the amount of time it takes for it to travel away
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
