using UnityEngine;
using System.Collections;

public class trailPlacer : MonoBehaviour {

    public static trailPlacer instance;
    [SerializeField]
    ParticleSystem[] systems;

    void Start()
    {
        instance = this;
    }

    public void PlaceTrailParticle(Vector3 pos)
    {
        foreach (ParticleSystem ps in systems)
        {
            ps.transform.position = pos;
            ps.Emit(1);
        }
    }
}
