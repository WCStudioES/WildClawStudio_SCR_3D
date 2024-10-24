using Unity.Netcode;
using UnityEngine;

public class UI : NetworkBehaviour
{
    
    [SerializeField] private ControladorDelJugador CJugador;
    [SerializeField] private OpcionesJugador opcionesJugador;
    
    [SerializeField]private GameObject LogIn;
    [SerializeField]private GameObject Personalizacion;
    [SerializeField]private GameObject BuscandoPartida;
    [SerializeField]private GameObject Instrucciones;
    [SerializeField]private GameObject EnPartida;
    [SerializeField]private GameObject Victoria;
    [SerializeField]private GameObject VictoriaRonda;
    [SerializeField]private GameObject Derrota;
    [SerializeField]private GameObject DerrotaRonda;
    [SerializeField]private GameObject Creditos;
    [SerializeField]private GameObject CuentaAtras;
    [SerializeField]private GameObject[] NumerosCuentaAtras;
    private int posicionCuentaAtras = 0;

    MatchmakingManager matchmakingManager;
    
    void Start()
    {
        matchmakingManager = FindAnyObjectByType<MatchmakingManager>();
        if(IsOwner && opcionesJugador.ActivarUI)
            ActivarUI();
    }

    //TODAS ESTAS FUNCIONES NECESITAN MÁS FUNCIONALIDAD, ESTO ES UN PLACEHOLDER

    public void ActivarUI()
    {
        if(IsOwner)
            LogIn.SetActive(true);
    }

    //RELACIONADAS CON LOGIN////////////////////

    public void IniciarSesion()
    {
        if (IsOwner)
        {
            LogIn.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }
    
    public void CrearNuevoUsuario()
    {
        if (IsOwner)
        {
            LogIn.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }
    
    public void CerrarSesion()
    {
        if (IsOwner)
        {
            Personalizacion.SetActive(false);
            LogIn.SetActive(true);
        }
    }
    
    ////////////////////////////////////////////

    //RELACIONADAS CON BUSCAR PARTIDA///////////
    
    public void BuscarPartida()
    {
        if (IsOwner)
        {
            Personalizacion.SetActive(false);
            BuscandoPartida.SetActive(true);
            //MeterJugadorEnLaPartidaServerRpc();
            UnirseALaColaServerRpc();
        }
    }
    
    public void DejarDeBuscarPartida()
    {
        if (IsOwner)
        {
            BuscandoPartida.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }

    public void PartidaEncontrada()
    {
        if (IsOwner)
        {
            BuscandoPartida.SetActive(false);
            EnPartida.SetActive(true);
        }
    }

    ////////////////////////////////////////////
    
    //RELACIONADAS CON LA PARTIDA///////////////

    public void mostrarResultado(bool ganador, bool partidaFinalizada)
    {
        if (IsOwner)
        {
            if (partidaFinalizada)
            {
                if (ganador)
                {
                    Ganar();
                }
                else
                {
                    Perder();
                }
            }
            else
            {
                if (ganador)
                {
                    GanarRonda();
                }
                else
                {
                    PerderRonda();
                }
            }
        }
    }
    public void Ganar()
    {
        if (IsOwner)
        {
            EnPartida.SetActive(false);
            Victoria.SetActive(true);
            opcionesJugador.movimientoActivado = false;
            CJugador.GameEndServerRpc();
        }
    }
    
    public void GanarRonda()
    {
        if (IsOwner)
        {
            VictoriaRonda.SetActive(true);
            CJugador.GameEndServerRpc();
            Invoke("desactivarResultadoRonda", 3.25f);
        }
    }
    
    public void Perder()
    {
        if (IsOwner)
        {
            EnPartida.SetActive(false);
            Derrota.SetActive(true);
            opcionesJugador.movimientoActivado = false;
            CJugador.GameEndServerRpc();
        }
    }
    
    public void PerderRonda()
    {
        if (IsOwner)
        {
            DerrotaRonda.SetActive(true);
            CJugador.GameEndServerRpc();
            Invoke("desactivarResultadoRonda", 3.25f);
        }
    }

    public void VolverAPersonalizacionDesdePartida()
    {
        if (IsOwner)
        {
            Victoria.SetActive(false);
            Derrota.SetActive(false);
            Personalizacion.SetActive(true);
            opcionesJugador.rehabilitarNave();
        }
    }

    public void desactivarResultadoRonda()
    {
        VictoriaRonda.SetActive(false);
        DerrotaRonda.SetActive(false);
    }

    ////////////////////////////////////////////

    
    
    //RELACIONADAS CON INSTRUCCIONES////////////
    
    public void MirarInstrucciones()
    {
        if (IsOwner)
        {
            Personalizacion.SetActive(false);
            Instrucciones.SetActive(true);
        }
    }
    
    public void VolverAPersonalizacionDesdeInstrucciones()
    {
        if (IsOwner)
        {
            Instrucciones.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }
    
    ////////////////////////////////////////////
    
    //RELACIONADAS CON CREDITOS/////////////////
    
    public void MirarCreditos()
    {
        if (IsOwner)
        {
            Personalizacion.SetActive(false);
            Creditos.SetActive(true);
        }
    }
    
    public void VolverAPersonalizacionDesdeCreditos()
    {
        if (IsOwner)
        {
            Creditos.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }

    ////////////////////////////////////////////

    // El cliente llama a esto para unirse a la cola de matchmaking
    [ServerRpc]
    public void UnirseALaColaServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong jugadorId = rpcParams.Receive.SenderClientId;
        Debug.Log("Jugador añadido a la cola: " + jugadorId);

        matchmakingManager.colaJugadores.Add(opcionesJugador.UIJugador);
    }

    //AÑADIR JUGADOR A PARTIDA
    //TODO (APAÑAR ESTO CON EL MATCHMAKER)
    //[ServerRpc]
    public void MeterJugadorEnLaPartida()
    {
        Debug.Log("Meter jugador en partida (Server)");
        Partida partida = FindObjectOfType<Partida>();
        if (IsServer)
        {
            partida.jugadores.Add(opcionesJugador.controladorDelJugador);
            MeterJugadorEnLaPartidaClientRpc();
        }
    }
    
    [ClientRpc]
    public void MeterJugadorEnLaPartidaClientRpc()
    {
        Debug.Log("Meter jugador en partida (Client)");
        Partida partida = FindObjectOfType<Partida>();
        if (IsClient && !IsHost)
        {
            partida.jugadores.Add(opcionesJugador.controladorDelJugador);
        }
    }
    
    //CUENTA ATRAS///////////////////////////////
    public void iniciarCuentaAtras()
    {
        if (IsOwner)
        {
            CuentaAtras.SetActive(true);
            contadorCuentaAtras();
        }
    }

    private void contadorCuentaAtras()
    {
        if (posicionCuentaAtras < NumerosCuentaAtras.Length)
        {
            if(posicionCuentaAtras != 0)
                NumerosCuentaAtras[posicionCuentaAtras - 1].SetActive(false);
            NumerosCuentaAtras[posicionCuentaAtras].SetActive(true);
            posicionCuentaAtras++;
            Invoke("contadorCuentaAtras", 1.0f);
        }
        else
        {
            NumerosCuentaAtras[posicionCuentaAtras - 1].SetActive(false);
            CuentaAtras.SetActive(false);
            posicionCuentaAtras = 0;
            //opcionesJugador.reactivarMovimiento();
        }
    }


    ////////////////////////////////////////////
}
