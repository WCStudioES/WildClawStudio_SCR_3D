using Unity.Netcode;

namespace DefaultNamespace
{
    public interface IDamageable
    {
        public void GetDamage(int damage, NetworkedPlayer jugador);
    }
}