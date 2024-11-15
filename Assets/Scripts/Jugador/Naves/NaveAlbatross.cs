using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaveAlbatross : PlayerShip
{
    public float additionalExecutionDmgPerOne;
    public float lifeThesholdPerOne;
    public override void InitializeStats()
    {
        shipName = "Albatross";
        description = "";

        initialHealth = 100;
        initialArmor = 10;

        healthIncrement = 20;
        armorIncrement = 3;
        
        //Crear lista de atributos que necesita su habilidad y rellenarla
        List<object> attributes = new List<object>();
        attributes.Add(shipName);
        
        //Pasar atributos
        ability.AssignAttributes(attributes);
        
        //skins;
        //chromas;
    }

    public override void FireProjectile()
    {
        throw new NotImplementedException();
    }

    public override void UseAbility()
    {
        ability.Execute();
    }

}
