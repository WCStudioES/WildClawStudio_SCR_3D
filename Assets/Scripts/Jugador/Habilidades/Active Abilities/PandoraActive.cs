using System.Collections;
using System.Collections.Generic;
using Jugador.Habilidades;
using UnityEngine;

public class PandoraActive : ActiveAbility
{
    public int hpThreshold = 20;            //vida mínima para activar
    private bool isActivated;               //Si esta activado o no el estado

    //Metodo para activar habilidad: más daño a costa de vida cada segundo
    public override void AbilityExecution()
    {
        if (CheckAvailability())
        {
            if (!isActivated)
            {
                isActivated = true;
                StartCoroutine("ReduceLife");

                //Aumentar daño

            }
            else
            {
                AcabarHabilidad();
            }
        }
    }

    //Metodo que acabar la habilidad y restaura los valores a su modo inicial
    private void AcabarHabilidad()
    {
        if (isActivated)
        {
            isActivated = false;
            //Disminuir daño
        }
    }
    
    //Metodo para redsucir la vida cada segundo, se desactiva si llega a la vida minima
    IEnumerator ReduceLife()
    {
        while (isActivated && networkedPlayer.actualHealth.Value > hpThreshold)
        {
            yield return new WaitForSeconds(1f); // Esperar un segundo para restar
                
            networkedPlayer.GetDamage((int)neededResQuantity, networkedPlayer);
                
            Debug.Log("Vida de Pandora" + networkedPlayer.actualHealth.Value);
        }
            
        AcabarHabilidad();
            
    }

    //Metodo para comprobar si se cumplen los requisitios para que se active la habilidad
    public override bool CheckAvailability()
    {
        if (networkedPlayer.actualHealth.Value > hpThreshold)
        {
            return true;
        }
        
        return false;
    }
}
