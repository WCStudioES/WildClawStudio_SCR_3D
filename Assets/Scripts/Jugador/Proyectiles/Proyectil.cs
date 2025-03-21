using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro.Examples;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class Proyectil : MonoBehaviour, IProyectil, IPooleableObject
{
    public Sprite sprite;
    public string weaponName;
    public string description;
    public string Upgradedescription;
    public bool mostrarEnSeleccion;

    public int dmg; // Da�o que inflige el proyectil
    public float speed; // Velocidad del proyectil
    protected Vector3 direction;
    public float cadencia; // Cadencia de disparo de este proyectil

    protected CapsuleCollider CuerpoNaveDuena;  // Gameobject con collider de la nave, evita autohit
    protected NetworkedPlayer ControladorNaveDueña; // Controlador de la nave a la que pertenece

    protected bool IsEnServidor;
    public Type type;

    public AudioClip projectileSFX;

    public GameObject projectileVFX;
    public GameObject trailVFX;

    VisualEffect trailInstanceVFX;

    //Variable activacion de IPooleableObject
    public bool Active
    {
        get;
        set;
    }

    //Tipo de proyectil (Se lanzan simultáneamente 1 o 2)
    public enum Type
    {
        Simple,
        Double,
        Triple
    }

    // Llamado para inicializar la direcci�n del proyectil
    public void Inicializar(Vector3 direccionInicial, CapsuleCollider hitboxNave, NetworkedPlayer controladorDelJugador ,bool esEservidor)
    {
        if (esEservidor)
        {
            RenderManager.Instance.RegisterNewObject(gameObject);
        }

        //Debug.Log("Proyectil Inicializado");
        direction = direccionInicial;
        CuerpoNaveDuena = hitboxNave;
        IsEnServidor = esEservidor;
        ControladorNaveDueña = controladorDelJugador;

        if(dmg > 0)
        {
            //Debug.Log("Daño" + dmg);
            dmg += controladorDelJugador.dmgBalance.Value * dmg / 100;
        }

        if (!esEservidor)
        {
            if (projectileSFX != null)
            {
                AudioManager.Instance.PlaySFX(projectileSFX, transform.position);
            }

            InitializeProjectileVFX(transform.position, transform.rotation, controladorDelJugador.nave.transform);
        }

        //Debug.Log("Soy de " + jugadorDueño.ToString());
    }

    public void Launch(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public abstract void OnHit(IDamageable target, NetworkedPlayer dmgDealer);

    protected void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        //Si el otro gameobject no es el mismo, compueba si puede hacer daño
        if (!IsChildOfOwner(other.transform))
        {
            //Al colisionar comprueba si el otro ente puede recibir daño
            IDamageable target = other.GetComponentInParent<IDamageable>();
            GameObject wall = other.gameObject;
            //Debug.Log(target);

            //Si puede hacer daño, hace daño en el servidor
            if (target != null || wall.tag == "Wall")
            {
                if (ControladorNaveDueña.cuerpoNave.GetComponent<PlayerShip>().passiveAbility.type == PassiveAbility.PassiveType.OnHit)
                {
                    OnHit(target, ControladorNaveDueña);
                    ControladorNaveDueña.cuerpoNave.GetComponent<OnHitPassive>().OnHit(other.gameObject);
                }
                else
                {
                    OnHit(target, ControladorNaveDueña);
                }

                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponentInChildren<Renderer>());

                //DESACTIVA EL EFECTO DEL TRAIL (Porque está parented)
                Destroy(GetComponentInChildren<VisualEffect>());
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

    //VFX
    public void InitializeProjectileVFX(Vector3 position, Quaternion rotation, Transform parent)
    {
        if (projectileVFX != null && projectileVFX.GetComponent<VFXPrefab>() != null)
        {
            Debug.Log("Activando vfx MuzzleFlash");
            VFXManager.Instance.SpawnVFX(projectileVFX.GetComponent<VFXPrefab>().type, position, rotation, parent);
        }
        
        if(trailVFX != null)
        {
            Debug.Log("Iniciando VFX del trail");
            GameObject vfxObject = Instantiate(trailVFX, position, rotation, transform);
            trailInstanceVFX = vfxObject.GetComponent<VisualEffect>();
            trailInstanceVFX.enabled = true;
        }
    }

    //INTERFAZ IPOOLEABLE OBJECT
    public IPooleableObject Clone()
    {
        return null;
    }

    private void OnDestroy()
    {
        //if (GetComponentInChildren<VFXPrefab>()) ; 
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}
