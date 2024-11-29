using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI : NetworkBehaviour
{
    
    [SerializeField] private NetworkedPlayer CJugador;
    [SerializeField] private OpcionesJugador opcionesJugador;
    [SerializeField] private CustomizationManager customizationManager;
    
    [SerializeField]private GameObject LogIn;
    [SerializeField]private GameObject Personalizacion;
    private bool cameraInGame = false;
    [SerializeField]private GameObject BuscandoPartida;
    [SerializeField]private GameObject Instrucciones;
    [SerializeField]private GameObject Instrucciones2;
    [SerializeField]private GameObject Build;
    [SerializeField]private GameObject EnPartida;
    [SerializeField]private GameObject Victoria;
    [SerializeField]private GameObject VictoriaRonda;
    [SerializeField]private GameObject Derrota;
    [SerializeField]private GameObject DerrotaRonda;
    [SerializeField]private GameObject Creditos;
    [SerializeField]private GameObject Historial;
    [SerializeField]private GameObject CuentaAtras;
    [SerializeField]private GameObject[] NumerosCuentaAtras;
    [SerializeField] private GameObject ToggleAudioButton;
    private int posicionCuentaAtras = 0;

    [SerializeField] private GameObject[] circulosAzulesVictoria;
    [SerializeField] private GameObject[] circulosRojosVictoria;
    
    [SerializeField] private GameObject[] circulosAzulesDerrota;
    [SerializeField] private GameObject[] circulosRojosDerrota;
    
    
    [SerializeField]private LogIn ScriptLogIn;
    [SerializeField]private Register ScriptRegister;
    
    [SerializeField] private AudioClip pitidoAgudo;
    [SerializeField] private AudioClip pitidoGrave;
    
    MatchmakingManager matchmakingManager;
    
    void Start()
    {
        matchmakingManager = FindAnyObjectByType<MatchmakingManager>();
        if(!IsHost && IsOwner && opcionesJugador.ActivarUI)
            ActivarUI();
    }

    private void Update()
    {
        if (IsOwner && !IsHost)
        {
            //SE ENCARGA DE COLOCAR LA CAMARA BIEN CADA VEZ QUE ENTRAS O SALES DE LA PAGINA DE PERSONALIZACIÓN
            if(Personalizacion.activeSelf && cameraInGame)
            {
                CJugador.nave.AssignMainCamera(ControladorNave.CameraType.Customization);
                cameraInGame = false;
            }
            else if(!Personalizacion.activeSelf && !cameraInGame)
            {
                CJugador.nave.AssignMainCamera(ControladorNave.CameraType.InGame);
                cameraInGame = true;
            }
        }
    }

    //TODAS ESTAS FUNCIONES NECESITAN MÁS FUNCIONALIDAD, ESTO ES UN PLACEHOLDER

    public void ActivarUI()
    {
        if (IsOwner)
        {
            LogIn.SetActive(true);
            AudioManager.Instance.PlayMenuMusic();
        }
        
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
    
    
    //COMPRUEBA SI EXISTE UN USUARIO Y SI ES VALIDO
    [ServerRpc]
    public void ComprobarUsuarioServerRpc(string name, string password)
    {
        if (Usuario.ComprobarSiUsuarioExiste(name))
        {
            if (Usuario.ComprobarSiUsuarioEsValido(name, password))
            {
                UsuarioValidoClientRpc(Usuario.SerializeUsuario(Usuario.CargarUsuario(name)));
            }
            else
            {
                UsuarioInvalidoClientRpc();
            }
        }
        else
        {
            UsuarioInvalidoClientRpc();
        }
    }

    //RESPUESTA SI EL USUARIO ES VALIDO(LOG IN)
    [ClientRpc]
    void UsuarioValidoClientRpc(string usuario)
    {
        ScriptLogIn.UsuarioValido(usuario);
    }
    
    //RESPUESTA SI EL USUARIO NO ES VALIDO(LOG IN)
    [ClientRpc]
    void UsuarioInvalidoClientRpc()
    {
        ScriptLogIn.UsuarioInvalido();
    }
    
    //COMPRUEBA SI SE PUEDE REGISTRAR AL NUEVO USUARIO
    [ServerRpc]
    public void CrearUsuarioServerRpc(string name, string password)
    {
        if (Usuario.ComprobarSiUsuarioExiste(name))
        {
            UsuarioExistenteClientRpc();
        }
        else
        {
            Usuario u = new Usuario(name, password);
            Usuario.GuardarUsuario(u);
            UsuarioCreadoClientRpc(Usuario.SerializeUsuario(u));
        }
    }
    
    //RESPUESTA SI EL USUARIO YA EXISTE(REGISTRO)
    [ClientRpc]
    void UsuarioExistenteClientRpc()
    {
        ScriptRegister.UsuarioInvalido();
    }
    
    //RESPUESTA SI E¡SE HA CREADO UN NUEVO USUARIO(REGISTRO)
    [ClientRpc]
    void UsuarioCreadoClientRpc(string usuario)
    {
        ScriptRegister.UsuarioValido(usuario);
    }

    public void AbrirHistorial()
    {
        Personalizacion.SetActive(false);
        Historial.SetActive(true);
    }

    public void CerrarHistorial()
    {
        Historial.SetActive(false);
        Personalizacion.SetActive(true);
    }

    public void ToggleAudio()
    {
        Debug.Log("Hola");
        ToggleAudioButton.GetComponent<AudioToggleButton>().OnButtonPressed();
    }

    //GUARDA UNA PARTIDA EN EL HISTORIAL DE PARTIDAD
    public void guardarPartidaEnHistorial(string rivalS, string naveRivalS, string navePropiaS, int pPropia,
        int pRival, bool ganada)
    {
        if (IsOwner)
        {
            opcionesJugador.usuario.guardarPartidaEnHistorial(rivalS, naveRivalS, navePropiaS,
                pPropia, pRival, ganada);
            guardarPartidaEnHistorialServerRpc(rivalS, naveRivalS, navePropiaS,
                pPropia, pRival, ganada);
        }
    }

    [ServerRpc]
    void guardarPartidaEnHistorialServerRpc(string rivalS, string naveRivalS, string navePropiaS, int pPropia,
        int pRival, bool ganada)
    {
        opcionesJugador.usuario.guardarPartidaEnHistorial(rivalS, naveRivalS, navePropiaS,
            pPropia, pRival, ganada);
        Usuario.GuardarUsuario(opcionesJugador.usuario);
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
            if(!opcionesJugador.testing)
                RetirarseDeLaColaServerRpc();
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

    public void mostrarResultado(bool ganador, bool partidaFinalizada, List<bool> rondasGanadas, int rondas)
    {
        if (IsOwner)
        {
            if (partidaFinalizada)
            {
                AudioManager.Instance.PlayMenuMusic();
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
                AudioManager.Instance.StopMusic();
                Debug.Log("Entra en mostrar resultados");
                if (ganador)
                {
                    GanarRonda(rondasGanadas, rondas);
                }
                else
                {
                    PerderRonda(rondasGanadas, rondas);
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
    
    public void GanarRonda(List<bool> rondasGanadas, int rondas)
    {
        if (IsOwner)
        {
            for (int i = 0; i < circulosAzulesDerrota.Length; i++)
            {
                circulosAzulesVictoria[i].SetActive(false);
                circulosRojosVictoria[i].SetActive(false);
                circulosAzulesDerrota[i].SetActive(false);
                circulosRojosDerrota[i].SetActive(false);
            }
            for (int i = 0; i < rondas; i++)
            {
                if(rondasGanadas[i])
                    circulosAzulesVictoria[i].SetActive(true);
                else
                    circulosRojosVictoria[i].SetActive(true);
            }
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
    
    public void PerderRonda(List<bool> rondasGanadas, int rondas)
    {
        if (IsOwner)
        {
            for (int i = 0; i < circulosAzulesDerrota.Length; i++)
            {
                circulosAzulesVictoria[i].SetActive(false);
                circulosRojosVictoria[i].SetActive(false);
                circulosAzulesDerrota[i].SetActive(false);
                circulosRojosDerrota[i].SetActive(false);
            }
            for (int i = 0; i < rondas; i++)
            {
                if(rondasGanadas[i])
                    circulosAzulesDerrota[i].SetActive(true);
                else
                    circulosRojosDerrota[i].SetActive(true);
            }
            DerrotaRonda.SetActive(true);
            CJugador.GameEndServerRpc();
            Invoke("desactivarResultadoRonda", 3.25f);
        }
    }

    public void VolverAPersonalizacionDesdePartida()
    {
        if (IsOwner)
        {
            EnPartida.SetActive(false);
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
            Instrucciones2.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }
    
    public void CambiarAInstrucciones2()
        {
            if (IsOwner)
            {
                Instrucciones.SetActive(false);
                Instrucciones2.SetActive(true);
            }
        }
    
    
    ////////////////////////////////////////////
    //RELACIONADAS CON CREDITOS/////////////////
    
    public void MirarCreditos()
    {
        if (IsOwner)
        {
            AudioManager.Instance.StopMusic();
            Personalizacion.SetActive(false);
            Creditos.SetActive(true);
        }
    }
    
    public void VolverAPersonalizacionDesdeCreditos()
    {
        if (IsOwner)
        {
            AudioManager.Instance.PlayMenuMusic();
            Creditos.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }

    ////////////////////////////////////////////

    // RELACIONADAS CON LA PERSONALIZACIÓN ////

    public void AbrirPopUpBuild()
    {
        if (IsOwner)
        {
            Build.SetActive(true);
        }
    }

    public void CerrarPopUpBuild()
    {
        if (IsOwner)
        {
            Build.SetActive(false);
        }
    }

    // El cliente llama a esto para unirse a la cola de matchmaking
    [ServerRpc]
    public void UnirseALaColaServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong jugadorId = rpcParams.Receive.SenderClientId;
        Debug.Log("Jugador añadido a la cola: " + jugadorId);

        matchmakingManager.colaJugadores.Add(opcionesJugador.UIJugador);
    }
    
    // El cliente llama a esto para retirarse de la cola de matchmaking
    [ServerRpc]
    public void RetirarseDeLaColaServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong jugadorId = rpcParams.Receive.SenderClientId;
        Debug.Log("Jugador eliminado de la cola: " + jugadorId);

        matchmakingManager.colaJugadores.Remove(opcionesJugador.UIJugador);
    }

    //AÑADIR JUGADOR A PARTIDA
    public void MeterJugadorEnLaPartida(int IDPartida)
    {
        if (IsServer)
        {
            MatchmakingManager.ListaDePartidas[IDPartida].jugadores.Add(opcionesJugador.controladorDelJugador);
            MeterJugadorEnLaPartidaClientRpc(IDPartida);
        }
    }
    
    [ClientRpc]
    public void MeterJugadorEnLaPartidaClientRpc(int IDPartida)
    {
        Debug.Log("Meter jugador en partida (Client)");
        if (IsClient && !IsHost)
        {
            MatchmakingManager.ListaDePartidas[IDPartida].jugadores.Add(opcionesJugador.controladorDelJugador);
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
            AudioManager.Instance.PlaySFX(pitidoGrave);
            if(posicionCuentaAtras != 0)
                NumerosCuentaAtras[posicionCuentaAtras - 1].SetActive(false);
            NumerosCuentaAtras[posicionCuentaAtras].SetActive(true);
            posicionCuentaAtras++;
            Invoke("contadorCuentaAtras", 1.0f);
        }
        else
        {
            AudioManager.Instance.PlaySFX(pitidoAgudo);
            NumerosCuentaAtras[posicionCuentaAtras - 1].SetActive(false);
            CuentaAtras.SetActive(false);
            posicionCuentaAtras = 0;
            //opcionesJugador.reactivarMovimiento();
        }
    }

    ////////////////////////////////////////////
    /// Música y cambios de canción
    /// ///////////////////////////////////////
    
    public void musicaInGame()
    {
        if(IsOwner)
        {
            AudioManager.Instance.PlayGameMusic();
        }
    }

    public void pararMusica()
    {
        if(IsOwner)
        {
            AudioManager.Instance.StopMusic();
        }
    }
}
