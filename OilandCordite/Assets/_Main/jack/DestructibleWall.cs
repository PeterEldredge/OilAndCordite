using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [SerializeField]
    private GameObject originalObject, fracturedObject;

    [SerializeField]
    private bool useTrigger;

    private void OnTriggerEnter(Collider collider)
    {
        if ((collider.CompareTag(Tags.PLAYER) || collider.CompareTag("PlayerAttack")) && useTrigger)
        {
            SwapObjects();
        }
    }

    public void SwapObjects()
    {
        if (fracturedObject != null)
        {
            fracturedObject.SetActive(true);
        }

        if (originalObject != null)
        {
            originalObject.SetActive(false);
        }
        
    }
}
