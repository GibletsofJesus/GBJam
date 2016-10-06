using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    public float moveSpeed,verticalSpeed,MaxHP,impactDamage;
    public Rigidbody2D rigBod;
    [HideInInspector]
    public float HP;
    public SpriteRenderer[] sr;
    public Color[] standardCol;
    public Animator anim;
    Vector3 _movement;
    
    public virtual void Start()
    {
        HP = MaxHP;
        standardCol = new Color[sr.Length];
        for (int i = 0; i < sr.Length; i++)
        {
            standardCol[i] = sr[i].color;
            standardCol[i] = Color.white;
        }
    }

    void Update()
    {

    }

    public void Move(float x, float y)
    {
        _movement.x += x;
        _movement.y += y;
    }

    public virtual void LateUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 clamped_position = new Vector2((int)pos.x, (int)pos.y);
        transform.position = clamped_position;


        // Clamp the current movement
        Vector3 clamped_movement = new Vector3((int)_movement.x, (int)_movement.y,0);
        // Check if a movement is needed (more than 1px move)
        if (clamped_movement.magnitude >= 1.0f)
        {
            // Update velocity, removing the actual movement
            _movement = _movement - clamped_movement;
            if (clamped_movement != Vector3.zero)
            {
                // Move to the new position
                transform.position = (transform.position) + clamped_movement;
            }
        }
    }

    public virtual IEnumerator TakeDamage(float damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Death();
        }
        int i = 0;
        foreach (SpriteRenderer s in sr)
        {
            s.enabled = true;
        }
        yield return new WaitForEndOfFrame();

        foreach (SpriteRenderer s in sr)
        {
            s.enabled = false;
        }
    }

    public virtual void Death()
    {
        gameObject.SetActive(false);
    }

    public virtual void Movement()
    {
        if (anim)
        {
            anim.SetBool("walking", (rigBod.velocity.magnitude / 10) > 0.1f ? true : false);
            anim.SetFloat("speed", 1  +rigBod.velocity.magnitude/10);
        }
    }
}
