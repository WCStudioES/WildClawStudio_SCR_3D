using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Partida : NetworkBehaviour
{
    
    //CONTROLA QUE LA PARTIDA ESTE ACTIVA
    public bool partidaEnMarcha = false;
    
    //CONTROLA QUE LA PARTIDA HAYA FINALIZADO
    public bool partidaFinalizada = true;
    
    //CONTROLA EL NUMERO MÁXIMO DE RONDAS
    [SerializeField] private int maximoNumeroDeRondas = 3;
    
    //CONTROLA EN QUE RONDA ESTA LA PARTIDA
    private int ronda = 1;
    
    //JUGADORES
    [SerializeField] public List<ControladorDelJugador> jugadores;
    
    //RONDAS GANADAS
    [SerializeField] public int[] rondasGanadas;

    //PUNTOS DONDE APARECERAN LOS JUGADORES
    [SerializeField] private GameObject[] puntosDeSpawn;
    
    
    //LOOP DE JUEGO
    void Update()
    {
        //SI NO HAY UNA PARTIDA EN MARCHA Y HAY JUGADORES SUFICIENTES INICIA LA PARTIDA
        if (!partidaEnMarcha && jugadores.Count == 2 && partidaFinalizada)
            iniciarPartida();
        
        if (partidaEnMarcha && !partidaFinalizada)
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
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                jugadores[i].opcionesJugador.desactivarMovimiento();
                if(IsServer)
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
        //RESTAURAMOS NAVES Y METEORITOS
        restaurarNaves();
        restaurarMeteoritos();
        //MOSTRAMOS EL RESULTADO A PARTIR DE LA INTERFAZ
        mostrarResultadoFinal();
        //ELIMINAMOS LAS NAVES DE LA LISTA Y SUS PUNTUACIONES
        partidaEnMarcha = false;
        jugadores.Clear();
        rondasGanadas[0] = 0;
        rondasGanadas[1] = 0;
    }

    //FINALIZA LA RONDA
    void finalizarRonda(int estadoDeLosJugadores)
    {
        //PARAMOS A LOS JUGADORES
        detenerElMovimientoDeLosJugadores();
        
        //APUNTAMOS LA VICTORIA
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                if (i != estadoDeLosJugadores)
                    rondasGanadas[i]++;
            }
        }
        
        //EJECUTAMOS LA RUTINA PARA PASAR DE RONDA, O FINALIZAR LA PARTIDA
        if (ronda < maximoNumeroDeRondas)
        {
            ronda++;
            mostrarResultado(estadoDeLosJugadores);
            partidaEnMarcha = false;
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
            if (jugadores[i] != null)
            {
                if (jugadores[i].hp.Value <= 0)
                    jugadores[i].naveDestruida = true;
                
                if (jugadores[i].naveDestruida)
                {
                    return i;
                }
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
                jugadores[i].opcionesJugador.UIJugador.mostrarResultado(estadoDeLosJugadores == i, false);
            }
        }
    }
    void mostrarResultadoFinal()
    {
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                if (i % 2 == 0)
                {
                    jugadores[i].opcionesJugador.UIJugador.mostrarResultado(rondasGanadas[i] > rondasGanadas[i+1], true);
                }
                else
                {
                    jugadores[i].opcionesJugador.UIJugador.mostrarResultado(rondasGanadas[i] > rondasGanadas[i-1], true); 
                }
            }
        }
    }

    //INICIA LA PARTIDA
    void iniciarPartida()
    {
        //INDICA QUE LA PARTIDA HA EMPEZADO
        partidaEnMarcha = true;
        partidaFinalizada = false;
        
        //PREPARA EL ESCENARIO
        prepararEscenario();
        
        //CARGA LA UI CORRESPONDIENTE
        foreach (var jugador in jugadores)
        {
            if(jugador != null)
                jugador.opcionesJugador.UIJugador.PartidaEncontrada();
        }
        
        //HABILITA EL MOVIMIENTO Y LAS NAVES EN EL SERVIDOR
        foreach (var jugador in jugadores)
        {
            if (jugador != null && IsServer && !IsHost)
            {
                jugador.opcionesJugador.rehabilitarNave();
                jugador.opcionesJugador.reactivarMovimiento();
            }
        }
        
        //HABILITA EL MOVIMIENTO LAS NAVES EN EL CLIENTE
        foreach (var jugador in jugadores)
        {
            if (jugador != null && IsClient)
            {
                jugador.opcionesJugador.rehabilitarNave();
                //jugador.opcionesJugador.reactivarMovimiento();
            }
        }
        
        //LANZA LA CUENTA ATRAS
        foreach (var jugador in jugadores)
        {
            if(jugador != null)
                jugador.opcionesJugador.UIJugador.iniciarCuentaAtras();
        }
    }
    
    //PREPARAR COMPONENTES
    private void prepararEscenario()
    {
        if (IsServer)
        {
            //CURA A LAS NAVES Y LAS PONE EN POSICIÓN
            restaurarNaves();
            //RESTAURA LOS METEORITOS A SU ESTADO INICIAL
            restaurarMeteoritos();
        }
    }

}
