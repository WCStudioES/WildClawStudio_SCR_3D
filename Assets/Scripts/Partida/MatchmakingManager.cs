using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchmakingManager : NetworkBehaviour
{
    public List<UI> colaJugadores = new List<UI>();
    public List<Partida> partidas = new List<Partida>();

    public static List<Partida> ListaDePartidas;

    private void Start()
    {
        ListaDePartidas = partidas;
    }

    public void Update()
    {
        if (colaJugadores.Count >= 2)
        {
            for(int i = 0; i < partidas.Count; i++)
            {
                if (partidas[i] != null)
                {
                    if (partidas[i].partidaEnMarcha == false && partidas[i].partidaFinalizada == true &&
                        partidas[i].jugadores.Count == 0)
                    {
                        IniciarPartida(i);
                        break;
                    }
                }
            }
            
        }
    }

    private void IniciarPartida(int IDPartida)
    {
        // Emparejar a los dos primeros jugadores
        colaJugadores[0].MeterJugadorEnLaPartida(IDPartida);
        colaJugadores[1].MeterJugadorEnLaPartida(IDPartida);

        // Remover los jugadores de la cola
        colaJugadores.RemoveAt(0);
        colaJugadores.RemoveAt(0);
    }
}
