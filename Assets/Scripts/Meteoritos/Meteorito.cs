using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;

public class Meteorito : NetworkBehaviour, ICanGetDamage
{
    public NetworkVariable<int> hp = new NetworkVariable<int>(100); // Vida inicial del meteorito

    private void Start()
    {
        if (IsServer)
        {
            hp.Value = 100;
        }
    }

    // M�todo para recibir da�o en el meteorito
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

    // Metodo para destruir el meteorito
    private void DestruirMeteorito()
    {
        
        gameObject.SetActive(false);
        DestruirMeteoritoClientRpc();
    }

    [ClientRpc]
    private void DestruirMeteoritoClientRpc()
    {
        
        gameObject.SetActive(false);
    }
}
