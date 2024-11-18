using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IDamageable
    {
        public void GetDamage(int damage, NetworkedPlayer dmgDealer);
    }
}