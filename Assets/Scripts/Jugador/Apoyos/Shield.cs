using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
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
        else
        {
            ChangeMaterialColorClientRpc(Color.cyan, 0.1f);
        }

    }

    [ClientRpc]
    private void ChangeMaterialColorClientRpc(Color hitColor, float duration)
    {
        StartCoroutine(FlashMaterialsInChildren(hitColor, duration));
    }

    private IEnumerator FlashMaterialsInChildren(Color hitColor, float duration)
    {
        // Busca todos los Renderers (MeshRenderer o SkinnedMeshRenderer) en los hijos
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) yield break; // Salir si no hay Renderers

        // Almacena los colores originales de todos los materiales
        var originalColors = new Dictionary<Material, Color>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (!originalColors.ContainsKey(material))
                {
                    // Guardar el color original
                    originalColors[material] = material.color;

                    // Si se preserva el alpha, ajustamos solo RGB
                    hitColor.a = material.color.a;

                    // Cambiar el color al de impacto
                    material.color = hitColor;
                }
            }
        }

        yield return new WaitForSeconds(duration); // Esperar el tiempo especificado

        // Restaurar los colores originales
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (originalColors.ContainsKey(material))
                {
                    material.color = originalColors[material];
                }
            }
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
