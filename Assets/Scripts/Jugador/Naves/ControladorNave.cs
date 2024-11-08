using System.Collections;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ControladorNave : NetworkBehaviour
{
    private Vector3 direccionMovimiento = Vector3.zero; // Dirección del movimiento (hacia adelante)
    [SerializeField] private Transform CameraPosition;

    public float rotationSpeed = 100f; // Velocidad de rotación
    public float acceleration = 5f; // Aceleración
    public float deceleration = 50f; // Desaceleración (fricción)
    public float maxSpeed = 10f; // Velocidad máxima alcanzable
    public float currentSpeed = 0f; // Velocidad actual
    private float targetRotation = 0f; // Rotación deseada en grados
    public Vector3 velocity = new Vector3(0f, 0f, 0f);

    [SerializeField] private OpcionesJugador opcionesJugador;
    [SerializeField] public PlayerShip playerShip;

    void Start()
    {
        opcionesJugador = GetComponentInParent<OpcionesJugador>();
        AssignMainCamera();
    }

    void Update()
    {
        if (opcionesJugador != null)
        {
            if (opcionesJugador.movimientoActivado && IsServer)
            {
                // Aplicar rotación suavemente usando Slerp para interpolación suave
                Quaternion targetRotationQuaternion = Quaternion.Euler(0, targetRotation, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationQuaternion, rotationSpeed * Time.fixedDeltaTime);

                Debug.Log("DIRECCION: " + direccionMovimiento);
                // Movimiento con aceleración y desaceleración
                if (direccionMovimiento.z > 0)
                {
                    // Acelerar si hay input, pero limitamos a maxSpeed
                    velocity += acceleration * Time.fixedDeltaTime * transform.forward;
                    velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
                }
                else
                {
                    // Desacelerar si no hay input, pero no permitimos que la velocidad sea negativa
                    velocity -= deceleration * Time.fixedDeltaTime * transform.forward;
                    if (velocity.magnitude < 0.01f)
                    {
                        velocity = Vector3.zero; // Detenemos por completo si es muy baja
                    }
                }

                // Aplicar la velocidad calculada en la dirección hacia adelante
                transform.position += velocity * Time.fixedDeltaTime;
            }
        }
    }

    public void Move()
    {
        direccionMovimiento = Vector3.forward; // Apuntar hacia adelante
    }

    public void Rotate(float input)
    {
        targetRotation = transform.eulerAngles.y + input * rotationSpeed * Time.deltaTime;
    }

    public void Stop()
    {
        direccionMovimiento = Vector3.zero;
    }

    public void SetToSpawn(GameObject spawnPoint)
    {
        if (IsServer)
        {
            Debug.Log("Set To Spawn: " + spawnPoint.transform.position);
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
            currentSpeed = 0f;
            //rb.velocity = Vector3.zero; // Asegurar que el Rigidbody esté detenido al reaparecer
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