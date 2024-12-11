using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Ability : MonoBehaviour, IAbility
{
    public string Name;
    public string Description;
    public Sprite Sprite;

    public ResourceType resourceType;
    public float actualResQuantity;
    public float neededResQuantity;

    [SerializeField] protected List<VFXPrefab> visualEffects;

    //Referencia al jugador, usado para restar y sumar vida
    public NetworkedPlayer networkedPlayer;
    
    //[SerializeField] protected UIBoosters uiBoosters;

    public enum ResourceType
    {
        None,
        Energy,
        CoolDown,
        Hp,
        RotationSpeed
    }

    //Usado cuando se llama a la habilidad (pulsar tecla o Upadte() si es pasiva)
    public virtual void Execute()
    {
        if (CheckAvailability() && gameObject.activeInHierarchy)
        {
            AbilityExecution();
            switch(resourceType)
            {
                case ResourceType.CoolDown:
                    actualResQuantity = 0;
                    StartCoroutine(CooldownCoroutine());
                    break;

                default:
                    //actualResQuantity -= neededResQuantity; 
                    break;
            }
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        while (actualResQuantity < neededResQuantity)
        {
            actualResQuantity += Time.deltaTime;
            yield return null;  // Esperar un frame
        }
    }


    //La propia ejecuci�n de la habilidad
    public abstract void AbilityExecution();

    //Detiene la ejecución de la habilidad
    public virtual void Stop()
    {
        return;
    }

    //Mira si puede usarse la habilidad
    public abstract bool CheckAvailability();

    // Start is called before the first frame update
    public void Start()
    {
        networkedPlayer = GetComponentInParent<NetworkedPlayer>();
    }

    public virtual void InitializeVFX()
    {
        return;
    }

    public virtual void ToggleVFX(bool active)
    {
        foreach(VFXPrefab effect in visualEffects)
        {
            if(effect != null)
            {
                effect.Toggle(active);
            }
        }
    }
}
