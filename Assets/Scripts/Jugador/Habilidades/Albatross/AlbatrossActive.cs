using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AlbatrossActive : ShootProjectileAbility
{
    [SerializeField] private GameObject stormGrenade;
    public override void AbilityExecution()
    {
        //Debug.Log("Proyectil creado");
        GameObject proyectil = Instantiate(stormGrenade);
        Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();

        // Inicializamos el proyectil en el servidor
        proyectilScript.Inicializar(Vector3.zero, networkedPlayer.GetComponentInChildren<CapsuleCollider>(), networkedPlayer, networkedPlayer.IsServer);

        // Programamos la destrucción del proyectil después de 2 segundos
        Destroy(proyectil, 10f);
    }
}
