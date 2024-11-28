using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour, IAbility
{
    public string Name;
    public string Description;
    public Sprite Sprite;

    public ResourceType resourceType;
    public float actualResQuantity;
    public float neededResQuantity;

    //Referencia al jugador, usado para restar y sumar vida
    public NetworkedPlayer networkedPlayer;
    
    //[SerializeField] protected UIBoosters uiBoosters;

    public enum ResourceType
    {
        None,
        Energy,
        CoolDown,
        Hp
    }

    //Usado cuando se llama a la habilidad (pulsar tecla o Upadte() si es pasiva)
    public void Execute()
    {
        if (CheckAvailability())
        {
            AbilityExecution();
            
            switch(resourceType)
            {
                case ResourceType.CoolDown:
                    actualResQuantity = 0;
                    break;

                default:
                    //actualResQuantity -= neededResQuantity; 
                    break;
            }
        }
    }

    //La propia ejecuci�n de la habilidad
    public abstract void AbilityExecution();

    //Mira si puede usarse la habilidad
    public abstract bool CheckAvailability();

    // Start is called before the first frame update
    public void Start()
    {
        networkedPlayer= GetComponentInParent<NetworkedPlayer>();
        InitializeVFX();
    }

    protected virtual void InitializeVFX()
    {
        return;
    }
}
