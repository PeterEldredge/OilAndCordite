using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    public bool InSmog { get; private set; }
    public bool InGas { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.GAS_CLOUD))
        {
            InGas = true;
        }
        else if(other.CompareTag(Tags.SMOG))
        {
            InSmog = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.GAS_CLOUD))
        {
            InGas = false;
        }
        else if (other.CompareTag(Tags.SMOG))
        {
            InSmog = false;
        }
    }
}
