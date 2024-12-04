using System;
using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : DestructibleAsset
{
    public float duration = 5f; // Duraci�n del escudo en segundos

    public NetworkedPlayer owner;
    public Transform spawn;

    public virtual void Initialize(NetworkedPlayer pOwner, Transform pSpawn)
    {
        Debug.Log("Inicializando escudo");
        owner = pOwner;
        spawn = pSpawn;

        // Esto podr�a ocurrir solo en el servidor
        actualHealth.Value = maxHealth; // Inicializar salud

        StartCoroutine(StartShieldTimer());
        
        if (this is VisualShield)
        {
            owner.UpdateShieldBarClientRpc(maxHealth, owner.maxHealth.Value);
        }
    }

    public void SetHealth(int health)
    {
        maxHealth = health;
        actualHealth.Value = health;
    }

    public override void GetDamage(int damage, NetworkedPlayer dmgDealer)
    {
        actualHealth.Value -= damage; // Resta vida y sincroniza con los clientes
        if (this is VisualShield)
            owner.UpdateShieldBarClientRpc(actualHealth.Value, owner.maxHealth.Value);
        if(actualHealth.Value <= 0 )
        {
            DestroyDamageable(dmgDealer);

            if(this is VisualShield)
            {
                owner.RemoveShield(this);
            }
        }
        else
        {
            ChangeMaterialColorClientRpc(0.1f);
        }
    }

    private IEnumerator StartShieldTimer()
    {
        yield return new WaitForSeconds(duration);

        DestroyDamageable(owner);
    }

    public NetworkedPlayer IsChildOfPlayer()
    {
        // Recorre la jerarqu�a de padres para ver si alguno coincide con el transform del jugador
        Transform current = transform;
        while (current != null)
        {
            if (current.GetComponent<NetworkedPlayer>() != null)
            {
                return current.GetComponent<NetworkedPlayer>(); // El escudo es hijo de un jugador
            }
            current = current.parent;
        }

        return null; // No se encontr� relaci�n con el jugador
    }

    protected override void SetAssetActive(bool active)
    {
        Destroy(gameObject);
    }


    private void Update()
    {
        if (IsServer && spawn != null)
        {
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }
    }
}
