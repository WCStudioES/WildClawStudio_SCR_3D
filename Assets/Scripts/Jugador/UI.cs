using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI : NetworkBehaviour
{
    
    [SerializeField] private NetworkedPlayer CJugador;
    [SerializeField] private OpcionesJugador opcionesJugador;
    [SerializeField] private CustomizationManager customizationManager;
    
    [SerializeField]private GameObject LogIn;
    [SerializeField]private GameObject Personalizacion;
    [SerializeField]private GameObject Settings;
    [SerializeField]private GameObject SettingsLogIn;
    public Image brightnessPanel;
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
    [SerializeField]private GameObject About;
    private int posicionCuentaAtras = 0;
    
    [SerializeField] private GameObject[] uiPantallasTutorial;
    private int indexTutorial = 0;

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
        {
            ActivarUI();

            //APLICAR AJUSTES DE BRILLO
            SettingsManager.Instance.brightnessPanel = brightnessPanel;
            SettingsManager.Instance.LoadBrightness();
        }
    }

    private void Update()
    {
        if (IsOwner && !IsHost)
        {
            //SE ENCARGA DE COLOCAR LA CAMARA BIEN CADA VEZ QUE ENTRAS O SALES DE LA PAGINA DE PERSONALIZACIÓN
            if(Personalizacion.activeSelf)
            {
                CJugador.nave.AssignMainCamera(ControladorNave.CameraType.Customization);
                //cameraInGame = false;
            }
            else if(!Personalizacion.activeSelf)
            {
                CJugador.nave.AssignMainCamera(ControladorNave.CameraType.InGame);
                //cameraInGame = true;
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
            opcionesJugador.rehabilitarNave();
            LogIn.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }
    
    public void CrearNuevoUsuario()
    {
        if (IsOwner)
        {
            opcionesJugador.rehabilitarNave();
            LogIn.SetActive(false);
            Personalizacion.SetActive(true);
        }
    }
    
    public void CerrarSesion()
    {
        if (IsOwner)
        {
            opcionesJugador.deshabilitarNave();
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
    
    //RESPUESTA SI SE HA CREADO UN NUEVO USUARIO(REGISTRO)
    [ClientRpc]
    void UsuarioCreadoClientRpc(string usuario)
    {
        ScriptRegister.UsuarioValido(usuario);
    }

    //HISTORIAL
    public void AbrirHistorial()
    {
        if (IsOwner)
        {
            Personalizacion.SetActive(false);
            Historial.SetActive(true);
            opcionesJugador.deshabilitarNave();
        }
    }

    public void CerrarHistorial()
    {
        if (IsOwner)
        {
            Historial.SetActive(false);
            Personalizacion.SetActive(true);
            opcionesJugador.rehabilitarNave();
        }
    }

    //AJUSTES
    public void AbrirSettings()
    {
        //Debug.Log("Hola");
        Settings.SetActive(true);
    }

    public void CerrarSettings()
    {
        //Debug.Log("Hola");
        Settings.SetActive(false);
    }

    public void AbrirSettingsLogIn()
    {
        //Debug.Log("Hola");
        SettingsLogIn.SetActive(true);
    }

    public void CerrarSettingsLogIn()
    {
        //Debug.Log("Hola");
        SettingsLogIn.SetActive(false);
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
            VictoriaRonda.SetActive(false);
            DerrotaRonda.SetActive(false);
            opcionesJugador.movimientoActivado = false;
            if (partidaFinalizada)
            {
                CancelInvoke();
                opcionesJugador.deshabilitarNave();
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
            CJugador.GameEndServerRpc();
            opcionesJugador.deshabilitarNave();
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
            CJugador.GameEndServerRpc();
            opcionesJugador.deshabilitarNave();
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
        opcionesJugador.rehabilitarNave();
        opcionesJugador.movimientoActivado = true;
    }

    ////////////////////////////////////////////    
    //RELACIONADAS CON INSTRUCCIONES////////////
    
    public void MirarInstrucciones()
    {
        if (IsOwner)
        {
            Personalizacion.SetActive(false);
            Instrucciones.SetActive(true);
            opcionesJugador.deshabilitarNave();
        }
    }
    
    public void VolverAPersonalizacionDesdeInstrucciones()
    {
        if (IsOwner)
        {
            Instrucciones.SetActive(false);
            Instrucciones2.SetActive(false);
            Personalizacion.SetActive(true);
            opcionesJugador.rehabilitarNave();
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
    public void VolverAInstruccionesDesdeInstrucciones2()
    {
        if (IsOwner)
        {
            Instrucciones.SetActive(true);
            Instrucciones2.SetActive(false);
            uiPantallasTutorial[indexTutorial].SetActive(false);
            indexTutorial = 0;
            uiPantallasTutorial[indexTutorial].SetActive(true);
        }
    }
    
    public void CambiarPantallaTutorial(bool isNext)
    {
        if (IsOwner)
        {
            uiPantallasTutorial[indexTutorial].SetActive(false);
            if (isNext)
            {
                if (indexTutorial == uiPantallasTutorial.Length - 1)
                    indexTutorial = 0;
                else
                    indexTutorial++;
                
                uiPantallasTutorial[indexTutorial].SetActive(true);
            }

            else
            {
                if (indexTutorial == 0)
                    indexTutorial = uiPantallasTutorial.Length - 1;
                else
                    indexTutorial--;
                
                uiPantallasTutorial[indexTutorial].SetActive(true);
            }
  
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
            opcionesJugador.deshabilitarNave();
        }
    }
    
    public void VolverAPersonalizacionDesdeCreditos()
    {
        if (IsOwner)
        {
            AudioManager.Instance.PlayMenuMusic();
            Creditos.SetActive(false);
            Personalizacion.SetActive(true);
            opcionesJugador.rehabilitarNave();
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

    public void AbrirPopUpAbout()
    {
        if (IsOwner)
        {
            About.SetActive(true);
        }
    }
    
    public void CerrarPopUpAbout()
    {
        if (IsOwner)
        {
            About.SetActive(false);
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
            // Añadir el jugador actual al servidor
            var jugadorActual = opcionesJugador.controladorDelJugador;
            MatchmakingManager.ListaDePartidas[IDPartida].jugadores.Add(jugadorActual);

            // Crear la lista de clientes objetivo
            List<ulong> jugadoresIds = new List<ulong>();
            foreach (var jugador in MatchmakingManager.ListaDePartidas[IDPartida].jugadores)
            {
                jugadoresIds.Add(jugador.OwnerClientId); // Obtener los IDs de los jugadores en la partida
            }

            // Obtener el rival (solo funciona si hay exactamente dos jugadores en la partida)
            var rival = MatchmakingManager.ListaDePartidas[IDPartida].jugadores
                .Find(j => j.OwnerClientId != jugadorActual.OwnerClientId);

            if (rival != null)
            {
                // Enviar el ClientRpc a ambos jugadores
                MeterJugadorEnLaPartidaClientRpc(
                    IDPartida,
                    jugadorActual.OwnerClientId,
                    rival.OwnerClientId,
                    new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = jugadoresIds.ToArray()
                        }
                    });
            }
        }
    }
    
    [ClientRpc]
    public void MeterJugadorEnLaPartidaClientRpc(int IDPartida, ulong myID, ulong rivalID, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Meter jugador en partida (Client)");
        if (IsClient)
        {
            // Obtener la partida localmente
            var partida = MatchmakingManager.ListaDePartidas[IDPartida];

            // Añadir el jugador actual (si no está ya en la lista)
            if (!partida.jugadores.Exists(j => j.OwnerClientId == myID))
            {
                partida.jugadores.Add(opcionesJugador.controladorDelJugador);
                opcionesJugador.controladorDelJugador.partida = partida;
                VFXManager.Instance.SetGameID(IDPartida);
            }

            // Añadir al rival (si no está ya en la lista)
            var jugadorRival = FindJugadorById(rivalID);
            if (jugadorRival != null && !partida.jugadores.Exists(j => j.OwnerClientId == rivalID))
            {
                jugadorRival.partida = partida;
                partida.jugadores.Add(jugadorRival);
            }
        }
    }

    // Método auxiliar para encontrar un jugador por su ID (NetworkedPlayer)
    private NetworkedPlayer FindJugadorById(ulong id)
    {
        foreach (var player in FindObjectsOfType<NetworkedPlayer>())
        {
            if (player.OwnerClientId == id) // Comparar con el ClientId del jugador
            {
                return player;
            }
        }
        return null; // No se encontró el jugador
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
    
    public void musicaMenu()
    {
        if(IsOwner)
        {
            AudioManager.Instance.PlayMenuMusic();
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
