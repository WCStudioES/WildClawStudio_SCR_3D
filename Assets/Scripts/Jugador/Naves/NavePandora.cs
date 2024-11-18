using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NavePandora : PlayerShip
{
    public override void InitializeStats()
    {
        shipName = "Pandora";
        description = "Est� guapa";

        initialHealth = 100;
        initialArmor = 10;

        healthIncrement = 20;
        armorIncrement = 3;

        dmgBalance = 0;

        //skins;
        //chromas;
    }

    public override void FireProjectile()
    {
        throw new System.NotImplementedException();
    }
}
