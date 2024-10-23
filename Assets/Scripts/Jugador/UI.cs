using Unity.Netcode;
using UnityEngine;

public class UI : MonoBehaviour
{
    
    [SerializeField] private ControladorDelJugador CJugador;
    
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
    }

    //TODAS ESTAS FUNCIONES NECESITAN MÁS FUNCIONALIDAD, ESTO ES UN PLACEHOLDER

    public void ActivarUI()
    {
        LogIn.SetActive(true);
    }

    //RELACIONADAS CON LOGIN////////////////////

    public void IniciarSesion()
    {
        LogIn.SetActive(false);
        Personalizacion.SetActive(true);
    }
    
    public void CrearNuevoUsuario()
    {
        LogIn.SetActive(false);
        Personalizacion.SetActive(true);
    }
    
    public void CerrarSesion()
    {
        Personalizacion.SetActive(false);
        LogIn.SetActive(true);
    }
    
    ////////////////////////////////////////////

    //RELACIONADAS CON BUSCAR PARTIDA///////////
    
    public void BuscarPartida()
    {
        Personalizacion.SetActive(false);
        BuscandoPartida.SetActive(true);

        FindObjectOfType<MatchmakingManager>().UnirseALaColaServerRpc();
    }
    
    public void DejarDeBuscarPartida()
    {
        BuscandoPartida.SetActive(false);
        Personalizacion.SetActive(true);
    }

    public void PartidaEncontrada()
    {
        BuscandoPartida.SetActive(false);
        EnPartida.SetActive(true);
        CJugador.activarMovimiento = true;
    }

    ////////////////////////////////////////////
    
    //RELACIONADAS CON LA PARTIDA///////////////

    public void Ganar()
    {
        EnPartida.SetActive(false);
        Victoria.SetActive(true);
        CJugador.activarMovimiento = false;
        CJugador.GameEndServerRpc();
    }
    
    public void Perder()
    {
        EnPartida.SetActive(false);
        Derrota.SetActive(true);
        CJugador.activarMovimiento = false;
        CJugador.GameEndServerRpc();
    }

    public void VolverAPersonalizacionDesdePartida()
    {
        Victoria.SetActive(false);
        Derrota.SetActive(false);
        Personalizacion.SetActive(true);
    }

    ////////////////////////////////////////////

    
    
    //RELACIONADAS CON INSTRUCCIONES////////////
    
    public void MirarInstrucciones()
    {
        Personalizacion.SetActive(false);
        Instrucciones.SetActive(true);
    }
    
    public void VolverAPersonalizacionDesdeInstrucciones()
    {
        Instrucciones.SetActive(false);
        Personalizacion.SetActive(true);
    }
    
    ////////////////////////////////////////////
    
    //RELACIONADAS CON CREDITOS/////////////////
    
    public void MirarCreditos()
    {
        Personalizacion.SetActive(false);
        Creditos.SetActive(true);
    }
    
    public void VolverAPersonalizacionDesdeCreditos()
    {
        Creditos.SetActive(false);
        Personalizacion.SetActive(true);
    }
    
    ////////////////////////////////////////////
}
