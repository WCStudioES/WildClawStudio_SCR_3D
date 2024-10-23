using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Partida : NetworkBehaviour
{
    
    //CONTROLA QUE LA PARTIDA ESTE ACTIVA
    public bool partidaEnMarcha = false;
    
    //CONTROLA EL NUMERO MÁXIMO DE RONDAS
    [SerializeField] private int maximoNumeroDeRondas = 3;
    
    //CONTROLA EN QUE RONDA ESTA LA PARTIDA
    private int ronda = 1;
    
    //JUGADORES
    [SerializeField] public List<ControladorDelJugador> jugadores;

    //PUNTOS DONDE APARECERAN LOS JUGADORES
    [SerializeField] private GameObject[] puntosDeSpawn;
    
    
    //LOOP DE JUEGO
    void Update()
    {
        if (!partidaEnMarcha && jugadores.Count == 2)
            iniciarPartida();
        
        if (partidaEnMarcha)
        {
            int estadoDeLosJugadores = comprobarEstadoDeLosJugadores();
            if (estadoDeLosJugadores != -1)
            {
                finalizarRonda(estadoDeLosJugadores);
            }
        }
    }

    //RESTAURA LAS POSICIONES DE LAS NAVES, SU NIVEL Y SU VIDA
    void restaurarNaves()
    {
        //DESACTIVA EL MOVIMIENTO DE LAS NAVES Y LAS DEVUELVE AL ORIGEN DE COORDENADAS, RESTAURA LA VIDA
        if(IsServer)
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                jugadores[i].opcionesJugador.desactivarMovimiento();
                jugadores[i].restaurarNaves(puntosDeSpawn[i]);
            }
        }
    }

    //RESTAURA LA VIDA DE LOS METEORITOS Y LOS REHABILITA
    void restaurarMeteoritos()
    {
        //TODO
    }

    //FINALIZA LA PARTIDA
    void finalizarPartida()
    {
        //TODO
    }

    //FINALIZA LA RONDA
    void finalizarRonda(int estadoDeLosJugadores)
    {
        //PARAMOS A LOS JUGADORES
        detenerElMovimientoDeLosJugadores();
        //MOSTRAMOS EL RESULTADO A PARTIR DE LA INTERFAZ
        mostrarResultado(estadoDeLosJugadores);
        
        //EJECUTAMOS LA RUTINA PARA PASAR DE RONDA, O FINALIZAR LA PARTIDA
        if (ronda < maximoNumeroDeRondas)
        {
            ronda++;
            restaurarNaves();
            restaurarMeteoritos();
        }
        else
        {
            finalizarPartida();
        }
    }

    //COMPRUEBA EL ESTADO DE LOS JUGADORES
    int comprobarEstadoDeLosJugadores()
    {
        //INDICA SI UNO DE LOS JUGADORES HA SIDO DESTRUIDO
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null && jugadores[i].naveDestruida)
            {
                return i;
            }
        }

        return -1;
    }
    
    //DETIENE EL MOVIMIENTO DE LOS JUGADORES
    void detenerElMovimientoDeLosJugadores()
    {
        foreach (var jugador in jugadores)
        {
            jugador.GameEndServerRpc();
        }
    }

    //ACTIVA LAS PANTALLAS DE VICTORIA O DERROTA EN LOS JUGADORES
    //TODO (HAY QUE HACER VERSIÓN DISTINTA PARA GANAR Y PERDER RONDAS)
    void mostrarResultado(int estadoDeLosJugadores)
    {
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                jugadores[i].opcionesJugador.UIJugador.mostrarResultado(estadoDeLosJugadores == i);
            }
        }
    }

    //INICIA LA PARTIDA
    void iniciarPartida()
    {
        restaurarNaves();
        foreach (var jugador in jugadores)
        {
            if(jugador != null)
                jugador.opcionesJugador.UIJugador.PartidaEncontrada();
        }
        
        //TODO (METER AQUI UNA CUENTA ATRAS O ALGO CUANDO INICIE LA PARTIDA, TAMBIÉN ENTRE RONDAS)
        
        partidaEnMarcha = true;
        
        foreach (var jugador in jugadores)
        {
            if(jugador != null)
                jugador.opcionesJugador.reactivarMovimiento();
        }
 
    }

}
