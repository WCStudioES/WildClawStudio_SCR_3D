using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Meteorito : NetworkBehaviour
{
    public NetworkVariable<int> hp = new NetworkVariable<int>(100); // Vida inicial del meteorito

    private void Start()
    {
        if (IsServer)
        {
            hp.Value = 100;
        }
    }

    // Método para recibir daño en el meteorito
    public void GetDamage(int dmg)
    {
        if (IsServer)
        {
            hp.Value -= dmg;
            Debug.Log("Vida del meteorito: " + hp.Value);

            // Si la vida llega a 0, destruir el meteorito
            if (hp.Value <= 0)
            {
                DestruirMeteorito();
            }
        }
    }

    // Método para destruir el meteorito
    private void DestruirMeteorito()
    {
        Debug.Log("Meteorito destruido");
        Destroy(this.gameObject);
    }
}
