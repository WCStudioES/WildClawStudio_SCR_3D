using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProyectil
{
    void OnHit(IDamageable target, NetworkedPlayer dmgDealer);
    bool IsChildOfOwner(Transform target);
}
