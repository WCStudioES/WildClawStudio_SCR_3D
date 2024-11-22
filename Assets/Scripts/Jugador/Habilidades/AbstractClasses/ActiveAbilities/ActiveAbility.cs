using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveAbility : Ability
{
    public ActiveType type;
    public int maxResource;
    public enum ActiveType
    {
        MovementBuff,
        ShootProjectile,
        Shield,
        TogglePassive
    }
    
    //Metodo para reiniciar habilidad entre rondas
    public void ResetRonda()
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
                networkedPlayer.UpdateAbilityCDUIClientRpc(actualResQuantity/neededResQuantity);
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

}
