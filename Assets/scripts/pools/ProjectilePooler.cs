﻿using UnityEngine;
using System.Collections.Generic;

public class ProjectilePooler : MonoBehaviour
{
    public static ProjectilePooler instance;
    
    public List<Projectile> allProjectiles = new List<Projectile>();

    private ObjectPool<Projectile> objectPool = null;
    [SerializeField]
    private Projectile projectilePrefab = null;

    private void Awake()
    {
        instance = this;
        objectPool = new ObjectPool<Projectile>(projectilePrefab, 150, transform);
    }
    public void Reset()
    {
        foreach (Projectile p in allProjectiles)
        {
            p.gameObject.SetActive(false);
            p.ReturnPool();
        }
    }

    public Projectile PoolProj(Projectile.ProjData data,Vector3 startPos, float scale)
    {
        Projectile newProj = objectPool.GetPooledObject(transform);
        newProj.OnPooled(data, startPos);
        newProj.transform.localScale = Vector3.one * scale;
        allProjectiles.Add(newProj);
        return newProj;
    }
}
