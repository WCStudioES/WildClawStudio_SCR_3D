using System;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorDelJugador : NetworkBehaviour, ICanGetDamage
{
    //CONTROLADOR DE LA NAVE
    [SerializeField] public CController nave;
    
    //CONTIENE LAS OPCIONES DE CONFIGURACION DEL JUGADOR
    public OpcionesJugador opcionesJugador;
    
    //INDICA SI LA NAVE HA SIDO DESTRUIDA O NO
    //TODO AÚN SIN UTILIZAR, UTILIZAR CUANDO SE IMPLEMENTE PERDER VIDA
    public bool naveDestruida = false;
    
    
    [SerializeField] private GameObject proyectilPrefab; // Prefab del proyectil
    [SerializeField] private Transform[] puntoDisparo; // Lugar donde se instancia el proyectil
    private bool isShooting = false;
    private float cadenciaDeDisparo; // cadencia de disparo del proyectil
    private int tipoProyectil; // tipo de proyectil del prefab
    private float shotTimer = 0f; // Temporizador para la cadencia

    private bool isMoving = false; // Indica si la tecla de movimiento está presionada
    private float rotationInput = 0f; // Almacena el input de rotación
    public NetworkVariable<int> hp = new NetworkVariable<int>(100);
    
    //public int equipo;  Para luego que no haya fuego amigo entre equipos

    private void Start()
    {
        cadenciaDeDisparo = proyectilPrefab.GetComponent<Proyectil>().cadencia;
        tipoProyectil = proyectilPrefab.GetComponent<Proyectil>().tipo;
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

        if (isShooting)
        {
            OnShootServerRpc();
        }
        shotTimer += Time.deltaTime;
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
                        isShooting = true;
                    }
                    else if (context.canceled)
                    {
                        isShooting = false;
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

    private bool ComprobadorDeCadencia()
    {
        //Debug.Log("Cadencia");

        // Verificar si ya ha pasado el tiempo suficiente para disparar nuevamente
        if (shotTimer >= cadenciaDeDisparo)
        {
            // Reiniciar el temporizador
            shotTimer = 0f;
            
            // Llamar a la función para disparar
            //Debug.Log("ComprobadorCadencia aceptado");
            return true;
        }
        else
        {
            //Debug.Log("ComprobadorCadencia denegado");
            return false;
        }
    }
    [ServerRpc]
    private void OnShootServerRpc()
    {
        //Si no puede disparar hace return
        if (ComprobadorDeCadencia() == false) { return;}

        if (tipoProyectil == 2)
        {
            // Crear el proyectil solo en el servidor
            GameObject proyectil1 = Instantiate(proyectilPrefab, puntoDisparo[1].position, puntoDisparo[1].rotation);
            GameObject proyectil2 = Instantiate(proyectilPrefab, puntoDisparo[2].position, puntoDisparo[2].rotation);

            // Obtener el componente del proyectil y establecer la dirección (forward de la nave)
            Proyectil proyectilScript1 = proyectil1.GetComponent<Proyectil>();
            proyectilScript1.Inicializar(puntoDisparo[1].forward, this);
            
            Proyectil proyectilScript2 = proyectil2.GetComponent<Proyectil>();
            proyectilScript2.Inicializar(puntoDisparo[2].forward, this);

            // El proyectil se destruirá automáticamente tras 2 segundos
            Destroy(proyectil1, 2f);
            Destroy(proyectil2, 2f);

            SpawnProyectilClientRpc(puntoDisparo[1].position, puntoDisparo[1].rotation, puntoDisparo[1].forward);
            SpawnProyectilClientRpc(puntoDisparo[2].position, puntoDisparo[2].rotation, puntoDisparo[2].forward);
        }
        else
        {
            // Crear el proyectil solo en el servidor
            GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo[0].position, puntoDisparo[0].rotation);

            // Obtener el componente del proyectil y establecer la dirección (forward de la nave)
            Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();
            proyectilScript.Inicializar(puntoDisparo[0].forward, this);

            // El proyectil se destruirá automáticamente tras 2 segundos
            Destroy(proyectil, 2f);

            SpawnProyectilClientRpc(puntoDisparo[0].position, puntoDisparo[0].rotation, puntoDisparo[0].forward);
        }
    }


    // GESTIONA LA REPLICACIÓN DE CADA DISPARO EN LOS CLIENTES
    [ClientRpc]
    private void SpawnProyectilClientRpc(Vector3 posicion, Quaternion rotacion, Vector3 direccion)
    {
        // Crear el proyectil en el cliente
        GameObject proyectil = Instantiate(proyectilPrefab, posicion, rotacion);

        // Configurar la dirección del proyectil en el cliente
        Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();
        proyectilScript.Inicializar(direccion, this);

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
