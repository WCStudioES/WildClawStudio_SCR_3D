using UnityEngine;
using Unity.Netcode;
using static UnityEngine.UI.GridLayoutGroup;

public class CargoQueenActive : ShieldAbility
{
    private GameObject shieldInstance; // Instancia del escudo
    public override void AbilityExecution()
    {
        Debug.Log("Cargo Queen lanza habilidad: " + networkedPlayer.IsServer);
        

        if (networkedPlayer.IsServer)
        {
            // Instanciamos el escudo en el servidor
            shieldInstance = Instantiate(shield, shieldSpawn.position, shieldSpawn.rotation);

            // Spawneamos el NetworkObject del escudo
            var networkObject = shieldInstance.GetComponent<NetworkObject>();
            networkObject.Spawn();

            // Aseguramos que el escudo est� bajo el transform de la nave
            shieldInstance.transform.SetParent(networkedPlayer.transform);

            // Aqu� podr�as llamar a una funci�n para inicializar el escudo si es necesario
            var shieldScript = shieldInstance.GetComponent<Shield>();
            shieldScript.Initialize(networkedPlayer, shieldSpawn);
            
        }
        networkedPlayer.uiBoosters.UpdateActiveImage(neededResQuantity);
    }
}
