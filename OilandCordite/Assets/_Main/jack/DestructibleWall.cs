using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [SerializeField]
    private GameObject originalObject, fracturedObject;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(Tags.PLAYER))
        {
            originalObject.SetActive(false);
            fracturedObject.SetActive(true);
            
            //for (int i = 0; i < fracturedObject.transform.childCount; i++)
            //{
            //    fracturedObject.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector3(1, 0, 0));
            //}
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
