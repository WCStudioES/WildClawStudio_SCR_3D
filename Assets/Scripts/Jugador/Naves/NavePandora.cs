using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NavePandora : PlayerShip
{
    public int healthRegen = 1;
    public NetworkedPlayer jugadorPandora;
    private bool hasASecondPassed = true;
    public override void InitializeStats()
    {
        shipName = "Pandora";
        description = "Est� guapa";

        initialHealth = 100;
        initialArmor = 10;

        healthIncrement = 20;
        armorIncrement = 3;
        
        jugadorPandora = shipController.opcionesJugador.controladorDelJugador;
        
        //Crear lista de atributos que necesita su habilidad y rellenarla
        List<object> attributes = new List<object>();
        Debug.Log("Pandora comprobacion de NetworkedPlayer" + jugadorPandora.name);
        attributes.Add(jugadorPandora);
        
        //Pasar atributos
        ability.AssignAttributes(attributes);
        
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
    protected void Update()
    {
        //Debug.Log("NaveRavager TRANSFORM: " + transform.position);
        transform.localPosition = Vector3.zero;

        if (hasASecondPassed && jugadorPandora.actualHealth.Value < jugadorPandora.maxHealth.Value)
        {
            jugadorPandora.GetHeal(healthRegen, jugadorPandora);
            hasASecondPassed = false;
            Invoke("resetSecond", 1f);
            Debug.Log( "Pandora" + jugadorPandora.actualHealth.Value);
        }
    }

    private void resetSecond()
    {
        hasASecondPassed = true;
    }
}