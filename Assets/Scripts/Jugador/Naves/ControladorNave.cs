using System.Collections;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ControladorNave : NetworkBehaviour
{
    [SerializeField] private Transform CameraPositionInGame;
    [SerializeField] private Transform CameraPositionCustomization;
    public Transform actualCamera;

    public enum CameraType{
        InGame,
        Customization
    };

    private Vector3 direccionMovimiento = Vector3.zero; // Dirección del movimiento (hacia adelante)
    public Vector3 velocity = Vector3.zero;

    public float rotationSpeed = 100f; // Velocidad de rotación
    public float acceleration = 5f; // Aceleración
    public float deceleration = 50f; // Desaceleración (fricción)
    private float reductionMultiplier = 0.025f; // Controla la intensidad de la reducción
    public float maxSpeed = 10f; // Velocidad máxima alcanzable

    [SerializeField] public OpcionesJugador opcionesJugador;
    [SerializeField] public PlayerShip playerShip;
    public CapsuleCollider colliderNave;

    void Start()
    {
        if (IsOwner)
        {
            opcionesJugador = GetComponentInParent<OpcionesJugador>();

            actualCamera = CameraPositionCustomization;
            //AssignMainCamera();
        }
    }

    void Update()
    {
        if (opcionesJugador != null && opcionesJugador.movimientoActivado && IsServer)
        {
            // Si hay dirección de movimiento, acelerar
            if (direccionMovimiento != Vector3.zero)
            {
                Vector3 forwardDirection = transform.forward.normalized;

                // Aumenta la velocidad en la dirección de movimiento
                velocity += forwardDirection * acceleration * Time.deltaTime;

                // Limitar la magnitud de la velocidad a maxSpeed
                if (velocity.magnitude > maxSpeed)
                {
                    velocity = velocity.normalized * maxSpeed;
                }
            }
            else
            {
                // Decelerar para detenerse si no hay input de movimiento
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * Time.deltaTime);
            }

            // Aplicar la velocidad calculada a la posición del objeto
            transform.position += velocity * Time.deltaTime;
        }
    }


    public void Move()
    {
        direccionMovimiento = Vector3.forward; // Apuntar hacia adelante
    }

    public void Rotate(float input)
    {
        transform.Rotate(0, input * rotationSpeed * Time.deltaTime, 0);

        if (velocity.magnitude > 0.01f)
        {
            float angle = Vector3.Angle(transform.forward, velocity.normalized);

            float reductionFactor = Mathf.Clamp01(1 - (angle / 180f));

            // Multiplica el factor de reducción para hacer que el efecto sea más o menos fuerte
            velocity *= Mathf.Lerp(1.0f, reductionFactor, reductionMultiplier);
        }
    }

    public void Stop()
    {
        direccionMovimiento = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(velocity.magnitude > maxSpeed/4 && collision.gameObject.GetComponent<IProyectil>() == null)
        {
            velocity.x = -velocity.x / 2;
            velocity.y = -velocity.y / 2;
            
            //Daño por impacto, Ravager no posee por su pasiva
            if (playerShip is not NaveRavager)
            {
                Debug.Log("Daño por choque" + opcionesJugador.controladorDelJugador);
                opcionesJugador.controladorDelJugador.GetDamage(20, opcionesJugador.controladorDelJugador);
            }
        }
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
        if (colliderNave != null)
        {
            Destroy(colliderNave);
        }

        // Obtenemos el CapsuleCollider del hijo, asumiendo que siempre estará presente
        CapsuleCollider colliderHijo = ship.GetComponent<CapsuleCollider>();
        if (colliderHijo != null)
        {
            // Agregamos un nuevo CapsuleCollider en el padre y copiamos las propiedades del hijo
            colliderNave = gameObject.AddComponent<CapsuleCollider>();
            colliderNave.center = colliderHijo.center;
            colliderNave.radius = colliderHijo.radius;
            colliderNave.height = colliderHijo.height;
            colliderNave.direction = colliderHijo.direction;

            // Desactivamos el CapsuleCollider en el hijo para evitar duplicidad en colisiones
            colliderHijo.enabled = false;

            //Debug.Log("CapsuleCollider del hijo copiado al padre y desactivado en el hijo.");
        }
        else
        {
            Debug.LogWarning("La nave asignada no tiene un CapsuleCollider.");
        }
    }

    public void AssignMainCamera(CameraType cameraType)
    {
        CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();
        Debug.Log(VC.m_Lens.OrthographicSize);
        if (VC != null)
        {
            switch (cameraType)
            {
                case CameraType.InGame:
                    actualCamera = CameraPositionInGame;
                    VC.m_Lens.Orthographic = true;
                    VC.m_Lens.OrthographicSize = 15;
                    break;

                case CameraType.Customization:
                    actualCamera = CameraPositionCustomization;
                    VC.m_Lens.Orthographic = false;
                    break;
            }

            VC.Follow = actualCamera;
            VC.LookAt = this.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró la cámara virtual Cinemachine.");
        }
    }
}