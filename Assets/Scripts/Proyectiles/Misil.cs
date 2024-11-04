using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Proyectiles;
using Unity.Netcode;
using UnityEngine;

public class Misil : Proyectil
{
    [SerializeField] private GameObject explosion; //Prefab de explosion para instanciar al petar
    private bool activo = false; //Variable apra ver si el misil ha explotado ya
    new protected void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject != CuerpoNaveDueña && CuerpoNaveDueña != null && activo == false)
        {
            Debug.Log("Dentro del if diferente");
            activo = true;
            DestruirProyectil();
        }
    }
    
    protected void DestruirProyectil()
    {
        Debug.Log("Entro a destruir proyectil papa");
        //if (NetworkManager.IsServer)
        {
            Debug.Log("Entro a la explosion Server");
            //Creo una instancia de explosion
            var explosionObject = NetworkManager.Instantiate(explosion, transform.position, Quaternion.identity);
            Explosion explosionScript = explosionObject.GetComponent<Explosion>();
            //Cargamos la explosion
            explosionScript.Crear(dmg);
            //Manda destruir al cliente
            //DestruirProyectilClientRpc();
            NetworkManager.Destroy(gameObject, 0.1f);
        }
    }
    
    // GESTIONA LA REPLICACIÓN DE CADA EXPLOSION EN LOS CLIENTES
    /*[ClientRpc]
    protected void DestruirProyectilClientRpc() 
    {
        Debug.Log("Entro a la explosion Cliente");
        var explosionObject = NetworkManager.Instantiate(explosion, transform.position, Quaternion.identity);
        Explosion explosionScript = explosionObject.GetComponent<Explosion>();
        explosionScript.Crear(dmg);
        NetworkManager.Destroy(gameObject, 0.1f);
    }*/
        
}
