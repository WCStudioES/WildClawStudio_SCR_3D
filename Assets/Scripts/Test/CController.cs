using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Windows;

public class CController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direccionMovimiento = Vector3.zero; // Direcci�n del movimiento (hacia adelante)
    private float rotacion = 0f; // Valor para la rotaci�n sobre el eje Y (izquierda/derecha)
    [SerializeField] private Transform CameraPosition;

    public float speed = 5f; // Velocidad m�xima de movimiento
    public float rotationSpeed = 100f; // Velocidad de rotaci�n
    public float acceleration = 2f; // Aceleraci�n
    public float deceleration = 1f; // Desaceleraci�n (fricci�n)
    public float maxSpeed = 10f; // Velocidad m�xima alcanzable
    public float currentSpeed = 0f; // Velocidad actual

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Aplicar la rotaci�n
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
            // Si no hay input, desacelera la nave lentamente (simula fricci�n)
            currentSpeed -= deceleration * Time.deltaTime;
        }

        // Limitar la velocidad actual para que no exceda el m�ximo
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // Movimiento hacia adelante en la direcci�n que mira la nave
        Vector3 movimiento = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(movimiento);
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

    public void SetToSpawn()
    {
        this.transform.position = GameObject.Find("PuntoDeSpawn").transform.position;
    }

    public void AssignMainCamera()
    {
        Debug.Log("Busca la c�mara");
        CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();
        VC.Follow = CameraPosition;
        VC.LookAt = this.transform;
    }
}
