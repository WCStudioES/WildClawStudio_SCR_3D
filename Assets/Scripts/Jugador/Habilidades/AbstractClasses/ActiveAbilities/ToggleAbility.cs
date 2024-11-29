using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleAbility : ActiveAbility
{
    protected bool active = false;

    private void Awake()
    {
        type = ActiveType.TogglePassive;
    }
    public override void Execute()
    {
        if (CheckAvailability())
        {
            Toggle();
            switch (resourceType)
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
    public override bool CheckAvailability()
    {
        return true;
    }

    public void Toggle()
    {
        active = !active;
        Debug.Log("Habilidad Toggle ejecutada");
        if (active)
        {
            networkedPlayer.ToggleAbilityVFXClientRpc(true);
            AbilityExecution();
        }
        //StartCoroutine("DurationCounter");
        //StartCoroutine("CheckEnergy");
        else
        {
            networkedPlayer.ToggleAbilityVFXClientRpc(false);
            Stop();
        }
    }

    public bool GetActive()
    {
        return active;
    }
}
