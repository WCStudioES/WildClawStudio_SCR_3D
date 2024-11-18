using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class RavagerActive: MovementAbility    
{
    private bool isActivated = false;   //Booleano que indica si está activado
        
    private float duracionTotal= 3; // Tiempo total que se puede usar la habilidad
    private float duracionActual = 0; // Tiempo de uso de la habilidad

    public float maxSpeedBuff = 20;
    public float accelerationBuff = 30;

    //Metodo para activar habilidad: impulso de velocidad durante unos segundos
    public override void AbilityExecution()
    {
        Debug.Log("Habilidad Ravager ejecutada");

        isActivated = true;
        networkedPlayer.nave.maxSpeed += maxSpeedBuff;
        networkedPlayer.nave.acceleration += accelerationBuff;

        StartCoroutine("DurationCounter");
    }

    //Metodo que acabar la habilidad y restaura los valores a su modo inicial
    private void AcabarHabilidad()
    {
        if (isActivated)
        {
            Debug.Log("Habilidad Ravager terminada");

            isActivated = false;
            networkedPlayer.nave.maxSpeed -= maxSpeedBuff;
            networkedPlayer.nave.acceleration -= accelerationBuff;
            duracionActual = 0;
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
}
