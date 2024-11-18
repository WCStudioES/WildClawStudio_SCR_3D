using DefaultNamespace;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Shield : MonoBehaviour, IDamageable
{
    public int actualHealth = 50; // Vida del escudo
    public int maxHealth = 50;    // Vida m�xima del escudo

    public NetworkedPlayer owner; // Propietario del escudo

    public float duration; // Duraci�n del escudo en segundos

    // Inicializaci�n del escudo
    public void Initialize(NetworkedPlayer pOwner)
    {
        owner = pOwner;

        // Activa el escudo localmente en el servidor y sincroniza a los clientes
        SpawnShield();
        NotifyClients(true);

        // Comienza el temporizador para destruir el escudo
        StartCoroutine(StartShieldTimer());
    }

    // M�todo para activar el escudo
    public void SpawnShield()
    {
        Debug.Log("Shield spawned");
        gameObject.SetActive(true);
        actualHealth = maxHealth;
    }

    // M�todo para aplicar da�o al escudo
    public void GetDamage(int damage, NetworkedPlayer dmgDealer)
    {
        if (!owner.IsServer) return; // Solo el servidor gestiona el da�o

        Debug.Log("Escudo da�ado: " + actualHealth + " - " + damage);
        actualHealth -= damage;

        if (actualHealth <= 0)
        {
            StartCoroutine("DestroyWithDelay");
        }
    }
    public IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
        gameObject.SetActive(false); // Desactiva el meteorito en el servidor
        DestroyShield(); // Sincroniza la desactivaci�n en los clientes
    }

    // Temporizador para la destrucci�n del escudo
    private IEnumerator StartShieldTimer()
    {
        yield return new WaitForSeconds(duration);
        DestroyShield();
    }

    // Destruye el escudo
    private void DestroyShield()
    {
        if (!owner.IsServer) return; // Solo el servidor controla la destrucci�n

        gameObject.SetActive(false);
        NotifyClients(false);
    }

    // Notificar a los clientes sobre el estado del escudo
    private void NotifyClients(bool isActive)
    {
        var message = new FastBufferWriter(sizeof(bool), Allocator.Temp);
        message.WriteValueSafe(isActive);

        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(
            "UpdateShieldState",
            message
        );
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
            "UpdateShieldState",
            HandleShieldStateUpdate
        );
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("UpdateShieldState");
        }
    }

    private void HandleShieldStateUpdate(ulong senderClientId, FastBufferReader reader)
    {
        bool isActive;
        reader.ReadValueSafe(out isActive);

        gameObject.SetActive(isActive);
    }
}
