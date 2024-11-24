using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandoraActive : ToggleAbility
{
    public int hpThreshold = 20;    //Vida mínima para activar
    private int dmgBoost = 15;      //En Porcentaje

    private bool active = false;
    private void Awake()
    {
        type = ActiveType.TogglePassive;
    }

    //Metodo para activar habilidad: más daño a costa de vida cada segundo
    public override void AbilityExecution()
    {
        if (!active)
        {
            active = true;
            StartCoroutine("ReduceLife");

            //Aumentar daño
            networkedPlayer.dmgBalance.Value += dmgBoost;
            
            networkedPlayer.UpdateAbilityUIClientRpc(Color.yellow);
        }
        else
        {
            AcabarHabilidad();
        }
    }

    //Metodo que acabar la habilidad y restaura los valores a su modo inicial
    private void AcabarHabilidad()
    {
        Debug.Log("AcabarHabilidad");
        if (active)
        {
            Debug.Log("AcabarHabilidadIf");
            active = false;

            //Disminuir daño
            networkedPlayer.dmgBalance.Value -= dmgBoost;
            
            networkedPlayer.UpdateAbilityUIClientRpc(Color.white);
        }
    }
    
    //Metodo para redsucir la vida cada segundo, se desactiva si llega a la vida minima
    IEnumerator ReduceLife()
    {
        while (active && networkedPlayer.actualHealth.Value > hpThreshold)
        {
            yield return new WaitForSeconds(1f); // Esperar un segundo para restar
                
            networkedPlayer.GetTrueDamage((int)neededResQuantity, networkedPlayer);
                
            //Debug.Log("Vida de Pandora" + networkedPlayer.actualHealth.Value);
        }
            
        AcabarHabilidad();
    }

    public override void ResetRonda()
    {
        Debug.Log("ResetRonda");
        AcabarHabilidad();
    }
    
}
