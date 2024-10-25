using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Meteorito : NetworkBehaviour, ICanGetDamage
{
    public int hpTotal = 100;
    
    //Rango de valores de vida que puede tener el meteorito
    public int hpMaximo = 100;
    public int hpMinimo= 20;
    
    //Rango de escalas de tamaño que puede tener el meteorito
    public float escalaMaxima = 4f;
    public float escalaMinima = 2f;
    
    public NetworkVariable<int> hpActual = new NetworkVariable<int>(100); // Vida actual del meteorito
    public NetworkVariable<int> xpADar = new NetworkVariable<int>(100); // Experiencia que da el meteorito al destruirlo

    private void Start()
    {
        if (IsServer)
        {
            hpActual.Value = 100;
            xpADar.Value = 100;
        }
    }

    // M�todo para recibir da�o en el meteorito
    public void GetDamage(int dmg, ControladorDelJugador dueñoDaño)
    {
        if (IsServer)
        {
            hpActual.Value -= dmg;
            Debug.Log("Vida del meteorito: " + hpActual.Value);

            // Si la vida llega a 0, destruir el meteorito
            if (hpActual.Value <= 0)
            {
                DestruirMeteorito(dueñoDaño);
            }
        }
    }

    // Metodo para destruir el meteorito
    private void DestruirMeteorito(ControladorDelJugador dueñoDaño)
    {
        if (IsServer)
        {
            dueñoDaño.GetXP(xpADar.Value);
            Debug.Log("XpDada del meteorito: " + xpADar.Value);
            Debug.Log("Xp de jugador: " + dueñoDaño.xp.Value);
            gameObject.SetActive(false);
            DestruirMeteoritoClientRpc();
            //Para testear, de moemnto se resetea cada 1 segundo
            //Invoke("RestaurarMeteorito", 1f); 
        }
    }

    // Metodo para destruir el meteorito en el cliente
    [ClientRpc]
    private void DestruirMeteoritoClientRpc()
    {
        gameObject.SetActive(false);
    }

    //Funcion para restaurar el meteorito con su hp
    public void RestaurarMeteorito()
    {
        if (IsServer)
        {
            hpActual.Value = hpTotal;
            RestaurarMeteoritoClientRpc();
        }
        gameObject.SetActive(true);
    }
    //Funcion para restaurar el meteorito con su hp con tamaño y experiencia aleatoria
    public void RestaurarMeteoritoAleatorio()
    {
        gameObject.SetActive(true);
        if (IsServer)
        {
            //int hpAponer
            
            hpActual.Value = hpTotal;
            RestaurarMeteoritoClientRpc();
        }
    }
    
    [ClientRpc]
    private void RestaurarMeteoritoClientRpc()
    {
        
        gameObject.SetActive(true);
    }
}
