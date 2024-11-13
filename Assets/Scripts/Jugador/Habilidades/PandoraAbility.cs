using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandoraAbility : IAbility
{
    public int hpCost = 5;                  //coste de vida cada segundo
    public int hpThreshold = 20;            //vida mínima para activar
    private bool isActivated = false;       //Si esta activado o no el estado
    private NavePandora pandora;            //Referencia a nave, esperar a ver como se asocian habilidades
    

    public void Execute()
    {
        if (CheckAvailability())
        {
            if (!isActivated)
            {
                isActivated = true;
                //Bajar vida(hpCost) cada segundo
                //Aumentar daño
                //Si baja del threshold desactivar
            }
            else
            {
                isActivated = false;
                //Desactivar el estado
            }
        }
    }

    public bool CheckAvailability()
    {
        //If(Pandora.vida > hpThreshold)
        return true;
    }

    public void AssignAttributes(Dictionary<string, object> attributes)
    {
        throw new System.NotImplementedException();
    }
}
