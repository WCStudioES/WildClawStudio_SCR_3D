using System;
using DefaultNamespace;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ControladorDelJugador : NetworkBehaviour, ICanGetDamage
{
    //CONTROLADOR DE LA NAVE
    [SerializeField] public CController nave;
    
    //CONTIENE LAS OPCIONES DE CONFIGURACION DEL JUGADOR
    public OpcionesJugador opcionesJugador;
    
    //INDICA SI LA NAVE HA SIDO DESTRUIDA O NO
    //TODO AÚN SIN UTILIZAR, UTILIZAR CUANDO SE IMPLEMENTE PERDER VIDA
    public bool naveDestruida = false;
    
    //VARIABLES DE DISPARO
    [SerializeField] private GameObject proyectilBasicoPrefab; // Prefab del proyectil basico
    [SerializeField] private GameObject proyectilMejoradoPrefab; // Prefab del proyectil escogido
    private GameObject proyectilEnUso; // Prefab del proyectil escogido
    
    [SerializeField] private Transform[] puntoDisparo; // Lugar donde se instancia el proyectil
    
    private bool isShooting = false; //Booleano para saber si el jugador esta disparando
    private float cadenciaDeDisparo; // cadencia de disparo del proyectil
    private int tipoProyectil; // tipo de proyectil del prefab
    private float shotTimer = 0f; // Temporizador para la cadencia
    
    
    //VARIABLES DE MOVIMIENTO
    public bool isMoving = false; // Indica si la tecla de movimiento está presionada
    private float rotationInput = 0f; // Almacena el input de rotación
    
    //VARIABLES DE ESTADISTICAS
    public int armaduraInicial; //Armadura para empezar la partida
    public int vidaInicial; //Vida para empezar cada ronda
    public NetworkVariable<int> hp = new NetworkVariable<int>(100); //Vida de la nave
    public NetworkVariable<int> armadura = new NetworkVariable<int>(10); //Armadura de la nave
    public NetworkVariable<int> xp = new NetworkVariable<int>(00); //Experiencia de la nave
    public NetworkVariable<int> lvl = new NetworkVariable<int>(1); //Nivel de la nave
    [SerializeField] NetworkVariable<int> xpADar = new NetworkVariable<int>(200); //Experiencia que da al otro jugador al destruir tu nave


    public Image barraDeVida; //Gameobject de la barra de vida
    public GameObject cuerpoNave;  //Gameobject con el collider de la nave, evita autohit
    
    //public int equipo;  Para luego que no haya fuego amigo entre equipos

    private void Start()
    {
        CambiarArma(proyectilBasicoPrefab);
        if (IsOwner)
        {
            OnPlayerStartServerRpc();
            nave.AssignMainCamera();
        }
        
    }

    //Funcion para resetear los valores predefinidos antes de empezar partida
    public void ResetPrePartida()
    {
        if (IsServer)
        {
            armadura.Value = armaduraInicial;
            xp.Value = 0;
            lvl.Value = 1;
        }
        
        CambiarArma(proyectilBasicoPrefab);
    }
    
    //RESTAURA LA POSICIÓN DE LAS NAVES Y SU VIDA
    //TODO RESTAURAR EXPERIENCIA Y NIVEL DEL ARMA
    public void resetDeRonda(GameObject spawnPosition)
    {
        if (IsServer)
        {
            //DEVUELVE LA NAVE A LA POSICION DE SPAWN
            nave.SetToSpawn(spawnPosition);

            //RESTAURA LA VIDA DE LA NAVE
            hp.Value = vidaInicial;
        }
        UpdateHealthBarClientRpc(hp.Value);
    }
    //Funcion que maneja la barra de vida
    [ClientRpc]
    public void UpdateHealthBarClientRpc(int vida)
    {
        float healthPercentage = vida / (float) vidaInicial;
        Debug.Log(healthPercentage);
        barraDeVida.fillAmount = healthPercentage;
    }
    

    private void Update()
    {
        if (!opcionesJugador.movimientoActivado)
        {
            isMoving = false;
            rotationInput = 0.0f;
            isShooting = false;
        }

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
                //Debug.Log("ROTANDO");
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
                        Debug.Log("");
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
    public void GetDamage(int dmg, ControladorDelJugador dueñoDaño)
    {
        hp.Value -= (dmg - armadura.Value);  // Resta la cantidad de daño a la vida de la nave

        // Si la vida llega a 0, destruye la nave (puedes modificar esto para otro comportamiento)
        if (hp.Value <= 0)
        {
            dueñoDaño.GetXP(xp.Value);
            Debug.Log("Xp de jugador: " + dueñoDaño.xp.Value);
            //Pierdes;
        }
        UpdateHealthBarClientRpc(hp.Value); //Actualizar barra de vida
        Debug.Log("Vida actual de la nave: " + hp);
    }
    
    //Funcion que gestiona la obtención de xp del jugador
    public void GetXP(int xpRecibida)
    {
        //Sumar experiencia
        xp.Value += xpRecibida;

        //Lvl 3, aplicar mejoras
        if (xp.Value >= 900 && lvl.Value < 3 )
        {
            Debug.Log("Level 3");
            armadura.Value += armadura.Value;
            xpADar.Value += 50;
            lvl.Value++;

        }
        //Lvl 2, aplicar mejoras
        else if (xp.Value >= 300 && lvl.Value < 2)
        {
            Debug.Log("Level 2");
            CambiarArma(proyectilMejoradoPrefab);
            armadura.Value += armadura.Value;
            xpADar.Value += 50;
            lvl.Value++;
        } 
        
    }

    //Funcion que cambia el arma de la nave y aplica sus estdisticas
    private void CambiarArma(GameObject proyectilNuevo)
    {
        proyectilEnUso = proyectilNuevo;
        cadenciaDeDisparo = proyectilNuevo.GetComponent<Proyectil>().cadencia;
        tipoProyectil = proyectilNuevo.GetComponent<Proyectil>().tipo;
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
        //TODO CAMBIAR ESTO DESPUES
        //hp.Value = 0;

            //Si no puede disparar hace return
            if (ComprobadorDeCadencia() == false) { return;}

            //Si es el dobleyectil, dispara por los dos cañones asociados
            if (tipoProyectil == 2)
            {
                // Crear el proyectil solo en el servidor
                GameObject proyectil1 = NetworkManager.Instantiate(proyectilEnUso, puntoDisparo[1].position, puntoDisparo[1].rotation);
                GameObject proyectil2 = NetworkManager.Instantiate(proyectilEnUso, puntoDisparo[2].position, puntoDisparo[2].rotation);

                // Obtener el componente del proyectil y establecer la dirección (forward de la nave)
                Proyectil proyectilScript1 = proyectil1.GetComponent<Proyectil>();
                proyectilScript1.Inicializar(puntoDisparo[1].forward, cuerpoNave, this, IsServer);
            
                Proyectil proyectilScript2 = proyectil2.GetComponent<Proyectil>();
                proyectilScript2.Inicializar(puntoDisparo[2].forward, cuerpoNave, this, IsServer);

                // El proyectil se destruirá automáticamente tras 2 segundos
                Destroy(proyectil1, 2f);
                Destroy(proyectil2, 2f);
                
                //Spawnea proyectil en el cliente
                SpawnProyectilClientRpc(puntoDisparo[1].position, puntoDisparo[1].rotation, puntoDisparo[1].forward);
                SpawnProyectilClientRpc(puntoDisparo[2].position, puntoDisparo[2].rotation, puntoDisparo[2].forward);
            }
            else
            {
                // Crear el proyectil solo en el servidor
                GameObject proyectil = Instantiate(proyectilEnUso, puntoDisparo[0].position, puntoDisparo[0].rotation);

                // Obtener el componente del proyectil y establecer la dirección (forward de la nave)
                Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();
                proyectilScript.Inicializar(puntoDisparo[0].forward, cuerpoNave, this, IsServer);

                // El proyectil se destruirá automáticamente tras 2 segundos
                Destroy(proyectil, 2f);

                //Spawnea proyectil en el cliente
                SpawnProyectilClientRpc(puntoDisparo[0].position, puntoDisparo[0].rotation, puntoDisparo[0].forward);
            }
        
    }


    // GESTIONA LA REPLICACIÓN DE CADA DISPARO EN LOS CLIENTES
    [ClientRpc]
    private void SpawnProyectilClientRpc(Vector3 posicion, Quaternion rotacion, Vector3 direccion)
    {
        if(!IsServer)
        {
            // Crear el proyectil en el cliente
            GameObject proyectil = NetworkManager.Instantiate(proyectilEnUso, posicion, rotacion);

            // Configurar la dirección del proyectil en el cliente
            Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();
            proyectilScript.Inicializar(direccion, cuerpoNave,this , IsServer);

            // El proyectil se destruirá automáticamente tras 2 segundos en el cliente
            Destroy(proyectil, 2f);
        }
    }

}
