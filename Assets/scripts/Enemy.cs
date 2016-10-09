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
    float killPointA,killPointB,indicateDistance;
    [SerializeField]
    float decellerationAmount, accellerationAmount;
    Collider2D thisCollider;
    bool willIndicate;

    void Awake()
    {
        thisCollider = GetComponent<Collider2D>();
        killPointA = Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0, 0)).x;
        killPointB = Camera.main.ViewportToWorldPoint(new Vector3(1.25f, 0, 0)).x;
        indicateDistance = Camera.main.ViewportToScreenPoint(new Vector3(2, 0, 0)).x;
    }

    void Update()
    {
        if (GameStateManager.instance.currentState != GameStateManager.GameState.Paused)
        {
            Movement();

            if (transform.position.x < killPointA)
                ReturnPool();
        }
        if (transform.position.x > killPointB && !moveLeft)
        {
            ReturnPool();
        }
        if (willIndicate)
        {
            if (transform.position.x < indicateDistance && transform.position.x > killPointB)
            {
                EnemyHudIndicators.instance.SetIndicator(transform.position,this);
            }
            if (transform.position.x < killPointB)
            {
                EnemyHudIndicators.instance.ResetIndicator(this);
            }
        }
    }

    float fallTimer = 0;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && fallTimer == 0)
        {
            float angle = Vector3.Angle(col.transform.position * (transform.position.y > col.transform.position.y ? -1 : 1), transform.position);
            //If snooker balls upgrade            
            StartCoroutine(fallToGround(angle, Player.instance.moveSpeed *
                ((PlayerPrefs.GetInt("Collision_Force") > 0 ? 1 : 0) + (PlayerPrefs.GetInt("Collision_Force")*0.25f))));
            //mb make the player flash
            frameHolder.instance.holdFrame(0.1f);
            Player.instance.StartCoroutine(Player.instance.TakeDamage(impactDamage, decellerationAmount));
            //StartCoroutine(TakeDamage(impactDamage));
        }
        if (col.tag == "enemy" && fallTimer > 0)
        {
            if (col.GetComponent<Enemy>().fallTimer == 0)
            {
                ExplosionManager.instance.PoolExplosion(transform.position, Vector3.one * 0.25f);
                
                frameHolder.instance.holdFrame(0.1f);
                Player.instance.IncreaseSpeed(accellerationAmount * .5f);
                col.GetComponent<Enemy>().StartCoroutine(col.GetComponent<Actor>().TakeDamage(50));
            }
        }
    }

    IEnumerator fallToGround(float angle, float impactForce)
    {
        moveLeft = false;
        if (angle > 90)
            angle -= 180;
        SoundManager.instance.playSound(deathSound[Random.Range(0, deathSound.Length)], 1, Random.Range(0.9f, 1.1f));
        float distance=0;

        float amp = 5;

        float y = Mathf.Sin(Mathf.Deg2Rad * angle) * impactForce * amp;
        float decay = 0.25f;
        float gravity = -50;
        float x = Mathf.Cos(Mathf.Deg2Rad * angle) * impactForce * amp;
        if (x < 0)
            x *= -1;

        while (transform.position.y > -54)//Floor height value from player.cs
        {
            while (GameStateManager.instance.currentState == GameStateManager.GameState.Paused)
                yield return null;

            //Debug.Log("Rotate by " + (impactForce * Time.deltaTime * (y > 0 ? 1 : -1)));
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

    public void OnPooled(Vector3 startPos,bool indicated)
    {
        //set everything up
        StopAllCoroutines();
        willIndicate = indicated;
        moveLeft = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        total++;
        transform.position = startPos;

        base.Start();
        int i = 0;
        foreach (SpriteRenderer s in sr)
        {
            s.color =standardCol[i];
            i++;
        }

    }

    public IEnumerator TakeDamage(float damage,float scale)
    {
        if ((HP - damage) <= 0)
        {
            Player.instance.IncreaseSpeed(scale + accellerationAmount);
        }
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
        EnemyHudIndicators.instance.ResetIndicator(this);
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
