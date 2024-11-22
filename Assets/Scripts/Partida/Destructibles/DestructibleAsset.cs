using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestructibleAsset : Damageable
{
    private bool resGiven = false;
    private void Start()
    {
        if (IsServer)
        {
            actualHealth.Value = maxHealth;
            //resToGive.Value = 100;
        }
    }

    // M�todo para recibir da�o en el meteorito
    public override void GetDamage(int dmg, NetworkedPlayer dueñoDaño)
    {
        if (IsServer)
        {
            actualHealth.Value -= dmg;

            if(resType == ResourceToGive.Health)
            {
                dueñoDaño.GetHeal(resToGive.Value, dueñoDaño);
            }

            Debug.Log("Vida del coso: " + actualHealth.Value);

            // Si la vida llega a 0, destruir el meteorito
            if (actualHealth.Value <= 0 && !resGiven)
            {
                resGiven = true;
                DestroyDamageable(dueñoDaño);
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

    protected override void DestroyDamageable(NetworkedPlayer dueñoDaño)
    {
        if (IsServer)
        {
            switch (resType)
            {
                case ResourceToGive.None:
                    break;

                case ResourceToGive.Experience:
                    dueñoDaño.GetXP(resToGive.Value);
                    break;

                case ResourceToGive.Health:
                    dueñoDaño.GetHeal(resToGive.Value, dueñoDaño);
                    break;
            }
            StopFlashingAndCleanUp(); // Detener el flashing y restaurar colores
            StartCoroutine("DestroyWithDelay");
        }
    }

    protected IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
        gameObject.SetActive(false); // Desactiva el destructible en el servidor
        DisableDamageableClientRpc(); // Sincroniza la desactivación en los clientes
    }

    // Metodo para destruir el meteorito en el cliente
    [ClientRpc]
    protected void DisableDamageableClientRpc()
    {
        gameObject.SetActive(false);
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
        }
        gameObject.SetActive(true);
    }
    //Funcion para restaurar el meteorito con su hp con tamaño y experiencia aleatoria
    public void RestoreRandomDestructibleAsset()
    {
        gameObject.SetActive(true);
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
        gameObject.SetActive(true);
    }
}
