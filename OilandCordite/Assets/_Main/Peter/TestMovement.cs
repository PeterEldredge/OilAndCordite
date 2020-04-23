using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W)) transform.Translate(new Vector3(0, 0, 1) * _speed);
        if (Input.GetKey(KeyCode.S)) transform.Translate(new Vector3(0, 0, -1) * _speed);
        if (Input.GetKey(KeyCode.A)) transform.Translate(new Vector3(-1, 0, 0) * _speed);
        if (Input.GetKey(KeyCode.D)) transform.Translate(new Vector3(1, 0, 0) * _speed);
        if (Input.GetMouseButton(0)) transform.Translate(new Vector3(0, -1, 0) * _speed);
        if (Input.GetMouseButton(1)) transform.Translate(new Vector3(0, 1, 0) * _speed);

        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
    }
}
