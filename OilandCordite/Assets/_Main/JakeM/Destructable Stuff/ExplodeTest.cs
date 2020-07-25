using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeTest : MonoBehaviour
{
    [SerializeField]
    private float despawnDelay, minForce, maxForce, radius;


    // Start is called before the first frame update
    void Start()
    {
        Explode();
    }


    public void Explode()
    {
        foreach (Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);
                //t.GetComponent<MeshCollider>().enabled = false; DISABLE MESH COLLIDER ON PIECES
            }

            Destroy(t.gameObject, despawnDelay);
        }
        
    }
}
