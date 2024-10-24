using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchmakingManager : NetworkBehaviour
{
    public List<UI> colaJugadores = new List<UI>();
    public List<Partida> partidas = new List<Partida>();

    public void Update()
    {
        if (colaJugadores.Count >= 2)
        {
            IniciarPartida();
        }
    }

    private void IniciarPartida()
    {
        Debug.Log("Iniciando partida para dos jugadores...");
        Debug.Log("Iniciando partida para dos jugadores...");

        // Emparejar a los dos primeros jugadores
        colaJugadores[0].MeterJugadorEnLaPartida();
        colaJugadores[1].MeterJugadorEnLaPartida();

        // Remover los jugadores de la cola
        colaJugadores.RemoveAt(0);
        colaJugadores.RemoveAt(0);
    }
}
