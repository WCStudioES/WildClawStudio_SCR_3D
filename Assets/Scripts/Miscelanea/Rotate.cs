using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool xRot = false;
    public bool yRot = false;
    public bool zRot = false;

    public float rotSpeed = 0.0f;

    // Update is called once per frame
    void Update()
    {
        float speed = rotSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(xRot ? speed : 0, yRot ? speed : 0, zRot ? speed : 0));
    }
}
