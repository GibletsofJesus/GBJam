using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionManager : MonoBehaviour {

    public static ExplosionManager instance;
    public List<explosions> allExplosions = new List<explosions>();

    private ObjectPool<explosions> objectPool = null;
    [SerializeField]
    private explosions ExplosionPrefab;

    // Use this for initialization
    void Start()
    {
        instance = this;
        objectPool = new ObjectPool<explosions>(ExplosionPrefab, 15, transform);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public explosions PoolExplosion(Vector3 position,Vector3 scale)
    {
        explosions newSplosion = objectPool.GetPooledObject(transform);
        newSplosion.OnPooled(position,scale);
        allExplosions.Add(newSplosion);
        return newSplosion;
    }

}
