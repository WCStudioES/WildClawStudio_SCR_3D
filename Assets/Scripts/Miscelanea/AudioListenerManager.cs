using UnityEngine;
using Unity.Netcode;

public class AudioListenerManager : MonoBehaviour
{
    private void Awake()
    {
        // Verifica si este es el jugador local
        if (!IsLocalPlayer())
        {
            // Si no es el jugador local, desactiva el AudioListener
            var audioListener = GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }
    }

    private bool IsLocalPlayer()
    {
        // Asume que tu objeto jugador tiene NetworkObject
        var networkObject = GetComponentInParent<NetworkObject>();
        return networkObject != null && networkObject.IsOwner;
    }
}
