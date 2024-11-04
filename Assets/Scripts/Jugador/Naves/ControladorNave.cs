using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class ControladorNave : NetworkBehaviour
{
    public CharacterController controller;
    private float rotacion = 0f; // Valor para la rotación sobre el eje Y (izquierda/derecha)
    private Vector3 direccionMovimiento = Vector3.zero; // Dirección del movimiento (hacia adelante)
    [SerializeField] private Transform CameraPosition;

    public float speed = 5f; // Velocidad máxima de movimiento
    public float rotationSpeed = 100f; // Velocidad de rotación
    public float acceleration = 2f; // Aceleración
    public float deceleration = 1f; // Desaceleración (fricción)
    public float maxSpeed = 10f; // Velocidad máxima alcanzable
    public float currentSpeed = 0f; // Velocidad actual

    [SerializeField] private OpcionesJugador opcionesJugador;
    [SerializeField] public PlayerShip playerShip;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (opcionesJugador.movimientoActivado)
        {
            // Aplicar la rotación
            transform.Rotate(0, rotacion * Time.deltaTime, 0);

            // Movimiento con inercia
            if (direccionMovimiento.z != 0)
            {
                // Si hay input, acelera la nave
                currentSpeed += acceleration * Time.deltaTime;
                direccionMovimiento.z = 0;
            }
            else
            {
                // Si no hay input, desacelera la nave lentamente (simula fricción)
                currentSpeed -= deceleration * Time.deltaTime;
            }

            // Limitar la velocidad actual para que no exceda el máximo
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

            // Movimiento hacia adelante en la dirección que mira la nave
            Vector3 movimiento = currentSpeed * Time.deltaTime * transform.forward;
            controller.Move(movimiento);
        }
    }

    public void Move()
    {
        //Debug.Log("Se mueve con: " + this.transform.rotation);
        direccionMovimiento = new Vector3(0f, 0f, 1);
    }

    public void Rotate(float input)
    {
        rotacion = input * rotationSpeed;
    }

    public void Stop()
    {
        direccionMovimiento = Vector3.zero;
        rotacion = 0f;
    }

    public void SetToSpawn(GameObject spawnPoint)
    {
        if (IsServer)
        {
            this.transform.position = spawnPoint.transform.position;
            this.transform.rotation = spawnPoint.transform.rotation;
        }


    }

    public void AssignMainCamera()
    {
        Debug.Log("Busca la cámara");
        CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();
        VC.Follow = CameraPosition;
        VC.LookAt = this.transform;
    }
}
