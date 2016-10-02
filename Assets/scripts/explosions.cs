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
	void Update () {

    }

    public IEnumerator Explode()
    {
        yield return new WaitForEndOfFrame();
        sr.color = A;
        yield return new WaitForEndOfFrame();

        yield return new WaitForEndOfFrame();
        sr.color = B;
        yield return new WaitForEndOfFrame();
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
