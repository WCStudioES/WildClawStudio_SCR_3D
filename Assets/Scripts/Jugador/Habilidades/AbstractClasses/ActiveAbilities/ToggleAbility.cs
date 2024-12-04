using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleAbility : ActiveAbility
{
    protected bool active = false;
    protected float timerForToggle = 1f;
    protected bool timerActive;

    private void Awake()
    {
        type = ActiveType.ToggleActive;
        timerActive = true;
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
        //Debug.Log("Toogle llamada");
        return true;
    }

    public void Toggle()
    {
        if (timerActive)
        {
            timerActive = false;
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

            StartCoroutine("ReducetimerToggle");
        }
    }

    public bool GetActive()
    {
        return active;
    }

    IEnumerator ReducetimerToggle()
    {
        Debug.Log("Toogle reducetime");
        yield return new WaitForSeconds(timerForToggle);
        timerActive = true;
    }
}
