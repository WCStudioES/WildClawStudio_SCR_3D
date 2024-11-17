using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NavePandora : PlayerShip
{
    public NetworkedPlayer jugadorPandora;
    public override void InitializeStats()
    {
        shipName = "Pandora";
        description = "Est� guapa";

        initialHealth = 100;
        initialArmor = 10;

        healthIncrement = 20;
        armorIncrement = 3;
        
        jugadorPandora = shipController.opcionesJugador.controladorDelJugador;
        
        //skins;
        //chromas;
    }
    
    public override void FireProjectile()
    {
        throw new System.NotImplementedException();
    }

    public override void UseAbility()
    {
        activeAbility.Execute();
    }
}
