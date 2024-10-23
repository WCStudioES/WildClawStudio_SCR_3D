using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchmakingManager : NetworkBehaviour
{
    private List<NetworkClient> colaJugadores = new List<NetworkClient>();
    public UI uiManager;

    private void Awake()
    {
        // Asegúrate de que el MatchmakingManager solo existe en el servidor
        //if (!IsServer)
        //{
        //    Destroy(this);
        //}
    }

    // Llamado cuando un jugador se une a la cola
    [ServerRpc(RequireOwnership = false)]
    public void UnirseALaColaServerRpc(ServerRpcParams rpcParams = default)
    {
        NetworkClient nuevoJugador = NetworkManager.Singleton.ConnectedClients[rpcParams.Receive.SenderClientId];
        colaJugadores.Add(nuevoJugador);
        Debug.Log("Jugador añadido a la cola: " + nuevoJugador.ClientId);

        // Si hay dos jugadores en la cola, comienza la partida
        if (colaJugadores.Count >= 2)
        {
            IniciarPartida();
        }
    }

    // Función que inicia la partida cuando hay suficientes jugadores
    private void IniciarPartida()
    {
        Debug.Log("Iniciando partida para 2 jugadores");

        // Notificar a los dos primeros jugadores que la partida ha comenzado
        foreach (NetworkClient jugador in colaJugadores.GetRange(0, 2))
        {
            ulong clientId = jugador.ClientId;
            IniciarPartidaClientRpc(clientId);
        }

        // Eliminar a los jugadores de la cola después de iniciar la partida
        colaJugadores.RemoveRange(0, 2);
    }

    // Llamada RPC para notificar al cliente que la partida ha comenzado
    [ClientRpc]
    private void IniciarPartidaClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.Log("Partida encontrada para jugador: " + clientId);
            // Llamar a la función para actualizar la UI en el cliente
            if (uiManager == null)
            {
                uiManager = FindAnyObjectByType<UI>();
            }
            uiManager.PartidaEncontrada();
        }
    }
}
