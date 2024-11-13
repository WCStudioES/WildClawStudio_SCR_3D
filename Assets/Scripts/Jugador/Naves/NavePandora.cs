using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavePandora : PlayerShip
{
    public float healthRegenPercent;
    public override void InitializeStats()
    {
        shipName = "Pandora";
        description = "Est� guapa";

        initialHealth = 100;
        initialArmor = 10;

        healthIncrement = 20;
        armorIncrement = 3;

        //skins;
        //chromas;
    }
    
    
    public override void FireProjectile()
    {
        throw new System.NotImplementedException();
    }

    public override void UseAbility()
    {
        ability.Execute();
    }
    
    // IMPLEMENTACION DE PASIVA:
    // regeneración de vida pasiva
}
