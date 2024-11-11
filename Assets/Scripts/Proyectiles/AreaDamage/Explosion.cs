using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace.Proyectiles
{
    public class Explosion: AreaDmg
    {
        public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
        {
            target.GetDamage(dmg, dmgDealer);
        }
    }
}