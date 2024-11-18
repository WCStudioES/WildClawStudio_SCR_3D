using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerShip
{
    public void InitializeStats();
    public void FireProjectile();
    public void UseAbility();
}
