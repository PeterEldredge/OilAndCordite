using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeTest : MonoBehaviour
{
    [SerializeField]
    private float despawnDelay, minForce, maxForce, radius;
    [SerializeField]
    private GameObject explosionParticles;
    [SerializeField]
    private Vector3 explosionOffset;


    public void Explode()
    {
        if (explosionParticles != null)
        {
            GameObject explosionFX = Instantiate(explosionParticles, transform.position, Quaternion.identity) as GameObject;
            Debug.Log("Particles Spawned");
            Destroy(explosionFX, despawnDelay);
        }

        foreach (Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);

                Debug.Log("EXPLOOOOSSSSSSIONNN");
            }

            Destroy(t.gameObject, despawnDelay);
            Debug.Log("Destroyed");
        }
        
    }
}
