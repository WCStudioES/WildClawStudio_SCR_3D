using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public abstract class AreaDmg : MonoBehaviour, IProyectil
{
    public int dmg; // Daño por tick del efecto
    public float timeOfEffect;  //Tiempo que dura el efecto en pantalla
    public int ticksPerSecond; // ticks de daño/segundo del efecto
    private bool isDestroyed = false; // Bandera para evitar múltiples destrucciones

    protected bool canHit = true;
    protected bool isTicking = false;

    protected List<IDamageable> damageablesInArea = new List<IDamageable>(); // Lista de objetos dañables

    public float speed; // Velocidad a la que se mueve el efecto (si se mueve)
    protected Vector3 direction = Vector3.zero;

    protected CapsuleCollider CuerpoNaveDueña;  // Gameobject con collider de la nave, evita autohit
    protected NetworkedPlayer ControladorNaveDueña; // Controlador de la nave a la que pertenece
    protected Partida partida;

    protected bool IsInServidor;

    public AudioClip aoeSFX;
    public GameObject aoeVFX;
    public VFXPrefab aoeVFXInstance;

    //Crear zona de daño con direccion de avance
    public void CrearAreaDmg(CapsuleCollider pCuerpoNaveDueña, NetworkedPlayer pDmgDealer, bool pIsInServidor, Vector3 pDirection, Partida partidaActual)
    {
        if (pIsInServidor)
        {
            RenderManager.Instance.RegisterNewObject(gameObject);
        }

        ControladorNaveDueña = pDmgDealer;
        CuerpoNaveDueña = pCuerpoNaveDueña;
        IsInServidor = pIsInServidor;
        direction = pDirection;
        partida = partidaActual;

        //Debug.Log("Explosion creada");
        if (aoeSFX != null)
        {
            AudioManager.Instance.PlaySFX(aoeSFX, transform.position);
        }

        if (aoeVFX != null && aoeVFX.GetComponent<VFXPrefab>() != null)
        {
            if (this is FireRing)
            {
                aoeVFXInstance = VFXManager.Instance.SpawnVFX(aoeVFX.GetComponent<VFXPrefab>().type, transform.position, transform.rotation);
            }
            else
            {
                VFXManager.Instance.SpawnVFX(aoeVFX.GetComponent<VFXPrefab>().type, transform.position, transform.rotation);
            }
        }

        if (timeOfEffect > 0)
        {
            DestroyAoE(timeOfEffect);
        }

        StartCoroutine(ApplyAoETicks());
    }
    
    //Crear zona de daño sin direccion de avance
    public void CrearAreaDmg(CapsuleCollider pCuerpoNaveDueña, NetworkedPlayer pDmgDealer, bool pIsInServidor)
    {
        if (pIsInServidor)
        {
            RenderManager.Instance.RegisterNewObject(gameObject);
        }

        ControladorNaveDueña = pDmgDealer;
        CuerpoNaveDueña = pCuerpoNaveDueña;
        IsInServidor = pIsInServidor;

        //Debug.Log("Explosion creada");
        if (aoeSFX != null)
        {
            AudioManager.Instance.PlaySFX(aoeSFX, transform.position);
        }

        if (aoeVFX != null && aoeVFX.GetComponent<VFXPrefab>() != null)
        {
            Debug.Log("Activando vfx AoE");
            if(this is FireRing)
            {
                aoeVFXInstance = VFXManager.Instance.SpawnVFX(aoeVFX.GetComponent<VFXPrefab>().type, transform.position, transform.rotation);
            }
            else
            {
                VFXManager.Instance.SpawnVFX(aoeVFX.GetComponent<VFXPrefab>().type, transform.position, transform.rotation);
            }
        }

        if(timeOfEffect > 0)
        {
            DestroyAoE(timeOfEffect);
        }
    }

    public abstract void OnHit(IDamageable target, NetworkedPlayer dmgDealer);

    //Efectos adicionales que hace en el OnTriggerStay
    protected virtual void AdditionalEffectsOnEnter(IDamageable target) { return; }

    //Efectos adicionales que hace en el OnTriggerStay
    protected virtual void AdditionalEffectsOnStay(IDamageable target) { return; }

    //Efectos adicionales que hace en el OnTriggerStay
    protected virtual void AdditionalEffectsOnExit(IDamageable target) { return; }

    protected void OnTriggerEnter(Collider other)
    {
        if (!IsInServidor) return;

        // Verificar si el objeto tiene el componente IDamageable
        if (!IsChildOfOwner(other.transform))
        {
            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target != null && !damageablesInArea.Contains(target))
            {
                damageablesInArea.Add(target);
                AdditionalEffectsOnEnter(target);
            }
        }
    }

    //protected void OnTriggerStay(Collider other)
    //{
    //    if (!IsInServidor) return;

    //    // Asegurarse de agregar objetos que permanezcan en el área
    //    if (!IsChildOfOwner(other.transform))
    //    {
    //        IDamageable target = other.GetComponentInParent<IDamageable>();
    //        if (target != null && !damageablesInArea.Contains(target))
    //        {
    //            damageablesInArea.Add(target);
    //        }
    //        AdditionalEffectsOnStay(other);

    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (!IsInServidor) return;

        if (!IsChildOfOwner(other.transform))
        {
            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target != null && damageablesInArea.Contains(target))
            {
                damageablesInArea.Remove(target);
                AdditionalEffectsOnExit(target);
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
    private IEnumerator ApplyAoETicks()
    {
        isTicking = true;
        float tickInterval = 1f / ticksPerSecond; // Calcula el intervalo de daño por tick

        while (isTicking)
        {
            foreach (var target in damageablesInArea)
            {
                if (target != null) // Verificar que el objeto siga existiendo
                {
                    OnHit(target, ControladorNaveDueña); // Aplica el daño
                }
            }
            yield return new WaitForSeconds(tickInterval); // Espera antes del próximo tick
        }
    }

    public void FixedUpdate()
    {
        //Comportamiento extra del AoE (Crecer, cambiar dirección, etc...)
        ExtraBehaviour(Time.fixedDeltaTime);

        if(direction != Vector3.zero)
        {
            transform.position += direction * (speed * Time.fixedDeltaTime);
        }

        //if(partida != null && !partida.rondaEnmarcha)
        //{
        //    Debug.Log(partida + ", " + partida.rondaEnmarcha);
        //    DestroyAoE();
        //}
    }
    public virtual void ExtraBehaviour(float tiempo)
    {
        return;
    }

    public virtual void DestroyAoE(float tiempo = 0f)
    {
        if (isDestroyed)
        {
            Debug.LogWarning($"DestroyAoE ya fue llamado. Ignorando llamada en {Time.time}");
            return;
        }
        Debug.Log($"DestroyAoE llamado. Destruyendo en {tiempo} segundos. Tiempo actual: {Time.time}");

        Destroy(gameObject, tiempo);
    }


    private void OnDestroy()
    {
        if(damageablesInArea.Count > 0)
        {
            foreach (var target in damageablesInArea)
            {
                AdditionalEffectsOnExit(target);
                damageablesInArea.Remove(target);
            }
        }

        isDestroyed = true;
        isTicking = false;

        damageablesInArea.Clear();
        StopAllCoroutines();
    }

}
