using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class RavagerActive : ToggleAbility
{
    private float duracionTotal = 3; // Tiempo total que se puede usar la habilidad
    private float duracionActual = 0; // Tiempo de uso de la habilidad

    public float maxSpeedBuff = 20;
    public float maxSpeedBuffUpgraded = 20;
    public float accelerationBuff = 30;

    public int energyRecharge;  //Energia recargada por segundo
    public int energyRechargeUpgraded;  //Energia recargada por segundo

    private List<VisualEffect> activeFireVFX = new List<VisualEffect>();
    [SerializeField] private List<Transform> extraFires;
    [SerializeField] private VisualEffect firePropulsionVFX;

    //Metodo para activar habilidad: impulso de velocidad durante unos segundos
    public override void AbilityExecution()
    {      
        if (!networkedPlayer.IsServer) return;
        Debug.Log("Habilidad Ravager ejecutada");
        //uiBoosters.UpdateActiveImage(neededResQuantity);

        if (!isUpgraded)
            networkedPlayer.nave.maxSpeed += maxSpeedBuff;
        else
        {
            Debug.Log("Habilidad Ravager mejorada ejecutada");
            networkedPlayer.nave.maxSpeed += maxSpeedBuffUpgraded;
        }
        
        networkedPlayer.nave.acceleration += accelerationBuff;
    }

    //Metodo para reiniciar habilidad entre ronda
    public override void ResetRonda()
    {
        if(!networkedPlayer.IsServer) return;

        if (active)
        {
            Toggle();
        }

        actualResQuantity = maxResource;

        if(!isUpgraded)
            networkedPlayer.UpdateAbilityUIClientRpc(neededResQuantity / maxResource, Color.white);
        else
        {
            networkedPlayer.UpdateAbilityUIClientRpc(neededResQuantity / maxResource, Color.magenta);
            Debug.Log("Habilidad Ravager mejorada ejecutada");
        }
        //StartCoroutine("UiUpdater");
    }

    public override bool CheckAvailability()
    {
        if (actualResQuantity < neededResQuantity && !active)
        {
            return false;
        }
        else if (actualResQuantity >= neededResQuantity)
        {
            return true;
        }
        else
        {
            return true;
        }
    }

    public new void Update()
    {
        if (!networkedPlayer.IsServer) return;

        Color color;

        if (!active && actualResQuantity < maxResource)
        {
            if(!isUpgraded)
                actualResQuantity += energyRecharge * Time.deltaTime;
            else
                actualResQuantity += energyRechargeUpgraded * Time.deltaTime;
        }
        else if (active)
        {
            Debug.Log("Habilidad activada");
            if (actualResQuantity <= 0)
            {
                Toggle();
            }
            actualResQuantity -= neededResQuantity * Time.deltaTime;
        }

        if (!active && actualResQuantity < neededResQuantity)
        {
            color = Color.red;
        }
        else if (active)
        {
            color = Color.yellow;
        }
        else
        {
            if(!isUpgraded)
                color = Color.white;
            else
                color = Color.magenta;
        }

        networkedPlayer.UpdateAbilityUIClientRpc(actualResQuantity / maxResource, color);
    }

    
    //Metodo que acabar la habilidad y restaura los valores a su modo inicial
    public override void Stop()
    {
        if (!networkedPlayer.IsServer) return; // Solo el servidor termina la habilidad

        Debug.Log("Habilidad Ravager terminada");
        if (!isUpgraded)
        {
            networkedPlayer.nave.maxSpeed -= maxSpeedBuff;
            networkedPlayer.UpdateAbilityUIClientRpc(actualResQuantity / maxResource, Color.white);
        }
        else
        {
            networkedPlayer.nave.maxSpeed -= maxSpeedBuffUpgraded;
            networkedPlayer.UpdateAbilityUIClientRpc(actualResQuantity / maxResource, Color.magenta);
        }
        
        networkedPlayer.nave.acceleration -= accelerationBuff;
        duracionActual = 0;
    }

    //Metodo para aumentar elcontador de duración de la habilidad
    IEnumerator DurationCounter()
    {
        while (duracionActual <= duracionTotal && active)
        {
            yield return new WaitForSeconds(1f); // Esperar un segundo para restar

            duracionActual++;

            Debug.Log("Duracion segundos Ravager: " + actualResQuantity);
        }
        Stop();
    }

    IEnumerator UiUpdater()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f); // Esperar un 0.1 segundo para actualizar
            networkedPlayer.UpdateAbilityUIClientRpc(actualResQuantity / maxResource);
        }
    }

    IEnumerator CheckEnergy()
    {
        while (active)
        {
            yield return new WaitForSeconds(1f); // Esperar un 0.1 segudnos para comprobar

            actualResQuantity -= neededResQuantity;
            if (actualResQuantity < neededResQuantity)
            {
                Stop();
            }
        }
    }

    //VFX
    public override void InitializeVFX()
    {
        foreach (Transform spawn in extraFires)
        {
            if (firePropulsionVFX != null)
            {

                VisualEffect newVFX = Instantiate(firePropulsionVFX, spawn.position, Quaternion.identity, spawn);
                newVFX.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                visualEffects.Add(newVFX);
            }
        }
        ToggleVFX(false);
    }

}
