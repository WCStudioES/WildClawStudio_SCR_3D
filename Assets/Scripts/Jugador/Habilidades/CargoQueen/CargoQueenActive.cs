using UnityEngine;
using Unity.Netcode;
using static UnityEngine.UI.GridLayoutGroup;
using System.Collections;

public class CargoQueenActive : ShieldAbility
{
    private GameObject shieldInstance; // Instancia del escudo
    private Coroutine shieldMonitorCoroutine;
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

            // Inicia el monitoreo del estado del escudo
            if (shieldMonitorCoroutine != null)
                StopCoroutine(shieldMonitorCoroutine); // Detén cualquier Coroutine previo
            shieldMonitorCoroutine = StartCoroutine(MonitorShieldInstance());
        }
        networkedPlayer.UpdateAbilityUIClientRpc(Color.yellow);
        //networkedPlayer.UpdateAbilityUIClientRpc(neededResQuantity);
    }

    private IEnumerator MonitorShieldInstance()
    {
        while (shieldInstance != null)
        {
            yield return null; // Espera un frame
        }

        // Cuando el escudo se destruye (shieldInstance == null)
        Debug.Log("El escudo ha sido destruido.");
        actualResQuantity = 0;
        networkedPlayer.UpdateCDAbilityUIClientRpc(neededResQuantity);

        // Opcional: Detén el Coroutine
        shieldMonitorCoroutine = null;
    }
    
}
