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
    [SerializeField]private GameObject Derrota;
    [SerializeField]private GameObject Creditos;
    
    
    void Start()
    {
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
            MeterJugadorEnLaPartidaServerRpc();
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

    public void mostrarResultado(bool ganador)
    {
        if (IsOwner)
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

    public void VolverAPersonalizacionDesdePartida()
    {
        if (IsOwner)
        {
            Victoria.SetActive(false);
            Derrota.SetActive(false);
            Personalizacion.SetActive(true);
        }
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
    
    
    //AÑADIR JUGADOR A PARTIDA
    //TODO (APAÑAR ESTO CON EL MATCHMAKER)
    [ServerRpc]
    public void MeterJugadorEnLaPartidaServerRpc()
    {
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
        Partida partida = FindObjectOfType<Partida>();
        if (IsClient && !IsHost)
        {
            partida.jugadores.Add(opcionesJugador.controladorDelJugador);
        }
    }
}
