using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToExplode : MonoBehaviour
{
    [SerializeField]
    private GameObject originalObject, fracturedObject;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnFracturedObject();
        }
    }


    void SpawnFracturedObject()
    {
        Destroy(originalObject);
        GameObject fractObj = Instantiate(fracturedObject) as GameObject;
        fractObj.GetComponent<ExplodeTest>().Explode();
    }

}
