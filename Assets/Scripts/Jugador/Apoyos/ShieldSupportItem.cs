using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShieldSupportItem : SupportItem
{
    private GameObject shieldInstance; // Instancia del escudo
    public override void AddToPlayer()
    {
        // Instanciamos el escudo en el servidor
        shieldInstance = Instantiate(
            supportItemPrefab,
            owner.cuerpoNave.GetComponent<PlayerShip>().transform.position,
            owner.cuerpoNave.GetComponent<PlayerShip>().transform.rotation
        );

        // Spawneamos el NetworkObject del escudo
        var networkObject = shieldInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();

        // Aseguramos que el escudo est� bajo el transform de la nave
        shieldInstance.transform.SetParent(owner.transform);

        // Inicializamos el escudo con el propietario y posici�n
        var shieldScript = shieldInstance.GetComponent<VisualShield>();
        shieldScript.Initialize(owner, owner.cuerpoNave.GetComponent<PlayerShip>().transform, owner.selectedShip.Value+1);

        // NOTA: El modelo visual ser� configurado por el ClientRpc O NO
        if (shieldScript != null)
        {
            owner.UpdateShieldBarClientRpc(shieldScript.GetMaxHealth(), false);
        }
    }
}
