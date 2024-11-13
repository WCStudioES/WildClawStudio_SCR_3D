using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NavePandora : PlayerShip
{
    public float healthRegenPerOne;
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
        
        //Crear lista de atributos que necesita su habilidad y rellenarla
        List<object> attributes = new List<object>();
        Debug.Log("Pandora comprobacion de NetworkedPlayer" + jugadorPandora.name);
        attributes.Add(jugadorPandora);
        
        //Pasar atributos
        ability.AssignAttributes(attributes);

        //Activar Pasiva
        StartCoroutine("RegenerateHealth");

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
    
    IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Esperar un segundo entre cada regeneración

            int vidaActual = jugadorPandora.actualHealth.Value;
            int vidaMax = jugadorPandora.maxHealth.Value;

            // Regenerar vida si el personaje no está al máximo
            if (vidaActual < vidaMax)
            {
                // Asegurarse de no sobrepasar la vida máxima
                jugadorPandora.actualHealth.Value  = Mathf.Min(vidaActual  + (int)(healthRegenPerOne * vidaMax), jugadorPandora.maxHealth.Value);
                jugadorPandora.UpdateHealthBarClientRpc(jugadorPandora.actualHealth.Value);
            }

            Debug.Log("Vida actual: " + jugadorPandora.actualHealth.Value );
        }
    }
}
