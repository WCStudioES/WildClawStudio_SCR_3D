using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NaveRavager : PlayerShip
{
    //NOTA: La pasiva de Ravager de no recibir daño de choque esta implementada en ControladorNave
    public override void InitializeStats()
    {
        
    }

    public override void FireProjectile()
    {
        throw new NotImplementedException();
    }
}
