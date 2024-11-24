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
    //private float tickTimer = -1;
    private bool canHit = true;
    private bool isResetting = false;

    public float speed; // Velocidad a la que se mueve el efecto (si se mueve)
    protected Vector3 direction;

    protected CapsuleCollider CuerpoNaveDueña;  // Gameobject con collider de la nave, evita autohit
    protected NetworkedPlayer ControladorNaveDueña; // Controlador de la nave a la que pertenece
    protected Partida partida;

    protected bool IsInServidor;
    public AudioClip aoeSFX;

    //Crear zona de daño con direccion de avance
    public void CrearAreaDmg(CapsuleCollider pCuerpoNaveDueña, NetworkedPlayer pDmgDealer, bool pIsInServidor, Vector3 pDirection, Partida partidaActual)
    {
        Debug.Log("Explosion creada");
        ControladorNaveDueña = pDmgDealer;
        CuerpoNaveDueña = pCuerpoNaveDueña;
        IsInServidor = pIsInServidor;
        direction = pDirection;
        partida = partidaActual;

        Destroy(gameObject, timeOfEffect);
    }
    
    //Crear zona de daño sin direccion de avance
    public void CrearAreaDmg(CapsuleCollider pCuerpoNaveDueña, NetworkedPlayer pDmgDealer, bool pIsInServidor)
    {
        Debug.Log("Explosion creada"); 
        AudioManager.Instance.PlaySFX(aoeSFX);
        ControladorNaveDueña = pDmgDealer;
        CuerpoNaveDueña = pCuerpoNaveDueña;
        IsInServidor = pIsInServidor;

        Destroy(gameObject, timeOfEffect);
    }

    public abstract void OnHit(IDamageable target, NetworkedPlayer dmgDealer);

    protected void OnTriggerEnter(Collider other)
    {
        //Si el otro gameobject no es el mismo, compueba si puede hacer daño
        if (!IsChildOfOwner(other.transform) && canHit)
        {
            //Al colisionar comprueba si el otro ente puede recibir daño
            IDamageable target = other.GetComponentInParent<IDamageable>();
            Debug.Log(target);

            //Si puede hacer daño, hace daño en el servidor
            if (target != null && IsInServidor)
            {
                canHit = false;
                OnHit(target, ControladorNaveDueña);
                //tickTimer = 0;
                StartCoroutine(ResetHitCooldown());
            }
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (canHit && !isResetting) // Asegurarse de no estar ya en proceso de reinicio
        {
            canHit = false;

            if (!IsChildOfOwner(other.transform))
            {
                // Al colisionar comprueba si el otro ente puede recibir daño
                IDamageable target = other.GetComponentInParent<IDamageable>();
                Debug.Log("Dmg over time is being done");

                // Si puede hacer daño, lo hace en el servidor
                if (target != null && IsInServidor)
                {
                    OnHit(target, ControladorNaveDueña);
                    StartCoroutine(ResetHitCooldown());
                }
            }
        }
    }

    public bool IsChildOfOwner(Transform target)
    {
        if (ControladorNaveDueña == null) return false;

        // Comprobamos si el target es parte del NetworkPlayer o su jerarquía
        Transform current = target;
        while (current != null)
        {
            if (current == ControladorNaveDueña.transform)
            {
                return true; // Es hijo del NetworkPlayer que disparó
            }
            current = current.parent; // Subimos en la jerarquía
        }

        return false;
    }

    // Corrutina para manejar el cooldown del golpe
    private IEnumerator ResetHitCooldown()
    {
        isResetting = true; // Bloquea nuevas ejecuciones mientras el cooldown está activo
        float time = 1/ (float)ticksPerSecond;
        Debug.Log(time);
        yield return new WaitForSeconds(time);
        canHit = true;
        isResetting = false; // Libera el bloqueo
    }
    
    //private void Update()
    //{
    //    if(tickTimer != -1)
    //    {
    //        tickTimer += Time.deltaTime;
    //    }
    //}

    public void FixedUpdate()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;
        if(partida != null && partida.rondaEnmarcha)
            Destroy(gameObject);
    }
}
