using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosters : MonoBehaviour
{
    public Image supportAbility;
    public Image activeAbility;
    public Image weaponAbility;

    private float contadorActiva;
    private float contadorArma;
    
    //FALTA METERLO XD
    private float contadorTotalActiva;
    private float contadorTotalArma;
    
    [SerializeField] private NetworkedPlayer player;

    public void SetSupportAbility(Sprite support)
    {
        if(player.IsOwner)
            supportAbility.sprite = support;
    }

    public void SetActiveAbility(Sprite active)
    {
        if(player.IsOwner)
            activeAbility.sprite = active;
    }
    
    public void SetWeaponAbility(Sprite weapon)
    {
        if(player.IsOwner)
            weaponAbility.sprite = weapon;
    }

    //Funcion para cambiar la UI de activa
    public void UpdateActiveImage(float value)
    {
        if (player.IsOwner)
        {
            contadorActiva = value;
            contadorTotalActiva = contadorActiva;
            StartCoroutine("UpdateActiveTimer");
        }
    }

    //Corrutina que lo actualiza la habilidad cada 0.1 segundo
    private IEnumerator UpdateActiveTimer()
    {
        while (contadorActiva >= 0)
        {
            yield return new WaitForSeconds(0.1f); // Esperar 0.1 segundos

            contadorActiva -= 0.1f; // Reducir el contador manualmente
            activeAbility.fillAmount = 1.0f - (contadorActiva / contadorTotalActiva);
        }
        
    }

    //Funcion para cambiar la UI de arma
    public void UpdateWeaponImage(float value)
    {
        if (player.IsOwner)
        {
            contadorArma = value;
            contadorTotalArma = value;
            StartCoroutine("UpdateWeaponTimer");
        }
    }

    //Corrutina que actualiza el arma cada 0.1 segundo
    private IEnumerator UpdateWeaponTimer()
    {
        while (contadorArma >= 0)
        {
            yield return new WaitForSeconds(0.1f); // Esperar 0.1 segundos

            contadorArma -= 0.1f; // Reducir el contador manualmente
            weaponAbility.fillAmount = 1.0f - (contadorArma / contadorTotalArma);
        }
    }
    
}