using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandoraActive : ToggleAbility
{
    public int hpThreshold = 20;    //Vida mínima para activar
    public int dmgBoost = 15;      //En Porcentaje
    public int dmgReduction = 10;      //En Porcentaje

    //Metodo para activar habilidad: más daño a costa de vida cada segundo
    public override void AbilityExecution()
    {
        if (!networkedPlayer.IsServer) return;
        Debug.Log("Pandora ability before routine" + isUpgraded);
        StartCoroutine("ReduceLife");
        Debug.Log("Pandora ability after routine" + isUpgraded);

        //Aumentar daño
        networkedPlayer.dmgBalance.Value += dmgBoost;
        
        //AumentarResistencia
        if (isUpgraded)
        {
            networkedPlayer.armor.Value += dmgReduction;
            Debug.Log("Pandora mejorada");
        }
            
        networkedPlayer.UpdateAbilityUIClientRpc(Color.yellow);
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

        Toggle();
    }

    public override void ResetRonda()
    {
        if (!networkedPlayer.IsServer) return;

        Debug.Log("ResetRonda");
        if (active)
        {
            Stop();
        }
        timerActive = true;
        networkedPlayer.actualHealth.Value = networkedPlayer.maxHealth.Value;
    }

    public override void Stop()
    {
        if (!networkedPlayer.IsServer) return;

        Debug.Log("AcabarHabilidadIf");

        PlayerShip ship = gameObject.GetComponent<PlayerShip>();

        if(ship != null)
        {
            //Disminuir daño
            networkedPlayer.dmgBalance.Value = ship.dmgBalance + ship.dmgIncrement * networkedPlayer.lvl.Value;
            if (isUpgraded)
            {
                networkedPlayer.armor.Value = ship.initialArmor + ship.armorIncrement * networkedPlayer.lvl.Value;
                networkedPlayer.UpdateAbilityUIClientRpc(Color.magenta);
            }
            else
                networkedPlayer.UpdateAbilityUIClientRpc(Color.white);

        }

    }
}
