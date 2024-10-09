using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Cubo : NetworkBehaviour
{
    private float Speed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        Speed = Random.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            transform.position = new Vector3(Time.deltaTime * Speed,Time.deltaTime * Speed,Time.deltaTime * Speed)  +  transform.position;
            transform.Rotate(new Vector3(Time.deltaTime * Speed,Time.deltaTime * Speed,Time.deltaTime * Speed));
        }

        
    }
}
