using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;

public class Debris : NetworkBehaviour, IDamageable
{
    public int hpTotal = 50;
    
    //Rango de valores de vida que puede tener el meteorito
    public int hpMaximo = 100;
    public int hpMinimo= 20;
    
    //Rango de escalas de tamaño que puede tener el meteorito
    public float escalaMaxima = 4f;
    public float escalaMinima = 2f;
    
    public NetworkVariable<int> hpActual = new NetworkVariable<int>(100); // Vida actual del meteorito
    public NetworkVariable<int> hpADarAlRecibirDaño = new NetworkVariable<int>(5); // Vida que restaura el meteorito al recibir daño
    public NetworkVariable<int> hpADarAlDestruir = new NetworkVariable<int>(5); // Vida que restaura el meteorito al ser destruido

    private void Start()
    {
        if (IsServer)
        {
            hpActual.Value = hpTotal;
            //xpADar.Value = 100;
        }
    }

    // M�todo para recibir da�o en el meteorito
    public void GetDamage(int dmg, NetworkedPlayer dueñoDaño)
    {
        if (IsServer)
        {
            hpActual.Value -= dmg;
            Debug.Log("Vida del meteorito: " + hpActual.Value);

            // Si la vida llega a 0, destruir el meteorito
            if (hpActual.Value <= 0)
            {
                DestruirDebris(dueñoDaño);
                
            }
            else
            {
                dueñoDaño.actualHealth.Value += hpADarAlRecibirDaño.Value;
                Debug.Log("HP al recibir daño debris: " + hpADarAlRecibirDaño.Value);
                Debug.Log("Hp de jugador: " + dueñoDaño.actualHealth.Value);
            }
        }
    }

    // Metodo para destruir el meteorito
    private void DestruirDebris(NetworkedPlayer dueñoDaño)
    {
        if (IsServer)
        {
            dueñoDaño.GetXP(hpADarAlDestruir.Value);
            Debug.Log("HP al morir que da el debris: " + hpADarAlDestruir.Value);
            Debug.Log("Hp de jugador: " + dueñoDaño.actualHealth.Value);
            gameObject.SetActive(false);
            DestruirDebrisClientRpc();
            //Para testear, de moemnto se resetea cada 1 segundo
            //Invoke("RestaurarMeteorito", 1f); 
        }
    }

    // Metodo para destruir el meteorito en el cliente
    [ClientRpc]
    private void DestruirDebrisClientRpc()
    {
        gameObject.SetActive(false);
    }

    //Funcion para restaurar el meteorito con su hp
    public void RestaurarDebris()
    {
        if (IsServer)
        {
            hpActual.Value = hpTotal;
            RestaurarDebrisClientRpc();
        }
        gameObject.SetActive(true);
    }
    //Funcion para restaurar el meteorito con su hp con tamaño y experiencia aleatoria
    public void RestaurarDebrisAleatorio()
    {
        gameObject.SetActive(true);
        if (IsServer)
        {
            //int hpAponer
            
            hpActual.Value = hpTotal;
            RestaurarDebrisClientRpc();
        }
    }
    
    [ClientRpc]
    private void RestaurarDebrisClientRpc()
    {
        
        gameObject.SetActive(true);
    }
}
