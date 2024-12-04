using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosters : MonoBehaviour
{
    public Image supportAbility;
    public Image activeAbility;
    private bool isUpgraded = false;
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

    //Funcion para cambiar la UI de activa con un cronometro para el CD
    public void UpdateActiveImageWithCD(float value, bool isUpgraded)
    {
        if (player.IsOwner)
        {
            this.isUpgraded = isUpgraded;
            activeAbility.color = Color.red;
            contadorActiva = value;
            contadorTotalActiva = contadorActiva;
            StartCoroutine("UpdateActiveTimer");
        }
    }
    
    //Funcion para cambiar la UI de activa con el valor dado y el color
    public void UpdateActiveImage(float value, Color color)
    {
        if (player.IsOwner)
        {
            activeAbility.fillAmount = value;
            activeAbility.color = color;
        }
    }
    
    //Funcion para cambiar el color de la UI de activa
    public void UpdateActiveImage(Color color)
    {
        if (player.IsOwner)
            activeAbility.color = color;
    }
    
    //Funcion para cambiar la UI de activa con el valor dado
    public void UpdateActiveImage(float value)
    {
        activeAbility.fillAmount = value;
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
        
        if(isUpgraded)
            activeAbility.color = Color.magenta;
        else
            activeAbility.color = Color.white;
        
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
    
    public void UpdateWeaponImage(Color color)
    {
        if (player.IsOwner)
        {
           weaponAbility.color = color;
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

    public void ResetPartida(Sprite weapon)
    {
        if (player.IsOwner)
        {
            activeAbility.gameObject.SetActive(false);
            supportAbility.gameObject.SetActive(false);
            weaponAbility.sprite = weapon;
            activeAbility.color = Color.white;
            weaponAbility.color = Color.white;
        }
    }
    
}
