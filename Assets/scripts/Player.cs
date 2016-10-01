using UnityEngine;
using System.Collections;

public class Player : Actor
{
    public static Player instance;
    
    public float deadzone, fireRate, floorHeight,maxJumpTime;
    [SerializeField]
    Sprite[] charSprites;
    [SerializeField]
    Transform[] shootTransforms;
    [SerializeField]
    Projectile.ProjData bulletStats;
    [SerializeField]
    ParticleSystem[] bulletCasings;
    [SerializeField]
    ParticleSystem muzzleFlash;
    [SerializeField]
    AudioClip shootSound;
    float shootCooldown,abilityCooldown,jumpTimer;
    
    public override void Start()
    {
        base.Start();
        instance = this;
    }
    
    void Update()
    {
        Movement();

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CameraShake.instance)
            {
                if (CameraShake.instance.shakeDuration < 1)
                    CameraShake.instance.shakeDuration = 1;
                CameraShake.instance.shakeAmount = 0.5f;
            }
        }*/
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    [SerializeField]
    bool jumping=false;

    public override void Movement()
    {
        base.Movement();
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > deadzone)
        {
            //rigBod.AddForce(Vector2.right * moveSpeed * Input.GetAxis("Horizontal"));
            Move(moveSpeed * Time.deltaTime * (Input.GetAxis("Horizontal") > 0 ? 1 : -1), 0);
        }

        #region jumping
        //When on ground a jump pressed, play sound start jump
        if (transform.position.y < floorHeight)
            transform.position = new Vector3(transform.position.x, floorHeight, 0);

        if (transform.position.y == floorHeight && !Input.GetButton("Jump"))
        {
            Debug.Log("landed");
            jumping = false;
            jumpTimer = 0;
            fallTimer = 0;
        }

        if (!jumping && Input.GetButtonDown("Jump") && transform.position.y == floorHeight)
        {
            jumping = true;
        }

        if (Input.GetButtonUp("Jump") && transform.position.y > floorHeight)
        {
            jumping = false;
        }


        if (jumping && jumpTimer < maxJumpTime)
        {
            Debug.Log("Jumping  UP");
            Move(0, verticalSpeed * Time.deltaTime -(gravity*jumpTimer*Time.deltaTime));
            jumpTimer += Time.deltaTime;
            if (jumpTimer > maxJumpTime)
                jumping = false;
        }

        //gravity
        if (transform.position.y > floorHeight)
        {
            if (!jumping)
            {
                Move(0, -Time.deltaTime * gravity*fallTimer);
                fallTimer += Time.deltaTime*3;
            }

        }
        #endregion
    }
    public float gravity,fallTimer;
}
