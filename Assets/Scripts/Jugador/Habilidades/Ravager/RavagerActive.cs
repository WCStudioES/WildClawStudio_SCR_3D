using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class RavagerActive: MovementAbility    
{
    private bool isActivated = false;   //Booleano que indica si está activado
        
    private float duracionTotal= 3; // Tiempo total que se puede usar la habilidad
    private float duracionActual = 0; // Tiempo de uso de la habilidad

    public float maxSpeedBuff = 20;
    public float accelerationBuff = 30;
    
    public int energyRecharge;  //Energia recargada por segundo
    
    //Metodo para activar habilidad: impulso de velocidad durante unos segundos
    public override void AbilityExecution()
    {
        Debug.Log("Habilidad Ravager ejecutada");
        //uiBoosters.UpdateActiveImage(neededResQuantity);

        if (!isActivated)
        {
            Debug.Log("Habilidad Ravager ejecutada");
            isActivated = true;
            networkedPlayer.nave.maxSpeed += maxSpeedBuff;
            networkedPlayer.nave.acceleration += accelerationBuff;

            //StartCoroutine("DurationCounter");
            //StartCoroutine("CheckEnergy");
        }
        else
        {
            AcabarHabilidad();
        }
    }

    //Metodo para reiniciar habilidad entre ronda
    public override void ResetRonda()
    {
        AcabarHabilidad();
        actualResQuantity = maxResource;

        networkedPlayer.UpdateAbilityUIClientRpc(neededResQuantity/maxResource);
        //StartCoroutine("UiUpdater");
    }

    public override bool CheckAvailability()
    {
        if(actualResQuantity < neededResQuantity && !isActivated)
        {
            return false;
        }
        else if(actualResQuantity >= neededResQuantity)
        {
            return true;
        }
        else
        {
            return true;
        }
    }

    public void Update()
    {
        Color color;
        
        if (!isActivated && actualResQuantity < maxResource)
        {
            actualResQuantity += energyRecharge * Time.deltaTime;
        }
        else if (isActivated)
        {
            Debug.Log("Habilidad estaCtivada");
            if (actualResQuantity <= 0 )
            {
                AcabarHabilidad();
            }
            actualResQuantity -= neededResQuantity * Time.deltaTime;
        }

        if (!isActivated && actualResQuantity < neededResQuantity)
        {
            color = Color.red;
        }
        else if (isActivated)
        {
            color = Color.yellow;
        }
        else
        {
            color = Color.white;
        }
        
        
        networkedPlayer.UpdateAbilityUIClientRpc(actualResQuantity/maxResource, color);
        
        
    }

    //Metodo que acabar la habilidad y restaura los valores a su modo inicial
    private void AcabarHabilidad()
    {
        Debug.Log("Habilidad Ravager terminada");
        if (isActivated)
        {
            Debug.Log("Habilidad Ravager terminada");

            isActivated = false;
            networkedPlayer.nave.maxSpeed -= maxSpeedBuff;
            networkedPlayer.nave.acceleration -= accelerationBuff;
            duracionActual = 0;
            //networkedPlayer.UpdateAbilityUIClientRpc(neededResQuantity);
        }
    }

    //Metodo para aumentar elcontador de duración de la habilidad
    IEnumerator DurationCounter()
    {
        while (duracionActual <= duracionTotal && isActivated)
        {
            yield return new WaitForSeconds(1f); // Esperar un segundo para restar
                
            duracionActual++;
                
            Debug.Log("Duracion segundos Ravager: " + actualResQuantity);
        }
        AcabarHabilidad();
    }

    IEnumerator UiUpdater()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f); // Esperar un 0.1 segundo para actualizar
            networkedPlayer.UpdateAbilityUIClientRpc(actualResQuantity/maxResource);
        }
    }

    IEnumerator CheckEnergy()
    {
        while (isActivated)
        {
            yield return new WaitForSeconds(1f); // Esperar un 0.1 segudnos para comprobar

            actualResQuantity -= neededResQuantity;
            if (actualResQuantity < neededResQuantity)
            {
                AcabarHabilidad();
            }
        }
    }
}
