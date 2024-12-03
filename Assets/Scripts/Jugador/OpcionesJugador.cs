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
    
    //BOOL PARA VER SI SE ESTA PROBANDO ALGO
    public bool testing = false;

    //DATOS DEL USUARIO
    public Usuario usuario;

    void Start()
    {
        desactivarMovimiento();
        if (IsServer)
        {
            controladorDelJugador.nave.SetToSpawn(GameObject.Find("SpawnPrincipal"), true);
        }
        if (IsClient && !IsOwner)
        {
            deshabilitarNave();
            preguntarNombreServerRpc();
        }
    }

    public void desactivarMovimiento()
    {
        movimientoActivado = false;
        networkTransformNave.Interpolate = false;
        nave.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    
    public void reactivarMovimiento()
    {
        movimientoActivado = true;
        networkTransformNave.Interpolate = true;
    }

    public void deshabilitarNave()
    {
        nave.GetComponent<Rigidbody>().velocity = Vector3.zero;
        nave.SetActive(false);
    }

    public void rehabilitarNave()
    {
        nave.SetActive(true);
    }

    public void resetToInitialState()
    {
        desactivarMovimiento();
        if (IsServer)
        {
            controladorDelJugador.ResetSupportItems();
            controladorDelJugador.nave.SetToSpawn(GameObject.Find("SpawnPrincipal"), true);

            //VFX
            controladorDelJugador.nave.LowHealthVFXClientRpc(false);
        }
        if (IsClient && !IsOwner)
        {
            deshabilitarNave();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void preguntarNombreServerRpc()
    {
        apuntarNombreClientRpc(usuario.name);
    }

    [ClientRpc(RequireOwnership = false)]
    private void apuntarNombreClientRpc(string nombre)
    {
        usuario.name = nombre;
    }
}
