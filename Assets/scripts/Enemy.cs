using UnityEngine;
using System.Collections;

public class Enemy : Actor, IPoolable<Enemy>
{
    public static int total;
    public PoolData<Enemy> poolData { get; set; }
    public float width;
    [SerializeField]
    ParticleSystem trails;
    bool moveLeft=true;
    [SerializeField]
    AudioClip[] deathSound;
    float killPointA,killPointB;
    [SerializeField]
    float decellerationAmount, accellerationAmount;
    Collider2D thisCollider;

    void Awake()
    {
        thisCollider = GetComponent<Collider2D>();
        killPointA = Camera.main.ScreenToWorldPoint(new Vector3(-0.5f, 0, 0)).x;
        killPointB = Camera.main.ViewportToScreenPoint(new Vector3(1.1f, 0, 0)).x;
    }

    void Update()
    {
        if (GameStateManager.instance.currentState != GameStateManager.GameState.Paused)
        {
            Movement();

            if (transform.position.x < killPointA)
                ReturnPool();
        }
        if (transform.position.x > killPointB && fallTimer > 0)
        {
            Debug.Log(killPointB + "   " + transform.position.x);
            ReturnPool();
        }
    }

    float fallTimer = 0,TravelSpeed;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && fallTimer == 0)
        {
            float angle = Vector3.Angle(col.transform.position * (transform.position.y > col.transform.position.y ? -1 : 1), transform.position);
            //If snooker balls upgrade            
            StartCoroutine(fallToGround(angle, Player.instance.moveSpeed));
            //mb make the player flash
            Player.instance.StartCoroutine(Player.instance.TakeDamage(impactDamage, decellerationAmount));
            //StartCoroutine(TakeDamage(impactDamage));
        }
        if (col.tag == "enemy" && fallTimer > 0)
        {
            ExplosionManager.instance.PoolExplosion(transform.position, Vector3.one * 0.25f);
            float angle = Vector3.Angle(col.transform.position * (transform.position.y > col.transform.position.y ? -1 : 1), transform.position);

            Player.instance.IncreaseSpeed(accellerationAmount*.5f);
            col.GetComponent<Enemy>().StartCoroutine(col.GetComponent<Enemy>().fallToGround(angle,TravelSpeed));
        }
    }

    IEnumerator fallToGround(float angle,float impactForce)
    {
        moveLeft = false;
        if (angle > 90)
            angle -= 180;
        frameHolder.instance.holdFrame(0.1f);
        SoundManager.instance.playSound(deathSound[Random.Range(0, deathSound.Length)], 1, Random.Range(0.9f, 1.1f));
        float distance=0;

        float amp = 5;

        float y = Mathf.Sin(Mathf.Deg2Rad * angle) * impactForce * amp;
        float decay = 0.1f;
        float gravity = -50;
        float x = Mathf.Cos(Mathf.Deg2Rad * angle) * impactForce * amp;
        if (x < 0)
            x *= -1;
        TravelSpeed = Vector2.SqrMagnitude(new Vector2(x, y));

        while (transform.position.y > -54)//Floor height value from player.cs
        {
            while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
                yield return null;

            transform.Rotate(new Vector3(0, 0, impactForce * Time.deltaTime * (y > 0 ? 1 : -1)));
            y -= decay;
            Move(x * Time.deltaTime, ((y - decay) + gravity) * Time.deltaTime);
                        
            Move((impactForce - Player.instance.moveSpeed) * Time.deltaTime * 50, 0);

            fallTimer += Time.deltaTime;
            distance = Vector2.Distance(Vector2.zero, new Vector2(200 * Time.deltaTime, -Time.deltaTime * 200 * fallTimer));
            yield return new WaitForEndOfFrame();
            if (distance > 1)
            {
                //Need to account for if an enemy travels at a speed higher than 1 unit per frame.
                trailPlacer.instance.PlaceTrailParticle(transform.position);
                distance -= 1;
            }
        }

        frameHolder.instance.holdFrame(0.1f);
        Death();
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

    public override IEnumerator TakeDamage(float damage)
    {
        if ((HP-damage) <=0)
            Player.instance.IncreaseSpeed(accellerationAmount);
        return base.TakeDamage(damage);
    }

    public override void Death()
    {
        total--;
        if (deathSound.Length>0)
            SoundManager.instance.playSound(deathSound[Random.Range(0, deathSound.Length)], 1, Random.Range(0.9f, 1.1f));
        frameHolder.instance.holdFrame(0.1f);
        ReturnPool();
        base.Death();
        HuDManager.instance.kills++;
        HuDManager.instance.killText.text = HuDManager.instance.kills+"";
        ExplosionManager.instance.PoolExplosion(transform.position, Vector3.one * 0.25f);
    }

    public void ReturnPool()
    {
        fallTimer = 0;
        StopAllCoroutines();
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }

    public override void Movement()
    {
        base.Movement();
        if (moveLeft)
        {
            if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
                Move(-Time.deltaTime * ((Player.instance.GetSpeed() * 50) + moveSpeed), 0);
            else if (GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
                Move(-Time.deltaTime * moveSpeed, 0);
        }

    }
}
