using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProyectil
{
    void Launch(Vector3 direction);
    void OnHit(IDamageable target);
}
