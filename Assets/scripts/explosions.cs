using UnityEngine;
using System.Collections;

public class explosions : MonoBehaviour, IPoolable<explosions>
{
    public PoolData<explosions> poolData { get; set; }
    [SerializeField]
    Color A, B;
    [SerializeField]
    SpriteRenderer sr;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        transform.position -= Vector3.right * Player.instance.moveSpeed * Time.deltaTime*50;
    }

    public IEnumerator Explode()
    {
        sr.color = A;
        yield return new WaitForSeconds(0.05f);

        yield return new WaitForEndOfFrame();
        sr.color = B;
        yield return new WaitForSeconds(0.1f);
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
