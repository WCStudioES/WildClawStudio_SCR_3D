using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorDelJugador : NetworkBehaviour
{
    // ASIGNAR EL CONTROLADOR DE LA NAVE
    [SerializeField] private CController nave;
    public bool activarMovimiento = false;
    [SerializeField] private GameObject proyectilPrefab; // Prefab del proyectil
    [SerializeField] private Transform puntoDisparo; // Lugar donde se instancia el proyectil

    private bool isMoving = false; // Indica si la tecla de movimiento está presionada
    private float rotationInput = 0f; // Almacena el input de rotación

    private void Start()
    {
        if (IsOwner)
        {
            OnPlayerStartServerRpc();
            nave.AssignMainCamera();
        }
    }

    private void Update()
    {
        if (IsOwner && activarMovimiento)
        {
            // Llama a la función de movimiento continuamente si la tecla de movimiento está presionada
            if (isMoving)
            {
                OnMoveServerRpc();
            }

            // Llama a la función de rotación si hay input de rotación
            if (rotationInput != 0f)
            {
                Debug.Log("ROTANDO");
                OnRotateServerRpc(rotationInput);
            }
        }
    }

    // RECOGE LOCALMENTE EL INPUT DEL JUGADOR
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.name);
        if (IsOwner && activarMovimiento)
        {
            switch (context.action.name)
            {
                case "Move":
                    if (context.performed)
                    {
                        isMoving = true;  // Activa el movimiento continuo cuando la tecla se presiona
                    }
                    else if (context.canceled)
                    {
                        isMoving = false; // Detiene el movimiento cuando se suelta la tecla
                    }
                    break;

                case "Shoot":
                    if (context.performed)
                    {
                        OnShootServerRpc();
                    }
                    break;

                case "Rotate":
                    if (context.performed)
                    {
                        rotationInput = context.ReadValue<float>(); // Almacena el valor de rotación
                    }
                    else if (context.canceled)
                    {
                        rotationInput = 0f; // Detiene la rotación cuando se suelta la tecla
                        OnRotateServerRpc(rotationInput);
                    }
                    //Debug.Log(rotationInput);
                    break;
            }
        }
    }

    // ACTUALIZA LA DIRECCIÓN DEL JUGADOR
    [ServerRpc]
    private void OnMoveServerRpc()
    {
        nave.Move(); // Llama a la función de movimiento en la nave
    }

    // GESTIONA LA ROTACIÓN DE LA NAVE
    [ServerRpc]
    private void OnRotateServerRpc(float input)
    {
        nave.Rotate(input); // Llama a la función de rotación en la nave
    }

    // LLEVA AL JUGADOR A LA POSICIÓN DE SPAWN
    [ServerRpc]
    private void OnPlayerStartServerRpc()
    {
        nave.SetToSpawn();
    }

    // PARA AL JUGADOR
    [ServerRpc]
    public void GameEndServerRpc()
    {
        nave.Stop();
    }

    // GESTIONA EL DISPARO DE PROYECTILES
    [ServerRpc]
    private void OnShootServerRpc()
    {
        // Instanciar el proyectil en el servidor
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);

        // Propagar la instancia del proyectil a todos los clientes
        NetworkObject proyectilNetworkObject = proyectil.GetComponent<NetworkObject>();
        proyectilNetworkObject.Spawn();

        Rigidbody rb = proyectil.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Aplica una fuerza en la dirección 'forward' del proyectil para que se mueva
            rb.AddForce(proyectil.transform.forward * 20f, ForceMode.Impulse); // Ajusta la magnitud de la fuerza según sea necesario
        }

        // Destruir el proyectil después de 2 segundos
        Destroy(proyectil, 2f);
    }
}
