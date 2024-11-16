using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour, IAbility
{
    public string Name;
    public string Description;
    public Sprite Sprite;

    public ResourceType resourceType;
    public float actualResQuantity;
    public float neededResQuantity;

    //Referencia al jugador, usado para restar y sumar vida
    public NetworkedPlayer networkedPlayer;

    public enum ResourceType
    {
        None,
        Energy,
        CoolDown
    }

    //public abstract void AssignAttributes(List<object> attributes);

    public void Execute()
    {
        if (CheckAvailability())
        {
            AbilityExecution();
        }
    }

    public abstract void AbilityExecution();
    public abstract bool CheckAvailability();

    // Start is called before the first frame update
    public void Start()
    {
        networkedPlayer= GetComponentInParent<NetworkedPlayer>();
    }

    // Update is called once per frame
    //public abstract void Update();
}
