using UnityEngine;
using System.Collections;

public class Player : Actor
{
    public static Player instance;

    public float fireRate, floorHeight, maxJumpTime;
    [HideInInspector]
    public float distanceCovered;
    [Header("Idle Acceleration")]
    public AnimationCurve IdleAccelerationCurve;
    public float IdleAccLimit, decellerationPerShot;

    [Header("")]
    [SerializeField]
    Sprite[] charSprites;

    [Header("Shooting tings")]
    [SerializeField]
    Transform[] shootTransforms;
    [SerializeField]
    Projectile.ProjData bulletStats;
    [SerializeField]
    ParticleSystem[] bulletCasings;
    [SerializeField]
    GameObject heightMarker;
    [SerializeField]

    ParticleSystem muzzleFlash, moveSparks;
    [SerializeField]
    AudioClip shootSound, accelerationSound;
    float shootCooldown, abilityCooldown, jumpTimer;
    float speedToAdd = 0.001f;
    public bool SlowMotion;

    public override void Start()
    {
        base.Start();
        instance = this;
    }

    void Update()
    {
        heightMarker.transform.position = new Vector3(0, -44+(verticalSpeed*maxJumpTime) - (gravity*maxJumpTime*maxJumpTime), 0);

        if (GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
        {
            if (transform.position.x < 128)
                Move(GetSpeed() * Time.deltaTime * Worldscroll.instance.speedMultiplier, 0);
        }
        else if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
        {
            distanceCovered += moveSpeed * Time.deltaTime;
            HuDManager.instance.speedText.text = "SPEED  " + (moveSpeed * 50).ToString("F2");
            moveSparks.emissionRate = moveSpeed * 3;

            Movement();
            Shooting();
            shootCooldown = shootCooldown > 0 ? 0 : shootCooldown - Time.deltaTime;
        }

#if UNITY_ANDROID
            TouchJumping();
#else
        KeyboardInput();
#endif
        #region gravity
        if (transform.position.y > floorHeight)
        {
            if (!jumping)
            {
                SoundManager.instance.managedAudioSources[1].AudioSrc.volume -= Time.deltaTime;
                SoundManager.instance.managedAudioSources[1].AudioSrc.pitch -= Time.deltaTime * 3;
                Move(0, -Time.deltaTime * gravity * fallTimer);
                fallTimer += Time.deltaTime * 3;
            }
            else
            {
                SoundManager.instance.managedAudioSources[1].AudioSrc.volume += Time.deltaTime;
                SoundManager.instance.managedAudioSources[1].AudioSrc.pitch += Time.deltaTime;
            }
        }

        //Stop jumping
        if (!pressingJump && wasPressingJump && transform.position.y > floorHeight || GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
            jumping = false;

        if (transform.position.y == floorHeight)
        {
            SoundManager.instance.managedAudioSources[1].AudioSrc.volume = 0f;
            SoundManager.instance.managedAudioSources[1].AudioSrc.volume = 0;
        }
        #endregion
    }

    public void TouchJumping()
    {
        wasPressingJump = pressingJump;
        wasPressingShoot = pressingShoot;

        pressingJump = false;
        pressingShoot = false;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).position.x > (Screen.width / 2))
                    pressingJump = true;
                else
                    pressingShoot = true;
            }
        }
    }

    bool pressingJump, wasPressingJump;
    bool pressingShoot, wasPressingShoot;

    void KeyboardInput()
    {
        if (Input.GetButtonDown("Fire2"))
            SlowMotion = true;

        if (Input.GetButtonUp("Fire2"))
            SlowMotion = false;

        if (Input.GetKey(KeyCode.S))
            Time.timeScale = 0.1f;
        else
            Time.timeScale = 1;

            wasPressingJump = pressingJump;
        wasPressingShoot = pressingShoot;
        //I want to make it clear I know there's options for GetButtonUp/Down etc.
        //I'm setting this up so mobile input works ok
        pressingJump = Input.GetButton("Jump");
        pressingShoot = Input.GetButton("Fire1");
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public float GetSpeed()
    {
        return moveSpeed * (SlowMotion ? 0.1f : 1f);
    }

    void Shooting()
    {
        if (!wasPressingShoot && pressingShoot && shootCooldown <= 0)
        {
            SoundManager.instance.playSound(shootSound, 1, Random.Range(0.85f, 1.15f));
            shootCooldown = fireRate;
            Projectile proj = ProjectilePooler.instance.PoolProj(bulletStats, transform.position,moveSpeed*0.2f);
           // moveSpeed -= decellerationPerShot;
            moveSpeed *= 0.8f;
            if (moveSpeed < 0)
                moveSpeed = 0.1f;

            if (CameraShake.instance)
            {
                if (CameraShake.instance.shakeDuration < 0.125f)
                    CameraShake.instance.shakeDuration = 0.125f;
                CameraShake.instance.shakeAmount = 0.5f;
            }
        }
    }

    public void IncreaseSpeed(float speed)
    {
        speedToAdd += speed;
    }
    
    bool jumping = false;

    public IEnumerator TakeDamage(float damage, float Decelleration)
    {
        moveSpeed -= Decelleration;
        if (moveSpeed < 0.1f)
            moveSpeed = 0.1f;
        return base.TakeDamage(damage);
    }

    public override void Movement()
    {
        base.Movement();
        
        //Make sure we don't fall through the floor
        if (transform.position.y < floorHeight)
            transform.position = new Vector3(transform.position.x, floorHeight, 0);

#region On ground
        if (transform.position.y == floorHeight)
        {
            SoundManager.instance.ChangeMoveSound(true, GetSpeed() / 100f);

#region just landed
            if (speedToAdd > 0)
            {
                if (speedToAdd > 0.1f)
                {
                    SoundManager.instance.playSound(accelerationSound, 1, 1);
                    if (CameraShake.instance)
                    {
                        if (CameraShake.instance.shakeDuration < 0.25f)
                            CameraShake.instance.shakeDuration = 0.25f;
                        CameraShake.instance.shakeAmount = 0.8f;

                    }
                }
                moveSparks.enableEmission = true;
                moveSpeed += speedToAdd;
                speedToAdd = 0;
                SoundManager.instance.managedAudioSources[1].AudioSrc.pitch = 0.5f;
            }
#endregion
            moveSpeed += IdleAccelerationCurve.Evaluate(moveSpeed / IdleAccLimit)*Time.deltaTime;
            //moveSpeed += Time.deltaTime * 0.35f;
            jumping = false;
            jumpTimer = 0;
            fallTimer = 0;
            maxJumpTime = moveSpeed * 0.1f;
        }
#endregion

        //Start jump
        if (!jumping && pressingJump && transform.position.y == floorHeight)
        {
            SoundManager.instance.ChangeMoveSound(false, GetSpeed() / 100f);
            jumping = true;
        }

#region jumping upwards
        if (jumping && jumpTimer < maxJumpTime)
        {
            if (speedToAdd == 0)
            {
                moveSparks.enableEmission = false;
                speedToAdd = 0.01f;
            }
            Move(0, (SlowMotion ? 0.1f : 1) * verticalSpeed * Time.deltaTime - (gravity * jumpTimer * Time.deltaTime));
            jumpTimer += Time.deltaTime;
            if (jumpTimer > maxJumpTime)
                jumping = false;
        }
        #endregion

    }
    public float gravity, fallTimer;
}
