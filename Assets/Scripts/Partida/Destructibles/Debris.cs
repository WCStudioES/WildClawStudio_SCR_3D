using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;

public class Debris : DestructibleAsset
{
    //Rango de valores de vida que puede tener el debris
    public int hpMaximo = 100;
    public int hpMinimo= 20;
    
    //Rango de escalas de tamaño que puede tener el debris
    public float escalaMaxima = 4f;
    public float escalaMinima = 2f;
}
