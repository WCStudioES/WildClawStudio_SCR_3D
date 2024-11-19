using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosters : MonoBehaviour
{
    private Image supportAbility;
    private Image activeAbility;
    private Image weaponAbility;

    private float contadorActiva;
    private float contadorArma;
    

    public void InitializeImages(Image support, Image active, Image weapon)
    {
        supportAbility = support;
        activeAbility = active;
        weaponAbility = weapon;
    }

    //Funcion para cambiar la UI de activa
    public void UpdateActiveImage(float value)
    {
      contadorActiva = value;
      StartCoroutine("UpdateActiveTimer");
    }

    //Corrutina que lo actualiza la habilidad cada 0.1 segundo
    private IEnumerator UpdateActiveTimer()
    {
        contadorActiva = contadorActiva - Time.deltaTime;
        activeAbility.fillAmount = contadorActiva;
        if (contadorActiva <= 0)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    //Funcion para cambiar la UI de arma
    public void UpdateWeaponImage(float value)
    {
        contadorArma = value;
        StartCoroutine("UpdateWeaponTimer");
    }

    //Corrutina que actualiza el arma cada 0.1 segundo
    private IEnumerator UpdateWeaponTimer()
    {
        contadorArma = contadorArma - Time.deltaTime;
        weaponAbility.fillAmount = contadorArma;
        if (contadorArma <= 0)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
    }
    
}
