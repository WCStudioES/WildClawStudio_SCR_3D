using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public int dmg = 20; // Daño que inflige el proyectil
    public float speed = 10f; // Velocidad del proyectil
    private Vector3 direction;

    // Llamado para inicializar la dirección del proyectil
    public void Inicializar(Vector3 direccionInicial)
    {
        direction = direccionInicial;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Colisiona
    }
}
