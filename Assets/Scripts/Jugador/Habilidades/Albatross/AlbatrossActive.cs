using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AlbatrossActive : ShootProjectileAbility
{
    [SerializeField] private GameObject stormGrenade;
    private GameObject proyectil;
    public float tiempoDeVida;
    private bool isActive;
    [SerializeField] private int damage;
    public override void AbilityExecution()
    {
        Debug.Log("Albatros lanza habilidad");

        if (!isActive)
        {
            isActive = true;
            Transform spawn = GetComponentInParent<PlayerShip>().proyectileSpawns[0];

            //Debug.Log("Proyectil creado");
            proyectil = Instantiate(stormGrenade, spawn.position, spawn.rotation);
            proyectil.GetComponent<StormGrenade>().partida = networkedPlayer.partida;
            Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();

            // Inicializamos el proyectil en el servidor
            proyectilScript.Inicializar(spawn.forward, networkedPlayer.GetComponentInChildren<CapsuleCollider>(), networkedPlayer, networkedPlayer.IsServer);
            //Debug.Log(neededResQuantity);
            
            networkedPlayer.UpdateAbilityUIClientRpc(Color.yellow);

            // Programamos la destrucci�n del proyectil despu�s de 10 segundos
            Invoke("CrearTormenta", tiempoDeVida);
        }
        else
        {
            CrearTormenta();
        }
    }

    public override bool CheckAvailability()
    {
        if (actualResQuantity < neededResQuantity && !isActive)
        {
            return false;
        }

        return true;
    }

    public override void ResetRonda()
    {
        isActive = false;
        actualResQuantity = neededResQuantity;
        networkedPlayer.UpdateCDAbilityUIClientRpc(actualResQuantity/neededResQuantity);
    }

    private void CrearTormenta()
    {
        if (isActive)
        {
            isActive = false;
            networkedPlayer.UpdateCDAbilityUIClientRpc(neededResQuantity);
            proyectil.GetComponent<StormGrenade>().dmg = (int)(damage *  ((float)networkedPlayer.dmgBalance.Value/100f));
            proyectil.GetComponent<StormGrenade>().Detonar(networkedPlayer);
            Destroy(proyectil);
        }
        
    }
    
}
