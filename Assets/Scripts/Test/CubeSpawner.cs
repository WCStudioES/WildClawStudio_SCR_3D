using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{

    [SerializeField] private NetworkObject cubo;

    public bool iniciarOrden66 = false;
    public int cantidad = 100;

    private bool hecho = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hecho)
        {
            if (iniciarOrden66)
            {
                for (int i = 0; i < cantidad; i++)
                {
                    NetworkObject nuevoCubo = Instantiate(cubo);
                    nuevoCubo.Spawn();
                }

                hecho = true;
            }
        }
    }
}
