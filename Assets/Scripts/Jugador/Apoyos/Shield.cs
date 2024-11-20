using DefaultNamespace;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : NetworkBehaviour, IDamageable
{
    public NetworkVariable<int> actualHealth = new NetworkVariable<int>(50); // Salud sincronizada
    public int maxHealth = 50; // Salud máxima
    public float duration = 5f; // Duración del escudo en segundos

    public NetworkedPlayer owner;
    public Transform spawn;

    public virtual void Initialize(NetworkedPlayer pOwner, Transform pSpawn)
    {
        Debug.Log("Inicializando escudo");
        owner = pOwner;
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

    public NetworkedPlayer IsChildOfPlayer()
    {
        // Recorre la jerarquía de padres para ver si alguno coincide con el transform del jugador
        Transform current = transform;
        while (current != null)
        {
            if (current.GetComponent<NetworkedPlayer>() != null)
            {
                return current.GetComponent<NetworkedPlayer>(); // El escudo es hijo del NetworkedPlayer objetivo
            }
            current = current.parent;
        }

        return null; // No se encontró relación con el jugador
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
