using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempCamMove : MonoBehaviour
{

    public GameObject target;
    public float temp = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        temp -= 0.005f;
        //transform.position += new Vector3(-0.1f, 0, 0);
        transform.RotateAround(target.transform.position, Vector3.up, 5 * Time.deltaTime);
        transform.RotateAround(target.transform.position, Vector3.right*temp, 1 * Time.deltaTime);

        if (temp < -1) temp = 1;
    }
}
