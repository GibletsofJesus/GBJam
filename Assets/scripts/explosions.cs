using UnityEngine;
using System.Collections;

public class explosions : MonoBehaviour, IPoolable<explosions>
{
    public PoolData<explosions> poolData { get; set; }
    [SerializeField]
    Color A, B;
    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    ParticleSystem[] particles; 
    
    // Update is called once per frame
    void Update()
    {
        transform.position -= Vector3.right * Player.instance.moveSpeed * Time.deltaTime*50;
    }

    public IEnumerator Explode()
    {
        sr.enabled = true;
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
        yield return new WaitForEndOfFrame();
        sr.color = A;
        yield return new WaitForEndOfFrame();

        yield return new WaitForEndOfFrame();
        sr.color = B;
        yield return new WaitForEndOfFrame();
        sr.enabled = false;
        yield return new WaitForSeconds(1);
        ReturnPool();
    }

    public void OnPooled(Vector3 startPos,Vector3 startScale)
    {
        //set everything up
        transform.localScale = startScale;
        transform.position = startPos;
        gameObject.SetActive(true);
        StartCoroutine(Explode());
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
