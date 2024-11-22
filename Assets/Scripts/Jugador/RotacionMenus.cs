using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionMenus : MonoBehaviour
{
    public int rotacionSegundo;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.forward, rotacionSegundo * Time.deltaTime);
    }
}
