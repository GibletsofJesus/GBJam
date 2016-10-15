using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : Actor
{
    public static Player instance;

    float fireRate=10, floorHeight=-54, maxJumpTime;
    [HideInInspector]
    public float distanceCovered;
    [Header("Idle Acceleration")]
    public AnimationCurve IdleAccelerationCurve;
    public float IdleAccLimit, decellerationPerShot;

    [SerializeField]
    Text heightText;

    [SerializeField]
    ParticleSystem[] coolFire;
    [SerializeField]
    Projectile.ProjData bulletStats;
    [SerializeField]
    GameObject heightMarker;
    [SerializeField]
    AudioClip shootSound, accelerationSound;
    float shootCooldown, abilityCooldown, jumpTimer;
    float speedToAdd = 0.001f;
    public int topSpeed;

    public override void Start()
    {
        base.Start();
        instance = this;
    }

    void Update()
    {
        if (moveSpeed > topSpeed)
        {
            topSpeed = (int)moveSpeed;

            if (PlayerPrefs.GetInt("mode") == 3)//We must be in endless mode
            {
                //In which case, record highest speed;
                if (topSpeed > PlayerPrefs.GetInt("3_s"))
                {
                    PlayerPrefs.SetInt("3_s", topSpeed);
                }
            }
        }

        heightMarker.transform.position = new Vector3(0, -44 + (verticalSpeed * maxJumpTime), 0);

        if (GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
        {
            if (transform.position.x < 128)
                Move(GetSpeed() * Time.deltaTime * Worldscroll.instance.speedMultiplier, 0);
        }
        else if (GameStateManager.instance.currentState == GameStateManager.GameState.Gameplay)
        {
            distanceCovered += moveSpeed * Time.deltaTime;
            HuDManager.instance.speedText.text = (moveSpeed * 15).ToString("F0");
            HuDManager.instance.UpdateSpeedBar(moveSpeed);

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
        if (GameStateManager.instance.currentState != GameStateManager.GameState.Paused)
        {
            if (transform.position.y > floorHeight)
            {
                if (transform.position.y > 4)
                    heightText.text = "" + (transform.position.y + 54);
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
            else
                heightText.text = "";
        }
        //Stop jumping
        if (!pressingJump && wasPressingJump && transform.position.y > floorHeight || GameStateManager.instance.currentState == GameStateManager.GameState.finishLine)
            jumping = false;


        if (transform.position.y == floorHeight && transform.position.x < 100)
        {
            if (GameStateManager.instance.currentState != GameStateManager.GameState.Paused)
            {
                coolFire[0].Play();
                coolFire[1].Play();
                coolFire[0].emissionRate = (moveSpeed - 5) * 30;
                coolFire[1].emissionRate = (moveSpeed - 5) * 30;

                coolFire[0].startSpeed = moveSpeed * 50 * (transform.position.x > -60 ? 4 : 1);
                coolFire[1].startSpeed = moveSpeed * 50 * (transform.position.x > -60 ? 4 : 1);
            }
            else
            {
                coolFire[0].Pause();
                coolFire[1].Pause();
            }
            SoundManager.instance.managedAudioSources[1].AudioSrc.volume = 0f;
            SoundManager.instance.managedAudioSources[1].AudioSrc.volume = 0;
        }
        else
        {
            coolFire[0].emissionRate = 0;
            coolFire[1].emissionRate = 0;
        }

        if (transform.position.x > -60)
        {
            foreach (ParticleSystem ps in coolFire)
            {
                ParticleSystem.Particle[] allParticles = new ParticleSystem.Particle[ps.particleCount];

                ps.GetParticles(allParticles);
                for (int i = 0; i < allParticles.Length; i++)
                {
                    allParticles[i].velocity = new Vector2(0, 0);
                }
                ps.SetParticles(allParticles, ps.particleCount);
            }
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
        wasPressingJump = pressingJump;
        wasPressingShoot = pressingShoot;
        //I want to make it clear I know there's options for GetButtonUp/Down etc.
        //I'm setting this up so mobile input works ok
        pressingJump = Input.GetButton("A");
        pressingShoot = Input.GetButton("B");
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    void Shooting()
    {
        if (!wasPressingShoot && pressingShoot && shootCooldown <= 0)
        {
            SoundManager.instance.playSound(shootSound, 1, Random.Range(0.85f, 1.15f));
            shootCooldown = fireRate;
            Projectile proj = ProjectilePooler.instance.PoolProj(bulletStats, transform.position, moveSpeed * decellerationPerShot);
            // moveSpeed -= decellerationPerShot;
            moveSpeed *= 1 - decellerationPerShot;
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
                moveSpeed += speedToAdd;
                speedToAdd = 0;
                SoundManager.instance.managedAudioSources[1].AudioSrc.pitch = 0.5f;
            }
#endregion
            moveSpeed += IdleAccelerationCurve.Evaluate(moveSpeed / IdleAccLimit)*Time.deltaTime;
            jumping = false;
            jumpTimer = 0;
            fallTimer = 0;
            slowMo = 0;
            maxJumpTime = moveSpeed * 0.1f;
        }
        #endregion

        //Start jump
        if (!jumping && pressingJump && transform.position.y == floorHeight)
        {
            SoundManager.instance.ChangeMoveSound(false, GetSpeed() / 100f);
            jumping = true;
        }

        if (slowMo < PlayerPrefs.GetInt("Slow_Motion") && pressingJump)
        {
            frameHolder.instance.normalSpeed = 0.25f;
            slowMo += Time.fixedDeltaTime;
            Debug.Log("Slow mo: " + slowMo);
        }
        else
        {
            frameHolder.instance.normalSpeed = 1;
        }
        #region jumping upwards
        if (jumpTimer < maxJumpTime && jumping)
        {
            if (speedToAdd == 0)
            {
                speedToAdd = 0.01f;
            }
            Move(0, verticalSpeed * Time.deltaTime - (gravity * fallTimer * Time.deltaTime));
            jumpTimer += Time.deltaTime;
            if (jumpTimer > maxJumpTime)
                jumping = false;
        }
        #endregion

    }
    float gravity=250, fallTimer;
    [SerializeField]
    float slowMo;
}
