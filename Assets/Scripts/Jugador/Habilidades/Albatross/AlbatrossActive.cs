using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AlbatrossActive : ShootProjectileAbility
{
    [SerializeField] private GameObject stormGrenade;
    [SerializeField] private GameObject upgradedStormGrenade;
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
            if (!isUpgraded)
            {
                Debug.Log("Albatross no mejorado");
                proyectil = Instantiate(stormGrenade, spawn.position, spawn.rotation);
                
                Debug.Log("Albatros lanza habilidad" + proyectil.name);
                StormGrenade storm = proyectil.GetComponent<StormGrenade>();
            
                storm.InitializeGrenade(networkedPlayer.partida, (int)(damage + damage *  ((float)networkedPlayer.dmgBalance.Value/100f)));
                //Debug.Log(networkedPlayer.userName + proyectil.GetComponent<StormGrenade>().isUpgraded);
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
                Debug.Log("Albatross mejorado");
                proyectil = Instantiate(upgradedStormGrenade, spawn.position, spawn.rotation);
            
                Debug.Log("Albatros lanza habilidad" + proyectil.name);
                StormGrenade storm = proyectil.GetComponent<StormGrenade>();
            
                storm.InitializeGrenade(networkedPlayer.partida, (int)(damage + damage *  ((float)networkedPlayer.dmgBalance.Value/100f)));
                //Debug.Log(networkedPlayer.userName + proyectil.GetComponent<StormGrenade>().isUpgraded);
                Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();

                // Inicializamos el proyectil en el servidor
                proyectilScript.Inicializar(spawn.forward, networkedPlayer.GetComponentInChildren<CapsuleCollider>(), networkedPlayer, networkedPlayer.IsServer);
                //Debug.Log(neededResQuantity);
            
                networkedPlayer.UpdateAbilityUIClientRpc(Color.yellow);

                // Programamos la destrucci�n del proyectil despu�s de 10 segundos
                Invoke("CrearTormenta", tiempoDeVida);
            }
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
        networkedPlayer.UpdateCDAbilityUIClientRpc(actualResQuantity/neededResQuantity, isUpgraded);
    }

    private void CrearTormenta()
    {
        if (isActive)
        {
            isActive = false;
            networkedPlayer.UpdateCDAbilityUIClientRpc(neededResQuantity, isUpgraded);
            proyectil.GetComponent<StormGrenade>().Detonar(networkedPlayer);
            Destroy(proyectil);
        }
        
    }
    
}
