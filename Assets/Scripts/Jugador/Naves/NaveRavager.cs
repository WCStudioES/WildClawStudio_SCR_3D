using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Jugador.Habilidades;
using UnityEngine;

public class NaveRavager : PlayerShip
{
    
    //NOTA: La pasiva de Ravager de no recibir daño de choque esta implementada en ControladorNave
    public override void InitializeStats()
    {
        shipName = "Ravager";
        description = "Est� guapa";

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
