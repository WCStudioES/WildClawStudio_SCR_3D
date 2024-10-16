using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorDelJugador : NetworkBehaviour
{

    //ASIGNAR EL CONTROLADOR DE LA NAVE
    [SerializeField] private CController nave;
    public bool activarMovimiento = false;
    
    private void Start()
    {
        if (IsOwner)
        {
            OnPlayerStartServerRpc();
            nave.AssignMainCamera();
        }
    }

    //RECOGE LOCALMENTE EL INPUT DE MOVIMIENTO
    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsOwner && activarMovimiento)
            OnMoveServerRpc(context.ReadValue<Vector2>());
    }
    
    //ACTUALIZA LA DIRECCIÓN DEL JUGADOR
    [ServerRpc]
    private void OnMoveServerRpc(Vector2 input)
    {
        nave.Move(input);
    }
    
    //LLEVA AL JUGADOR A LA POSICIÓN DE SPAWN
    [ServerRpc]
    private void OnPlayerStartServerRpc()
    {
        nave.SetToSpawn();
    }
    
    //PARA AL JUGADOR
    [ServerRpc]
    public void GameEndServerRpc()
    {
        nave.Stop();
    }


}
