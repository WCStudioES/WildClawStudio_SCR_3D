using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class AreaDmg : MonoBehaviour, IProyectil
{
    public int dmg; // Daño por tick del efecto
    public float timeOfEffect;  //Tiempo que dura el efecto en pantalla
    public int ticksPerSecond; // ticks de daño/segundo del efecto
    private float tickTimer = -1;

    public float speed; // Velocidad a la que se mueve el efecto (si se mueve)
    protected Vector3 direction;

    protected CapsuleCollider CuerpoNaveDueña;  // Gameobject con collider de la nave, evita autohit
    protected NetworkedPlayer ControladorNaveDueña; // Controlador de la nave a la que pertenece

    protected bool IsInServidor;

    public void CrearAreaDmg(CapsuleCollider pCuerpoNaveDueña, NetworkedPlayer pDmgDealer, bool pIsInServidor)
    {
        Debug.Log("Explosion creada");
        ControladorNaveDueña = pDmgDealer;
        CuerpoNaveDueña = pCuerpoNaveDueña;
        IsInServidor = pIsInServidor;

        Destroy(gameObject, timeOfEffect);
    }

    public abstract void OnHit(IDamageable target, NetworkedPlayer dmgDealer);

    protected void OnTriggerEnter(Collider other)
    {
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
                tickTimer = 0;
            }
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (ticksPerSecond > 0)
        {
            if (other.gameObject != CuerpoNaveDueña.gameObject && CuerpoNaveDueña != null && tickTimer > 1/ticksPerSecond)
            {
                //Al colisionar comprueba si el otro ente puede recibir daño
                IDamageable target = other.GetComponentInParent<IDamageable>();
                Debug.Log("Dmg over time is being done");

                //Si puede hacer daño, hace daño en el servidor
                if (target != null && IsInServidor)
                {
                    OnHit(target, ControladorNaveDueña);
                    tickTimer = 0;
                }
            }
        }
    }

    private void Update()
    {
        if(tickTimer != -1)
        {
            tickTimer += Time.deltaTime;
        }
    }

    public void Launch(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }
}
