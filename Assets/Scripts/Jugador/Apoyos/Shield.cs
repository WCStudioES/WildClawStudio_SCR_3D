using DefaultNamespace;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Shield : NetworkBehaviour, IDamageable
{
    public NetworkVariable<int> actualHealth = new NetworkVariable<int>(50); // Salud sincronizada
    public int maxHealth = 50; // Salud máxima
    public float duration = 5f; // Duración del escudo en segundos

    public NetworkVariable<ulong> ownerClientId = new NetworkVariable<ulong>();
    public NetworkedPlayer owner;
    public Transform spawn;

    public void Initialize(NetworkedPlayer pOwner, Transform pSpawn)
    {
        Debug.Log("Inicializando escudo");
        owner = pOwner;
        ownerClientId.Value = pOwner.OwnerClientId;
        spawn = pSpawn;

        // Esto podría ocurrir solo en el servidor
        actualHealth.Value = maxHealth; // Inicializar salud

        StartCoroutine(StartShieldTimer());
    }

    public void GetDamage(int damage, NetworkedPlayer dmgDealer)
    {
        actualHealth.Value -= damage; // Resta vida y sincroniza con los clientes

        if(actualHealth.Value <= 0 )
        {
            StartCoroutine("DestroyWithDelay");
        }
    }

    public IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
        DestroyShield();
        DisableShieldClientRpc();
    }

    private void DestroyShield()
    {
        Debug.Log("Escudo Destruido con " + actualHealth.Value);
        Destroy(gameObject);
    }

    [ClientRpc]
    private void DisableShieldClientRpc()
    {
        DestroyShield();
    }

    private IEnumerator StartShieldTimer()
    {
        yield return new WaitForSeconds(duration);
        // Llamamos a ServerRpc para desactivar el escudo después de la duración
        DestroyShield();
        DisableShieldClientRpc();
    }

    private void Update()
    {
        if (IsServer)
        {
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }
    }

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();

    //    if (!IsServer)
    //    {
    //        // En los clientes, convertimos ownerClientId en una referencia al jugador
    //        TryAssignOwner();
    //    }
    //}

    //private void TryAssignOwner()
    //{
    //    // Intentamos buscar el NetworkObject asociado al ownerClientId
    //    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(ownerClientId.Value, out var networkObject))
    //    {
    //        owner = networkObject.GetComponent<NetworkedPlayer>();
    //        if (owner != null)
    //        {
    //            Debug.Log($"Escudo asignado correctamente al jugador: {owner.name} en cliente.");
    //            UpdateOwnerVisuals();
    //        }
    //        else
    //        {
    //            Debug.LogError("No se encontró un NetworkedPlayer para el owner. Reintentando...");
    //            Invoke(nameof(TryAssignOwner), 0.1f); // Reintentar después de 100 ms
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("No se encontró un objeto de red con el ID del owner. Reintentando...");
    //        Invoke(nameof(TryAssignOwner), 0.1f); // Reintentar después de 100 ms
    //    }
    //}

    //private void UpdateOwnerVisuals()
    //{
    //    // Aquí puedes cambiar visuales basados en el dueño, por ejemplo:
    //    if (owner != null)
    //    {
    //        //var renderer = GetComponentInChildren<MeshRenderer>();
    //        //renderer.material.color = owner.TeamColor; // Ejemplo: cambiar el color según el equipo del jugador
    //    }
    //}
}
