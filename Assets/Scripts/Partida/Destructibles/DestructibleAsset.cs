﻿using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.VFX;

public class DestructibleAsset : Damageable
{
    private bool resGiven = false;
    public Partida partida;

    [SerializeField] private GameObject destructionVFX;
    private VFXPrefab destructionVFXInstance;
    [SerializeField] private Image ResetTimeImage;
    [SerializeField] private float ResetTime;

    private void Start()
    {
        if (IsServer)
        {
            RenderManager.Instance.RegisterNewObject(gameObject);
            actualHealth.Value = maxHealth;
            //resToGive.Value = 100;
        }
    }

    // M�todo para recibir da�o en el meteorito
    public override void GetDamage(int dmg, NetworkedPlayer dmgDealer)
    {
        Debug.Log(IsServer + " " + partida.rondaEnmarcha + " RONDAENMARCHAAAAAAAAAA");
        if (IsServer && partida.rondaEnmarcha)
        {
            actualHealth.Value -= dmg;

            if(resType == ResourceToGive.Health)
            {
                dmgDealer.GetHealPercentage((int)((float)resToGive.Value * ((float)dmg/maxHealth)), dmgDealer);
                ActivateGetHealVFXForMatchPlayers(dmgDealer);
            }

            //Debug.Log("Vida del coso: " + actualHealth.Value);

            // Si la vida llega a 0, destruir el meteorito
            if (actualHealth.Value <= 0 && !resGiven)
            {
                resGiven = true;
                DestroyDamageable(dmgDealer);
            }
            else
            {
                ChangeMaterialColorClientRpc(0.1f);
            }
        }
    }

    /// <summary>
    /// DESTRUIR DESTRUCTIBLE
    /// </summary>

    protected override void DestroyDamageable(NetworkedPlayer dmgDealer)
    {
        if (IsServer)
        {
            switch (resType)
            {
                case ResourceToGive.None:
                    break;

                case ResourceToGive.Experience:
                    dmgDealer.GetXP(resToGive.Value);
                    break;

                case ResourceToGive.Health:
                    //dmgDealer.GetHealPercentage(resToGive.Value*2, dmgDealer);
                    StartCoroutine("ResetAssetAfterTime");
                    break;
            }
            StopFlashingAndCleanUp(); // Detener el flashing y restaurar colores
            StartCoroutine(DestroyWithDelay());

            PlayDestructionSFXClientRpc();
        }
    }

    protected IEnumerator DestroyWithDelay()
    {
        ActivateDestructionVFXForMatchPlayers();
        yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
        SetAssetActive(false); // Desactiva el destructible en el servidor
        DisableDamageableClientRpc(); // Sincroniza la desactivación en los clientes
    }

    protected IEnumerator ResetAssetAfterTime()
    {
        float time = ResetTime;
        StartCoroutine("ResetCountdownClientRpc");
        while (time > 0)
        {
            yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
            time -= 0.1f;
        }
        RestoreDestructibleAsset();
    }


    [ClientRpc]
    protected void ResetCountdownClientRpc()
    {
        StartCoroutine("EnumeratorReset");
    }

    protected IEnumerator EnumeratorReset()
    {
        float time = ResetTime;
        ResetTimeImage.gameObject.SetActive(true);
        while (time > 0)
        {
            yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
            time -= 0.1f;
            ResetTimeImage.fillAmount = time / ResetTime;
        }
        ResetTimeImage.gameObject.SetActive(false);
    }
    // Metodo para destruir el meteorito en el cliente
    [ClientRpc]
    public void DisableDamageableClientRpc()
    {
        StopFlashingAndCleanUp();
        isFlashing = false;
        SetAssetActive(false);
    }


    /// <summary>
    /// RESTAURAR DESTRUCTIBLE
    /// </summary>

    //Funcion para restaurar el meteorito con su hp
    public void RestoreDestructibleAsset()
    {
        if (IsServer)
        {
            actualHealth.Value = maxHealth;
            RestoreDestructibleAssetClientRpc();
            resGiven = false;
        }
        SetAssetActive(true);
    }
    
    //Funcion para restaurar el meteorito con el hp dado
    public void RestoreDestructibleAsset(int health)
    {
        if (IsServer)
        {
            maxHealth = health;
            actualHealth.Value = health;
            RestoreDestructibleAssetClientRpc();
            resGiven = false;
            StopCoroutine("ResetAssetAfterTime");
        }
        SetAssetActive(true);
    }
    
    //Funcion para restaurar el meteorito con su hp con tamaño y experiencia aleatoria
    public void RestoreRandomDestructibleAsset()
    {
        SetAssetActive(true);
        if (IsServer)
        {
            //int hpAponer

            actualHealth.Value = maxHealth;
            RestoreDestructibleAssetClientRpc();
        }
    }

    [ClientRpc]
    private void RestoreDestructibleAssetClientRpc()
    {
        SetAssetActive(true);
        if(resType == ResourceToGive.Health)
            ResetTimeImage.gameObject.SetActive(false);
    }

    protected virtual void SetAssetActive(bool active)
    {
        if(assetCollider!= null)
        {
            assetCollider.enabled = active;
        }

        if(assetRenderer!= null)
        {
            assetRenderer.enabled = active;
        }
    }


    private void ActivateDestructionVFXForMatchPlayers()
    {
        if (IsServer)
        {
            // Crear la lista de clientes objetivo
            List<ulong> targetClientIds = new List<ulong>();
            foreach (var player in partida.jugadores)
            {
                targetClientIds.Add(player.OwnerClientId);
            }

            // Configurar los ClientRpcParams
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = targetClientIds.ToArray()
                }
            };

            // Llamar al ClientRpc con los parámetros personalizados
            ActivateDestructionVFXClientRpc(clientRpcParams);
        }
    }

    private void ActivateGetHealVFXForMatchPlayers(NetworkedPlayer jugador)
    {
        if (IsServer)
        {
            // Crear la lista de clientes objetivo
            List<ulong> targetClientIds = new List<ulong>();
            foreach (var player in partida.jugadores)
            {
                targetClientIds.Add(player.OwnerClientId);
            }

            // Configurar los ClientRpcParams
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = targetClientIds.ToArray()
                }
            };

            // Llamar al ClientRpc con los parámetros personalizados
            jugador.ActivateGetHealVFXClientRpc(clientRpcParams);
        }
    }

    [ClientRpc]
    public void ActivateDestructionVFXClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Activando vfx de meteorito");
        if(this is Meteorito)
        {
            VFXManager.Instance.SpawnVFX(VFXManager.VFXType.meteorite, transform.position, Quaternion.identity);
        }
    }
}
