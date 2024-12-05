using UnityEngine;
using Unity.Netcode;
using static UnityEngine.UI.GridLayoutGroup;
using System.Collections;

public class CargoQueenActive : ShieldAbility
{
    private GameObject shieldInstance; // Instancia del escudo
    [SerializeField] private int health;
    [SerializeField] private float healthScalationPercent;
    public int timeIncrementation;
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
            if (isUpgraded)
            {
                shieldScript.duration += timeIncrementation;
                Debug.Log("Cargo mejorada");
            }
            shieldScript.Initialize(networkedPlayer, shieldSpawn);
            int healthToGive = isUpgraded ? health + 100 : health;
            shieldScript.SetHealth(healthToGive + (int)(networkedPlayer.maxHealth.Value* (healthScalationPercent / 100f)));
            

            StartCoroutine(MonitorShieldInstance());
        }
        networkedPlayer.UpdateAbilityUIClientRpc(Color.yellow);
        networkedPlayer.UpdateAbilityUIClientRpc(neededResQuantity);
    }

    private IEnumerator MonitorShieldInstance()
    {
        while (shieldInstance != null && shieldInstance.activeSelf)
        {
            //Debug.Log("CORUTINA");
            yield return null; // Espera un frame
        }

        // Cuando el escudo se destruye (shieldInstance == null)
        Debug.Log("El escudo ha sido destruido.");
        actualResQuantity = 0;
        networkedPlayer.UpdateCDAbilityUIClientRpc(neededResQuantity, isUpgraded);
    }

    public override void ResetRonda()
    {
        Destroy(shieldInstance);
        
        actualResQuantity = neededResQuantity;
        networkedPlayer.UpdateCDAbilityUIClientRpc(actualResQuantity/neededResQuantity, isUpgraded);
    }
}
