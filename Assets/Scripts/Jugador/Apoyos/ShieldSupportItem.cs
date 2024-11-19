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
        shieldInstance = Instantiate(supportItemPrefab, owner.cuerpoNave.GetComponent<PlayerShip>().transform.position, owner.cuerpoNave.GetComponent<PlayerShip>().transform.rotation);

        // Spawneamos el NetworkObject del escudo
        var networkObject = shieldInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();

        // Aseguramos que el escudo esté bajo el transform de la nave
        shieldInstance.transform.SetParent(owner.transform);

        // Aquí podrías llamar a una función para inicializar el escudo si es necesario
        var shieldScript = shieldInstance.GetComponent<Shield>();
        shieldScript.Initialize(owner, owner.cuerpoNave.GetComponent<PlayerShip>().transform);
    }
}
