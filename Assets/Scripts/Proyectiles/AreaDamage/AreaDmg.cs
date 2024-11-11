using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class AreaDmg : MonoBehaviour, IProyectil
{
    public int dmg; // Da�o que inflige el proyectil
    public float timeOfEffect;
    public float speed; // Velocidad del proyectil
    protected Vector3 direction;
    public float cadencia; // Cadencia de disparo de este proyectil

    protected CapsuleCollider CuerpoNaveDueña;  // Gameobject con collider de la nave, evita autohit
    protected NetworkedPlayer ControladorNaveDueña; // Controlador de la nave a la que pertenece

    protected bool IsInServidor;

    public void CrearAreaDmg(int daño, float t, CapsuleCollider pCuerpoNaveDueña, NetworkedPlayer pDmgDealer, bool pIsInServidor)
    {
        Debug.Log("Explosion creada");
        dmg = daño;
        timeOfEffect = t;
        ControladorNaveDueña = pDmgDealer;
        CuerpoNaveDueña = pCuerpoNaveDueña;
        IsInServidor = pIsInServidor;
    }

    public abstract void OnHit(IDamageable target, NetworkedPlayer dmgDealer);

    protected void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
        //Debug.Log(CuerpoNaveDueña);
        //Debug.Log(other.gameObject != CuerpoNaveDueña);

        //Si el otro gameobject no es el mismo, compueba si puede hacer daño
        if (other.gameObject != CuerpoNaveDueña.gameObject && CuerpoNaveDueña != null)
        {
            //Al colisionar comprueba si el otro ente puede recibir daño
            IDamageable target = other.GetComponentInParent<IDamageable>();
            Debug.Log(target);

            //Si puede hacer daño, hace daño en el servidor
            if (target != null && IsInServidor)
            {
                OnHit(target, ControladorNaveDueña);
            }
            Destroy(gameObject, timeOfEffect);
        }
    }

    public void Launch(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }
}
