using DefaultNamespace;
using DefaultNamespace.Proyectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StormGrenade : Proyectil
{
    [SerializeField] private GameObject storm;
    [SerializeField] private Transform stormSpawn;
    private bool activo = true; //Variable apra ver si el misil ha explotado ya

    public Partida partida;
    public bool isUpgraded;

    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        if (activo)
        {
            activo = false;

            Debug.Log("*Bonk* Granada en la cabeza");

            var stormObject = Instantiate(storm, stormSpawn.position, Quaternion.identity);
            StormAoE explosionScript = stormObject.GetComponent<StormAoE>();
            explosionScript.dmg = dmg;
            explosionScript.CrearAreaDmg(CuerpoNaveDuena, dmgDealer, IsEnServidor, direction, partida);
            Debug.Log("Mejorado");

            /*
            else
            {
                var stormObject = Instantiate(upgradedstorm, stormSpawn.position, Quaternion.identity);
                StormAoE explosionScript = stormObject.GetComponent<StormAoE>();
                explosionScript.dmg = dmg;
                explosionScript.isUpgraded = isUpgraded;
                explosionScript.CrearAreaDmg(CuerpoNaveDuena, dmgDealer, IsEnServidor, direction, partida);
            }
            */
        }
    }

    public void InitializeGrenade(Partida partida, int damage)
    {
        this.partida = partida;
        //this.isUpgraded = isUpgraded;
        dmg = damage;
        Debug.Log(dmg + "Daño tormenta" + isUpgraded + "mejorado");
    }
    public void Detonar(NetworkedPlayer dmgDealer)
    {
        if (activo)
        {
            activo = false;

            Debug.Log("Granada de tormenta detonada");


            var stormObject = Instantiate(storm, stormSpawn.position, Quaternion.identity);
            StormAoE explosionScript = stormObject.GetComponent<StormAoE>();
            explosionScript.dmg = dmg;
            explosionScript.CrearAreaDmg(CuerpoNaveDuena, dmgDealer, IsEnServidor, direction, partida);
            //explosionScript.IncreaseScale();
        
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponentInChildren<Renderer>());
  
        }
    }
    
}
