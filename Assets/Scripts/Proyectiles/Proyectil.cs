using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Proyectil : NetworkBehaviour, IPooleableObject
{
    public int dmg = 20; // Da�o que inflige el proyectil
    public float speed = 10f; // Velocidad del proyectil
    protected Vector3 direction;
    public float cadencia = 1.75f; //Cadencia de disparo de este proyectil
    public int tipo;  //Identificador del tipo de proyectil

    protected GameObject NaveDueña;
    //Variable activacion de IPooleableObject

    public GameObject getDueño()
    {
        return NaveDueña;
    }
    public bool Active
    {
        get;
        set;
    }

    // Llamado para inicializar la direcci�n del proyectil
    public void Inicializar(Vector3 direccionInicial, GameObject jugador)
    {
        direction = direccionInicial;
        NaveDueña = jugador;
        //Debug.Log("Soy de " + jugadorDueño.ToString());
    }

    protected void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        Debug.Log(NaveDueña);
        Debug.Log(other.gameObject != NaveDueña);

        if (other.gameObject != NaveDueña.gameObject && NaveDueña != null)
        {
            //Al colisionar comprueba si el otro ente puede recibir daño
            ICanGetDamage target = other.GetComponentInParent<ICanGetDamage>();
            Debug.Log(target);
            Debug.Log("Entro al if de diferente");
            if (target != null)
            {
                Debug.Log("Entro al if de daño");
                target.GetDamage(dmg);
            }
            DestruirProyectilServer();
        }
    }
    
    protected void DestruirProyectilServer()
    {
        if (NetworkManager.IsServer)
        {
            Debug.Log("Destruir proyectil");
            DestruirProyectilClientRpc();
            gameObject.SetActive(false);
        }
    }
    
    // GESTIONA LA DESTRUCCIÓN DE CADA DISPARO EN LOS CLIENTES
    [ClientRpc]
    protected void DestruirProyectilClientRpc()
    {
        gameObject.SetActive(false);
    }
    

    
    //INTERFAZ IPOOLEABLE OBJECT
    public IPooleableObject Clone()
    {
        return null;
    }

    public void Reset()
    {
        
    }
}
