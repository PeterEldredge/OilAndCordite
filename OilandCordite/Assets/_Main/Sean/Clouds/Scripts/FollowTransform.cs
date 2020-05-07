using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform followTarget;
    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(followTarget.position.x, this.transform.position.y, followTarget.position.z);
    }
}
