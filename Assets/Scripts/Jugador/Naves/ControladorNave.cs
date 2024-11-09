using System.Collections;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ControladorNave : NetworkBehaviour
{
    [SerializeField] private Transform CameraPosition;

    private Vector3 direccionMovimiento = Vector3.zero; // Dirección del movimiento (hacia adelante)
    public Vector3 velocity = Vector3.zero;

    public float rotationSpeed = 100f; // Velocidad de rotación
    public float acceleration = 5f; // Aceleración
    public float deceleration = 50f; // Desaceleración (fricción)
    public float maxSpeed = 10f; // Velocidad máxima alcanzable

    [SerializeField] private OpcionesJugador opcionesJugador;
    [SerializeField] public PlayerShip playerShip;
    public CapsuleCollider collider;

    void Start()
    {
        if (IsOwner)
        {
            opcionesJugador = GetComponentInParent<OpcionesJugador>();
            AssignMainCamera();
        }
    }

    void Update()
    {
        Debug.Log("Transform ControladorNave: " + this.transform.position);
        if (opcionesJugador != null)
        {
            if (opcionesJugador.movimientoActivado && IsServer)
            {
                //Debug.Log("VELOCIDAD: " + velocity);

                // Si hay dirección de movimiento, aceleramos
                if (direccionMovimiento != Vector3.zero)
                {
                    //Debug.Log("ACELERANDO");

                    // Descomponer la dirección de movimiento en componentes x y z
                    Vector3 forwardDirection = transform.forward.normalized;
                    Vector3 rightDirection = transform.right.normalized;

                    // Acelerar en cada componente x y z basado en la dirección
                    velocity.x += acceleration * Time.deltaTime * forwardDirection.x;
                    velocity.z += acceleration * Time.deltaTime * forwardDirection.z;

                    // Limitar la velocidad a maxSpeed en cada componente
                    velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
                    velocity.z = Mathf.Clamp(velocity.z, -maxSpeed, maxSpeed);
                }
                else
                {
                    //Debug.Log("DECELERANDO");

                    // Aplicar desaceleración en cada componente para que se acerque a 0
                    if (Mathf.Abs(velocity.x) > 0.01f)
                    {
                        velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
                    }
                    else
                    {
                        velocity.x = 0; // Detenemos completamente si es muy baja
                    }

                    if (Mathf.Abs(velocity.z) > 0.01f)
                    {
                        velocity.z = Mathf.MoveTowards(velocity.z, 0, deceleration * Time.deltaTime);
                    }
                    else
                    {
                        velocity.z = 0; // Detenemos completamente si es muy baja
                    }
                }

                // Aplicar la velocidad calculada a la posición del objeto
                transform.position += velocity * Time.deltaTime;
            }
        }
    }

    public void Move()
    {
        direccionMovimiento = Vector3.forward; // Apuntar hacia adelante
    }

    public void Rotate(float input)
    {
        transform.Rotate(0, input * rotationSpeed * Time.deltaTime, 0);
    }

    public void Stop()
    {
        direccionMovimiento = Vector3.zero;
    }

    public void SetToSpawn(GameObject spawnPoint)
    {
        if (IsServer)
        {
            //Debug.Log("Set To Spawn: " + spawnPoint.transform.position);
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
            velocity = Vector3.zero;
        }
    }

    public void AssignShip(GameObject ship)
    {
        playerShip = ship.GetComponent<PlayerShip>();


        // Eliminamos cualquier CapsuleCollider existente en el padre
        if (collider != null)
        {
            Destroy(collider);
        }

        // Obtenemos el CapsuleCollider del hijo, asumiendo que siempre estará presente
        CapsuleCollider colliderHijo = ship.GetComponent<CapsuleCollider>();
        if (colliderHijo != null)
        {
            // Agregamos un nuevo CapsuleCollider en el padre y copiamos las propiedades del hijo
            collider = gameObject.AddComponent<CapsuleCollider>();
            collider.center = colliderHijo.center;
            collider.radius = colliderHijo.radius;
            collider.height = colliderHijo.height;
            collider.direction = colliderHijo.direction;

            // Desactivamos el CapsuleCollider en el hijo para evitar duplicidad en colisiones
            colliderHijo.enabled = false;

            Debug.Log("CapsuleCollider del hijo copiado al padre y desactivado en el hijo.");
        }
        else
        {
            Debug.LogWarning("La nave asignada no tiene un CapsuleCollider.");
        }
    }

    public void AssignMainCamera()
    {
        CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();
        if (VC != null)
        {
            VC.Follow = CameraPosition;
            VC.LookAt = this.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró la cámara virtual Cinemachine.");
        }
    }
}