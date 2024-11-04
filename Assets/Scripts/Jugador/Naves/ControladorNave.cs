using System.Collections;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class ControladorNave : NetworkBehaviour
{
    private Rigidbody rb;
    private Vector3 direccionMovimiento = Vector3.zero; // Dirección del movimiento (hacia adelante)
    [SerializeField] private Transform CameraPosition;

    public float rotationSpeed = 100f; // Velocidad de rotación
    public float acceleration = 5f; // Aceleración
    public float deceleration = 50f; // Desaceleración (fricción)
    public float maxSpeed = 10f; // Velocidad máxima alcanzable
    public float currentSpeed = 0f; // Velocidad actual
    private float targetRotation = 0f; // Rotación deseada en grados

    [SerializeField] private OpcionesJugador opcionesJugador;
    [SerializeField] public PlayerShip playerShip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody en el objeto.");
            return;
        }

        // Configurar el Rigidbody para evitar rotación automática
        rb.freezeRotation = true;
        rb.isKinematic = false;  // Asegúrate de que el Rigidbody no sea cinemático
    }

    void FixedUpdate()
    {
        if (opcionesJugador.movimientoActivado)
        {
            // Aplicar rotación suavemente usando Slerp para interpolación suave
            Quaternion targetRotationQuaternion = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationQuaternion, rotationSpeed * Time.fixedDeltaTime);

            // Movimiento con aceleración y desaceleración
            if (direccionMovimiento.z > 0)
            {
                // Acelerar si hay input
                currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
            }
            else
            {
                // Desacelerar si no hay input
                currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.fixedDeltaTime, 0f);
            }

            // Aplicar la velocidad calculada en la dirección hacia adelante
            Vector3 movimiento = transform.forward * currentSpeed;
            rb.MovePosition(rb.position + movimiento * Time.fixedDeltaTime);
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
            rb.position = spawnPoint.transform.position;
            rb.rotation = spawnPoint.transform.rotation;
            currentSpeed = 0f;
            rb.velocity = Vector3.zero; // Asegurar que el Rigidbody esté detenido al reaparecer
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
