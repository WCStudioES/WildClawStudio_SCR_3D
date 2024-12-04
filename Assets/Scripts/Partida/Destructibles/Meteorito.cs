using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Meteorito : DestructibleAsset
{ 
    
    //Rango de valores de vida que puede tener el meteorito
    public int hpMaximo = 100;
    public int hpMinimo= 20;
    
    //Rango de escalas de tamaño que puede tener el meteorito
    public float escalaMaxima = 4f;
    public float escalaMinima = 2f;
    
    private Vector3 rotationSpeed = new Vector3(0, 100, 0); 
    
    public void ResetStats(int vida)
    {
        maxHealth = vida;
        actualHealth.Value = vida;
    }
}
