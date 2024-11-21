using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 target = new Vector3(0f, -90f, 0f);

    private void Start()
    {
        // Obtén la cámara principal
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Haz que este objeto mire directamente a la cámara
            transform.rotation = Quaternion.LookRotation(target);
        }
    }
}
