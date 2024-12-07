using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Partida : NetworkBehaviour
{
    
    //CONTROLA QUE LA PARTIDA ESTE ACTIVA
    public bool partidaEnMarcha = false;
    
    //CONTROLA QUE LA PARTIDA HAYA FINALIZADO
    public bool partidaFinalizada = true;
    
    //CONTROLA QUE LA RONDA HAYA FINALIZADO
    public bool rondaEnmarcha;
    //CONTROLA EL NUMERO MÁXIMO DE RONDAS
    [SerializeField] private int maximoNumeroDeRondas = 3;
    
    //CONTROLA EN QUE RONDA ESTA LA PARTIDA
    public int ronda = 1;
    
    //CONTROLA EL TIEMPO MÁXIMO DE CADA RONDA
    [SerializeField] private float maximoTiempoPorRonda = 60;

    //CONTROLA EL TIEMPO RESTANTE DE LA RONDA ACTUAL
    public NetworkVariable<float> tiempoDeRondaActual = new NetworkVariable<float>(60);
    
    //JUGADORES
    [SerializeField] public List<NetworkedPlayer> jugadores;
    
    //METEORITOS
    public GameObject EmptyContenedorDeMeteoritos; //Empty contenedor de los meteoritos en la escena
    public Meteorito[] meteoritos; //Lista con los meteoritos
    
    //Debris
    public GameObject EmptyContenedorDeDebris; //Empty contenedor de los meteoritos en la escena
    [SerializeField] public Debris[] debris; //Lista con los meteoritos
    
    //RONDAS GANADAS
    [SerializeField] public int[] rondasGanadas;

    //PUNTOS DONDE APARECERAN LOS JUGADORES
    [SerializeField] private GameObject[] puntosDeSpawn;

    //ANILLO DE FUEGO
    [SerializeField] private FireRing fireRing;

    //LISTA DE RONDAS GANADAS
    private List<List<bool>> partidasGanadasPorJugador = new List<List<bool>>();

    private bool hasntAnnouncedFireRing;
    
    //USUARIOS
    private List<Usuario> _usuarios = new List<Usuario>();

    
    //LOOP DE JUEGO
    void Update()
    {
        //SI NO HAY UNA PARTIDA EN MARCHA Y HAY JUGADORES SUFICIENTES INICIA LA PARTIDA
        if (!partidaEnMarcha && jugadores.Count == 2 && partidaFinalizada)
            iniciarPartida();
        
        //SI LA PARTIDA ESTA EN MARCHA
        if (partidaEnMarcha && !partidaFinalizada)
        { 
            jugadores.RemoveAll( x => x == null);
            if (jugadores.Count == 2)
            {
                if (comprobarEstadoDeLosJugadores())
                {
                    partidaEnMarcha = false;
                    finalizarRonda();
                }

                if (tiempoDeRondaActual.Value <= 0.0f)
                {
                    //FINAL POR MUERTE SÚBITA
                    fireRing.CrearAreaDmg(null, null, IsServer);
                    OcultarDestructibles();
                    fireRing.isShrinking = true;

                    if (hasntAnnouncedFireRing)
                    {
                        foreach (NetworkedPlayer jugador in jugadores)
                        {
                            jugador.MensajeAnuncioClientRpc("FINAL SHOWDOWN!");
                        }
                        hasntAnnouncedFireRing = false;
                    }
                    

                    //FINAL POR TIEMPO
                    //partidaEnMarcha = false;
                    //finalizarRondaPorTiempo();
                }

                if(IsServer)
                    tiempoDeRondaActual.Value -= Time.deltaTime;
                foreach (var jugador in jugadores)
                {
                    jugador.Cronometro.fillAmount = Mathf.Max(tiempoDeRondaActual.Value/maximoTiempoPorRonda, 0);
                }
            }
            else
            {
                finalizarPartidaPorDesconexion();
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
                    jugadores[i].resetDeRonda(puntosDeSpawn[i]);
                jugadores[i].naveDestruida = false;
            }
        }
    }

    //RESTAURA LA VIDA DE LOS METEORITOS Y LOS REHABILITA
    void restaurarDestructibles()
    {
        foreach (var meteorito in meteoritos)
        {
            meteorito.RestoreDestructibleAsset();
            switch (ronda)
            {
                case 1: 
                    meteorito.RestoreDestructibleAsset(50);
                    break;
                case 2:
                    meteorito.RestoreDestructibleAsset(75);
                    break;
                case 3:
                    meteorito.RestoreDestructibleAsset(100);
                    break;
                
                default:
                    Debug.Log("AVISO: NUMERO DE RONDA NO ES 1, 2 O 3");
                    break;
                    
            }
        }

        foreach (var unDebris in debris)
        {
            unDebris.RestoreDestructibleAsset();
        }
    }

    private void OcultarDestructibles()
    {
        foreach (var meteorito in meteoritos)
        {
            meteorito.DisableDamageableClientRpc();
        }

        foreach (var unDebris in debris)
        {
            unDebris.DisableDamageableClientRpc();
        }
    }

    //FINALIZA LA PARTIDA
    void finalizarPartida()
    {
        //RESTAURAMOS NAVES Y METEORITOS
        prepararEscenario();
        //DEVOLVEMOS LAS NAVES A SU ESTADO ORIGINAL
        foreach (var jugador in jugadores)
        {
            if(jugador != null)
                jugador.opcionesJugador.resetToInitialState();
        }
        //MOSTRAMOS EL RESULTADO A PARTIR DE LA INTERFAZ
        mostrarResultadoFinal();
        //ELIMINAMOS LAS NAVES DE LA LISTA Y SUS PUNTUACIONES
        partidaEnMarcha = false;
        partidaFinalizada = true;
        jugadores.Clear();
        rondasGanadas[0] = 0;
        rondasGanadas[1] = 0;
        ronda = 1;
        partidasGanadasPorJugador.Clear();
    }

    //FINALIZA LA RONDA
    void finalizarRonda()
    {
        rondaEnmarcha = false;
        //APUNTAMOS LA VICTORIA
        for (int i = 0; i < jugadores.Count; i++)
        {
            if (jugadores[i] != null)
            {
                if (!jugadores[i].naveDestruida)
                {
                    rondasGanadas[i]++;
                    partidasGanadasPorJugador[i].Add(true);
                }
                else
                {
                    partidasGanadasPorJugador[i].Add(false);
                }
                //jugadores[i].GetComponent<PlayerShip>().ResetRonda();
            }
        }

        //SI UN JUGADOR HA GANADO MÁS DE LA MITAD DE LAS PARTIDAS, SE FINALIZA LA PARTIDA
        if (rondasGanadas[0] > maximoNumeroDeRondas / 2 || rondasGanadas[1] > maximoNumeroDeRondas / 2)
        {
            mostrarResultado();
            Invoke("finalizarPartida", 3.0f);
        }
        else
        {
            //EJECUTAMOS LA RUTINA PARA PASAR DE RONDA, O FINALIZAR LA PARTIDA
            if (ronda < maximoNumeroDeRondas)
            {
                mostrarResultado();
                Invoke("iniciarRonda", 3.0f);
                ronda++;
            }
            else
            {
                mostrarResultado();
                Invoke("finalizarPartida", 3.0f);
            }
        }
    }
    
    //FINALIZA LA RONDA POR TIEMPO
    void finalizarRondaPorTiempo()
    {
        //APUNTAMOS LA VICTORIA
        if (jugadores[0] != null && jugadores[1] != null)
        {
            if (jugadores[0].actualHealth.Value / jugadores[0].maxHealth.Value > jugadores[1].actualHealth.Value / jugadores[1].maxHealth.Value)
            {
                rondasGanadas[0]++;
                jugadores[1].naveDestruida = true;
                partidasGanadasPorJugador[0].Add(true);
                partidasGanadasPorJugador[1].Add(false);
            }
            else if (jugadores[0].actualHealth.Value / jugadores[0].maxHealth.Value < jugadores[1].actualHealth.Value / jugadores[1].maxHealth.Value)
            {
                rondasGanadas[1]++;
                jugadores[0].naveDestruida = true;
                partidasGanadasPorJugador[1].Add(true);
                partidasGanadasPorJugador[0].Add(false);
            }
            else
            {
                rondasGanadas[0]++;
                jugadores[1].naveDestruida = true;
                partidasGanadasPorJugador[0].Add(true);
                partidasGanadasPorJugador[1].Add(false);
            }
            
            jugadores[0].opcionesJugador.deshabilitarNave();
            jugadores[1].opcionesJugador.deshabilitarNave();
        }
        
        //SI UN JUGADOR HA GANADO MÁS DE LA MITAD DE LAS PARTIDAS, SE FINALIZA LA PARTIDA
        if (rondasGanadas[0] > maximoNumeroDeRondas / 2 || rondasGanadas[1] > maximoNumeroDeRondas / 2)
        {
            mostrarResultado();
            Invoke("finalizarPartida", 3.0f);
        }
        else
        {
            //EJECUTAMOS LA RUTINA PARA PASAR DE RONDA, O FINALIZAR LA PARTIDA
            if (ronda < maximoNumeroDeRondas)
            {
                mostrarResultado();
                Invoke("iniciarRonda", 3.0f);
                ronda++;
            }
            else
            {
                mostrarResultado();
                Invoke("finalizarPartida", 3.0f);
            }
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
                if (jugadores[i].actualHealth.Value <= 0)
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
                jugadores[i].opcionesJugador.UIJugador.mostrarResultado(!jugadores[i].naveDestruida, false, partidasGanadasPorJugador[i], ronda);
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
                    jugadores[i].opcionesJugador.UIJugador.mostrarResultado(rondasGanadas[i] > rondasGanadas[i+1], true, partidasGanadasPorJugador[0], ronda);
                }
                else
                {
                    jugadores[i].opcionesJugador.UIJugador.mostrarResultado(rondasGanadas[i] > rondasGanadas[i-1], true, partidasGanadasPorJugador[1], ronda); 
                }
            }
        }
        //GUARDAR EN EL HISTORIAL
        jugadores[0].opcionesJugador.UIJugador.guardarPartidaEnHistorial(jugadores[1].opcionesJugador.usuario.name, jugadores[1].nave.playerShip.name, jugadores[0].nave.playerShip.name, rondasGanadas[0], rondasGanadas[1], rondasGanadas[0] > rondasGanadas[1]);
        jugadores[1].opcionesJugador.UIJugador.guardarPartidaEnHistorial(jugadores[0].opcionesJugador.usuario.name, jugadores[0].nave.playerShip.name, jugadores[1].nave.playerShip.name, rondasGanadas[1], rondasGanadas[0], rondasGanadas[1] > rondasGanadas[0]);
    }

    //INICIA LA PARTIDA
    void iniciarPartida()
    {
        foreach (var jugador in jugadores)
        {
            if (jugador != null)
            {
                //QUITAR MÚSICA
                jugador.opcionesJugador.UIJugador.pararMusica(); 

                //ASIGNA SU NOMBRE A LA UI
                jugador.NombreEnemigo.text = jugador.opcionesJugador.usuario.name;
                
                //ASIGNA EL USUARIO A LA LISTA
                Usuario aux = new Usuario(jugador.opcionesJugador.usuario.name, jugador.opcionesJugador.usuario.password);
                aux.historial = jugador.opcionesJugador.usuario.historial;
                _usuarios.Add(aux);
            }

        }

        foreach (var VARIABLE in jugadores)
        {
            partidasGanadasPorJugador.Add(new List<bool>(maximoNumeroDeRondas));
        }
        
        //Obtener todos los meteoritos del propio mapa
        meteoritos = EmptyContenedorDeMeteoritos.GetComponentsInChildren<Meteorito>();

        foreach (var meteorito in meteoritos)
        {
            meteorito.partida = this;
        }
        
        //Obtener todos los meteoritos del propio mapa
        debris = EmptyContenedorDeDebris.GetComponentsInChildren<Debris>();
        
        foreach (var resto in debris)
        {
            resto.partida = this;
        }
        
        //Debug.Log(meteoritos.Length);
        
        iniciarRonda();
        
        //CARGA LA UI CORRESPONDIENTE Y REINICIA A LOS JUGADORES
        foreach (var jugador in jugadores)
        {
            if (jugador != null)
            {
                jugador.opcionesJugador.UIJugador.PartidaEncontrada();
                jugador.ResetPrePartida();
                jugador.partida = this;
            }
        }

        partidaFinalizada = false;
    }
    
    //EMPEZAR SIGUIENTE RONDA
    void iniciarRonda()
    {
        //PREPARA EL ESCENARIO
        prepararEscenario();
        
        //HABILITA EL MOVIMIENTO Y LAS NAVES
        //HABILITA EL MOVIMIENTO Y LAS NAVES
        Invoke("reactivarNaves", 3.0f);
        hasntAnnouncedFireRing = true;
        
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
        fireRing.Reset();
        //RESTAURA EL TIEMPO DE LA RONDA
        if(IsServer)
            tiempoDeRondaActual.Value = maximoTiempoPorRonda;
        foreach (var jugador in jugadores)
        {
            jugador.Cronometro.fillAmount = Mathf.Max(tiempoDeRondaActual.Value/maximoTiempoPorRonda, 0);
            fireRing.AddPlayer(jugador.nave.transform);
        }
        //CURA A LAS NAVES Y LAS PONE EN POSICIÓN
        Invoke("restaurarNaves", 0.2f);
        //RESTAURA LOS METEORITOS A SU ESTADO INICIAL
        restaurarDestructibles();
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
        foreach (var jugador in jugadores)
        {
            if (jugador != null)
            {
                jugador.opcionesJugador.UIJugador.musicaInGame();
            }

        }
        partidaEnMarcha = true;
        rondaEnmarcha = true;
    }

    //FINALIZA LA PARTIDA POR DESCONEXION
    void finalizarPartidaPorDesconexion()
    {
        //RESTAURAMOS NAVES Y METEORITOS
        prepararEscenario();
        //MOSTRAMOS EL RESULTADO A PARTIR DE LA INTERFAZ
        //mostrarResultadoFinal();
        //DEVOLVEMOS LAS NAVES A SU ESTADO ORIGINAL
        foreach (var jugador in jugadores)
        {
            if (jugador != null)
            {
                jugador.opcionesJugador.resetToInitialState();
                jugador.opcionesJugador.UIJugador.VolverAPersonalizacionDesdePartida();
                jugador.opcionesJugador.UIJugador.pararMusica(); 
                jugador.opcionesJugador.UIJugador.musicaMenu();
                //GUARDAMOS EN EL HISTORIAL EL RESULTADO
                Usuario rival = new Usuario("rival", "rival");
                foreach (var usuario in _usuarios)
                {
                    if (usuario.name != jugador.opcionesJugador.usuario.name)
                        rival = usuario;
                }

                foreach (var usuario in _usuarios)
                {
                    if (usuario.name == jugador.opcionesJugador.usuario.name)
                    {
                        jugador.opcionesJugador.UIJugador.guardarPartidaEnHistorial(rival.name, "", jugador.nave.playerShip.name, 3, 0, true);
                    }
                    else
                    {
                        if (IsServer)
                        {
                            rival.guardarPartidaEnHistorial(jugador.opcionesJugador.usuario.name, jugador.nave.playerShip.name, "", 0, 3, false);
                            Usuario.GuardarUsuario(rival);
                        }
                    }
                }
            }
        }
        //ELIMINAMOS LAS NAVES DE LA LISTA Y SUS PUNTUACIONES
        partidaEnMarcha = false;
        partidaFinalizada = true;
        jugadores.Clear();
        rondasGanadas[0] = 0;
        rondasGanadas[1] = 0;
        ronda = 1;
        partidasGanadasPorJugador.Clear();
    }

}
