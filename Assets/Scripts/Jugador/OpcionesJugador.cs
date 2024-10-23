using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;

public class OpcionesJugador : MonoBehaviour
{
    //ACTIVA O DESACTIVA LA UI
    public bool ActivarUI = true;

    //UI DEL JUGADOR
    public UI UIJugador;
    
    //ACTIVA O DESACTIVA EL MOVIMIENTO DEL JUGADOR
    public bool movimientoActivado = false;

    //CONTROLADOR DEL JUGADOR
    public ControladorDelJugador controladorDelJugador;

    //CHARACTER CONTROLLER DE LA NAVE
    public CharacterController controladorDeLaNave;
    
    //NETWORK TRANSFORM DE LA NAVE
    public NetworkTransform networkTransformNave;

    public void desactivarMovimiento()
    {
        movimientoActivado = false;
        controladorDeLaNave.enabled = false;
        networkTransformNave.Interpolate = false;
    }
    
    public void reactivarMovimiento()
    {
        movimientoActivado = true;
        controladorDeLaNave.enabled = true;
        networkTransformNave.Interpolate = true;
    }

}
