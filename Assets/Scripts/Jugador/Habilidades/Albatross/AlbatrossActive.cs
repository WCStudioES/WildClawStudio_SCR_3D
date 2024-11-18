using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AlbatrossActive : ShootProjectileAbility
{
    [SerializeField] private GameObject stormGrenade;
    public override void AbilityExecution()
    {
        Debug.Log("Albatros lanza habilidad");

        Transform spawn = GetComponentInParent<PlayerShip>().proyectileSpawns[0];

        //Debug.Log("Proyectil creado");
        GameObject proyectil = Instantiate(stormGrenade, spawn.position, spawn.rotation);
        Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();

        // Inicializamos el proyectil en el servidor
        proyectilScript.Inicializar(spawn.forward, networkedPlayer.GetComponentInChildren<CapsuleCollider>(), networkedPlayer, networkedPlayer.IsServer);

        // Programamos la destrucción del proyectil después de 10 segundos
        Destroy(proyectil, 10f);
    }
}
