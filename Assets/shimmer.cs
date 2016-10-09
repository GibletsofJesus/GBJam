using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shimmer : MonoBehaviour {

    public float interval,animSpeed;
    float cooldown;
    [SerializeField]
    Animator shimmerAnimator;
    // Use this for initialization
    void Start()
    {
        cooldown = interval;
    }

    void OnEnable()
    {
        shimmerAnimator.Play("shimmer_idle");
    }

    // Update is called once per frame
    void Update()
    {
        shimmerAnimator.SetFloat("speed", animSpeed);
        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        if (cooldown <= 0)
        {
            shimmerAnimator.Play("swhing");
            cooldown = interval;
        }
    }
}
