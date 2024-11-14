using System.Collections.Generic;
using UnityEngine;

namespace Jugador.Habilidades
{
    public abstract class BasicAbility : MonoBehaviour, IAbility
    {
        public abstract void AssignAttributes(List<object> attributes);

        public abstract void Execute();

        public abstract bool CheckAvailability();
    }
}
