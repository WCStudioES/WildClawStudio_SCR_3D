using Jugador.Habilidades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveAbility : Ability
{
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

            default:
                return false;
        }
    }

    public void Update()
    {
        switch (resourceType)
        {
            case ResourceType.Energy:
                if (actualResQuantity < neededResQuantity)
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

            default:
                break;
        }
    }

}
