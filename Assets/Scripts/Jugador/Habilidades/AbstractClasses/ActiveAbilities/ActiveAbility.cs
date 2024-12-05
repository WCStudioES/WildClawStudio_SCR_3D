using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveAbility : Ability
{
    public ActiveType type;
    public int maxResource;
    protected bool isUpgraded = false;
    public enum ActiveType
    {
        MovementBuff,
        ShootProjectile,
        Shield,
        ToggleActive,
        Dash
    }
    
    //Metodo para reiniciar habilidad entre rondas
    public virtual void ResetRonda()
    {
        switch (resourceType)
        {
            case ResourceType.None:
                break;

            case ResourceType.Energy:
                actualResQuantity = maxResource;
                networkedPlayer.UpdateAbilityUIClientRpc(actualResQuantity/maxResource);
                break;

            case ResourceType.CoolDown:
                actualResQuantity = neededResQuantity;
                networkedPlayer.UpdateCDAbilityUIClientRpc(actualResQuantity/neededResQuantity, isUpgraded);
                break;

            case ResourceType.Hp:
                break;

            default:
                break;
        }
    }

    public override bool CheckAvailability()
    {
        switch (resourceType)
        {
            case ResourceType.None:
                return true;

            case ResourceType.Energy:
                if(actualResQuantity < neededResQuantity)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            case ResourceType.CoolDown:
                if (actualResQuantity < neededResQuantity)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            case ResourceType.Hp:
                if(actualResQuantity < neededResQuantity)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            default:
                return false;
        }
    }

    public void Update()
    {
        switch (resourceType)
        {
            //Ravager lo lleva solo, no quiero petar la habilidad con stats que otros no usan
            case ResourceType.Energy:
                if (actualResQuantity < 10)
                {
                    actualResQuantity += Time.deltaTime * 5;
                }
                break;

            case ResourceType.CoolDown:
                if (actualResQuantity < neededResQuantity)
                {
                    actualResQuantity += Time.deltaTime;
                }
                break;

            case ResourceType.Hp:
                actualResQuantity = networkedPlayer.actualHealth.Value;
                break;

            default:
                break;
        }
    }

    public virtual void UpgradeAbility()
    {
        isUpgraded = true;
        networkedPlayer.UpdateAbilityUIClientRpc(Color.magenta);
        Debug.Log(networkedPlayer.userName + " upgraded Ability");
    }
    
    public virtual void ResetPartida()
    {
        Debug.Log(networkedPlayer.userName + " reseted Ability");
        networkedPlayer.UpdateAbilityUIClientRpc(Color.white);
        isUpgraded = false;
    }
    
}
