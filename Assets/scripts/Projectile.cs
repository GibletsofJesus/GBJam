using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    public PoolData<Projectile> poolData { get; set; }
    public float maxLifetime;
    float currentLifeTime,killPoint,nerfPoint;

    float maxCollisions, collisions;

    [System.Serializable]
    public class ProjData
    {
        public float speed = 5;
        public float damage = 50;
    }
    public ProjData ProjectileData;

    void Awake()
    {
        //maxCollisions = PlayerPrefs.GetInt("Bullet_Penetration");
        maxCollisions = 0;
        killPoint = Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 0, 0)).x;
        nerfPoint = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
    }

    void Update()
    {
        transform.Translate(Vector3.right * ProjectileData.speed*Time.deltaTime);
        if (transform.position.x > killPoint)
            ReturnPool();
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime > maxLifetime)
            ReturnPool();
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (transform.position.x < nerfPoint)
        {
            if (col.tag != "Player" && col.tag != "bullet")
            {
                if (col.GetComponent<Enemy>())
                {
                    col.GetComponent<Enemy>().StartCoroutine(col.GetComponent<Enemy>().TakeDamage(ProjectileData.damage,transform.localScale.x));
                }
                collisions++;
                if (collisions >= maxCollisions)
                {
                    ReturnPool();
                }
            }
        }
    }

    public void OnPooled(ProjData data,Vector3 startPos)
    {
        //set everything up
        collisions = 0;
        transform.position = startPos;
        ProjectileData = data;
        gameObject.SetActive(true);
    }

    public void ReturnPool()
    {
        currentLifeTime = 0;
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
