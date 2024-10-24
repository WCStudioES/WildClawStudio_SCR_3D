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
    public int ronda = 1;
    
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
            if (comprobarEstadoDeLosJugadores())
            {
                partidaEnMarcha = false;
                finalizarRonda();
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
                jugadores[i].naveDestruida = false;
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
        prepararEscenario();
        //MOSTRAMOS EL RESULTADO A PARTIR DE LA INTERFAZ
        mostrarResultadoFinal();
        //DEVOLVEMOS LAS NAVES A SU ESTADO ORIGINAL
        foreach (var jugador in jugadores)
        {
            if(jugador != null)
                jugador.opcionesJugador.resetToInitialState();
        }
        //ELIMINAMOS LAS NAVES DE LA LISTA Y SUS PUNTUACIONES
        partidaEnMarcha = false;
        partidaFinalizada = true;
        jugadores.Clear();
        rondasGanadas[0] = 0;
        rondasGanadas[1] = 0;
        ronda = 1;
    }

    //FINALIZA LA RONDA
    void finalizarRonda()
    {
        //APUNTAMOS LA VICTORIA
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                if (!jugadores[i].naveDestruida)
                    rondasGanadas[i]++;
            }
        }
        
        //EJECUTAMOS LA RUTINA PARA PASAR DE RONDA, O FINALIZAR LA PARTIDA
        if (ronda < maximoNumeroDeRondas)
        {
            ronda++;
            mostrarResultado();
            Invoke("iniciarRonda", 3.0f);
        }
        else
        {
            mostrarResultado();
            Invoke("finalizarPartida", 3.0f);
        }
    }
    
    

    //COMPRUEBA EL ESTADO DE LOS JUGADORES
    bool comprobarEstadoDeLosJugadores()
    {
        //INDICA SI UNO DE LOS JUGADORES HA SIDO DESTRUIDO
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                if (jugadores[i].hp.Value <= 0)
                {
                    jugadores[i].naveDestruida = true;
                    jugadores[i].opcionesJugador.deshabilitarNave();
                }
                
                if (jugadores[i].naveDestruida)
                {
                    return true;
                }
            }
        }

        return false;
    }
    

    //ACTIVA LAS PANTALLAS DE VICTORIA O DERROTA EN LOS JUGADORES
    //TODO (HAY QUE HACER VERSIÓN DISTINTA PARA GANAR Y PERDER RONDAS)
    void mostrarResultado()
    {
        for (int i = 0; i < jugadores.Count;i++)
        {
            if (jugadores[i] != null)
            {
                jugadores[i].opcionesJugador.UIJugador.mostrarResultado(!jugadores[i].naveDestruida, false);
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
        iniciarRonda();
        
        //CARGA LA UI CORRESPONDIENTE
        foreach (var jugador in jugadores)
        {
            if(jugador != null)
                jugador.opcionesJugador.UIJugador.PartidaEncontrada();
        }

        partidaFinalizada = false;
    }
    
    //EMPEZAR SIGUIENTE RONDA
    void iniciarRonda()
    {
        //PREPARA EL ESCENARIO
        prepararEscenario();
        
        //HABILITA EL MOVIMIENTO Y LAS NAVES
        Invoke("reactivarNaves", 3.0f);
        
        //LANZA LA CUENTA ATRAS PARA INICIAR LA PARTIDA
        foreach (var jugador in jugadores)
        {
            if (jugador != null)
            {
                jugador.opcionesJugador.UIJugador.iniciarCuentaAtras();
                jugador.opcionesJugador.rehabilitarNave();
            }

        }
        
        //INICIAMOS LA RONDA
        Invoke("ponerPartidaEnMarcha", 3.0f);
    }
    
    //PREPARAR COMPONENTES
    private void prepararEscenario()
    {
        //CURA A LAS NAVES Y LAS PONE EN POSICIÓN
        Invoke("restaurarNaves", 0.2f);
        //RESTAURA LOS METEORITOS A SU ESTADO INICIAL
        restaurarMeteoritos();
    }
    

    //REACTIVA EL MOVIMIENTO DE LAS NAVES
    private void reactivarNaves()
    {
        foreach (var jugador in jugadores)
        {
            if (jugador != null)
            {
                jugador.opcionesJugador.reactivarMovimiento();
            }
        }
    }

    private void ponerPartidaEnMarcha()
    {
        //INDICA QUE LA PARTIDA HA EMPEZADO
        partidaEnMarcha = true;
    }

}
