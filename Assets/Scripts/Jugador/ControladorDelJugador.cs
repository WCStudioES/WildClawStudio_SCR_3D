using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorDelJugador : NetworkBehaviour
{
    //CONTROLADOR DE LA NAVE
    [SerializeField] public CController nave;
    
    //CONTIENE LAS OPCIONES DE CONFIGURACION DEL JUGADOR
    public OpcionesJugador opcionesJugador;
    
    //INDICA SI LA NAVE HA SIDO DESTRUIDA O NO
    //TODO AÚN SIN UTILIZAR, UTILIZAR CUANDO SE IMPLEMENTE PERDER VIDA
    public bool naveDestruida = false;
    
    
    [SerializeField] private GameObject proyectilPrefab; // Prefab del proyectil
    [SerializeField] private Transform puntoDisparo; // Lugar donde se instancia el proyectil

    private bool isMoving = false; // Indica si la tecla de movimiento está presionada
    private float rotationInput = 0f; // Almacena el input de rotación
    public NetworkVariable<int> hp = new NetworkVariable<int>(100);

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
        if (IsOwner && opcionesJugador.movimientoActivado)
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
        //Debug.Log(context.action.name);
        if (IsOwner && opcionesJugador.movimientoActivado)
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

    // Función que aplica daño a la nave
    public void GetDamage(int dmg)
    {
        hp.Value -= dmg;  // Resta la cantidad de daño a la vida de la nave
        Debug.Log("Vida actual de la nave: " + hp);

        // Si la vida llega a 0, destruye la nave (puedes modificar esto para otro comportamiento)
        if (hp.Value <= 0)
        {
            //Pierdes;
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
        //nave.SetToSpawn();
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
        // Crear el proyectil solo en el servidor
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);

        // Obtener el componente del proyectil y establecer la dirección (forward de la nave)
        Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();
        proyectilScript.Inicializar(puntoDisparo.forward);

        // El proyectil se destruirá automáticamente tras 2 segundos
        Destroy(proyectil, 2f);

        SpawnProyectilClientRpc(puntoDisparo.position, puntoDisparo.rotation, puntoDisparo.forward);
    }


    // GESTIONA LA REPLICACIÓN DE CADA DISPARO EN LOS CLIENTES
    [ClientRpc]
    private void SpawnProyectilClientRpc(Vector3 posicion, Quaternion rotacion, Vector3 direccion)
    {
        // Crear el proyectil en el cliente
        GameObject proyectil = Instantiate(proyectilPrefab, posicion, rotacion);

        // Configurar la dirección del proyectil en el cliente
        Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();
        proyectilScript.Inicializar(direccion);

        // El proyectil se destruirá automáticamente tras 2 segundos en el cliente
        Destroy(proyectil, 2f);
    }
    
    //RESTAURA LA POSICIÓN DE LAS NAVES Y SU VIDA
    //TODO RESTAURAR EXPERIENCIA Y NIVEL DEL ARMA
    public void restaurarNaves(GameObject spawnPosition)
    {
        if (IsServer)
        {
            //DEVUELVE LA NAVE A LA POSICION DE SPAWN
            nave.SetToSpawn(spawnPosition);

            //RESTAURA LA VIDA DE LA NAVE
        
            hp.Value = 100;
        }
    }

}
