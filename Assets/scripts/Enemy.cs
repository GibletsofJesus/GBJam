using UnityEngine;
using System.Collections;

public class Enemy : Actor, IPoolable<Enemy>
{
    public static int total;
    public PoolData<Enemy> poolData { get; set; }
    public float width;
    
    [SerializeField]
    AudioClip[] deathSound;
    float killPoint;
    [SerializeField]
    float decellerationAmount,accellerationAmount;

    void Awake()
    {
        killPoint = Camera.main.ScreenToWorldPoint(new Vector3(-0.5f, 0, 0)).x;
    }

    void Update()
    {
        Movement();

        if (transform.position.x < killPoint)
            ReturnPool();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            frameHolder.instance.holdFrame(0.1f);
            //mb make the player flash
            Player.instance.StartCoroutine(Player.instance.TakeDamage(impactDamage,decellerationAmount));
            StartCoroutine(TakeDamage(impactDamage));
        }
    }

    public void OnPooled(Vector3 startPos)
    {
        total++;
        //set everything up
        transform.position = startPos;

        base.Start();
        int i = 0;
        foreach (SpriteRenderer s in sr)
        {
            s.color =standardCol[i];
            i++;
        }

        gameObject.SetActive(true);
    }

    public override void Death()
    {
        total--;
        if (deathSound.Length>0)
            SoundManager.instance.playSound(deathSound[Random.Range(0, deathSound.Length)], 1, Random.Range(0.9f, 1.1f));
        frameHolder.instance.holdFrame(0.1f);
        ReturnPool();
        Player.instance.IncreaseSpeed(accellerationAmount);
        base.Death();
        HuDManager.instance.kills++;
        HuDManager.instance.killText.text = HuDManager.instance.kills+"";
        ExplosionManager.instance.PoolExplosion(transform.position, Vector3.one * 0.25f);
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }

    public override void Movement()
    {
        base.Movement();
        if (GameStateManager.instance.currentState==GameStateManager.GameState.Gameplay)
        Move(-Time.deltaTime * ((Player.instance.moveSpeed *50)+ moveSpeed), 0);
        else if (GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
            Move(-Time.deltaTime *moveSpeed, 0);

    }
}
