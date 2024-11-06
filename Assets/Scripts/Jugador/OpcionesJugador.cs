using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


public class OpcionesJugador : NetworkBehaviour
{
    //ACTIVA O DESACTIVA LA UI
    public bool ActivarUI = true;

    //UI DEL JUGADOR
    public UI UIJugador;
    
    //ACTIVA O DESACTIVA EL MOVIMIENTO DEL JUGADOR
    public bool movimientoActivado = false;

    //CONTROLADOR DEL JUGADOR
    public NetworkedPlayer controladorDelJugador;

    //NETWORK TRANSFORM DE LA NAVE
    public NetworkTransform networkTransformNave;
    
    //NAVE
    public GameObject nave;

    void Start()
    {
        desactivarMovimiento();
        if (IsServer)
        {
            //controladorDelJugador.nave.SetToSpawn(GameObject.Find("SpawnPrincipal"));
        }
        if (IsClient && !IsOwner)
        {
            deshabilitarNave();
        }
    }

    public void desactivarMovimiento()
    {
        movimientoActivado = false;
        networkTransformNave.Interpolate = false;
    }
    
    public void reactivarMovimiento()
    {
        movimientoActivado = true;
        networkTransformNave.Interpolate = true;
    }

    public void deshabilitarNave()
    {
        nave.SetActive(false);
    }

    public void rehabilitarNave()
    {
        controladorDelJugador.nave.currentSpeed = 0;
        nave.SetActive(true);
    }

    public void resetToInitialState()
    {
        desactivarMovimiento();
        if (IsServer)
        {
            controladorDelJugador.nave.SetToSpawn(GameObject.Find("SpawnPrincipal"));
        }
        if (IsClient && !IsOwner)
        {
            deshabilitarNave();
        }
    }

}
