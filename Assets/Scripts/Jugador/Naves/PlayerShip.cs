using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerShip", menuName = "PlayerShip")]
public abstract class PlayerShip : ScriptableObject, IPlayerShip
{
    public string shipName;
    public string description;
    public Sprite sprite;

    public int actualMaxHealth;
    public int actualHealth;
    public int movementSpeed;
    public int rotationSpeed;
    public int armor;

    public int actualLevel;
    public int actualExperience;
    public int maxLevel = 10;

    public ControladorNave shipController;
    public List<Transform> proyectileSpawns;

    //public IAbility ability;

    public GameObject shipModel;
    public List<Sprite> skins;
    public List<Sprite> chromas;

    public abstract void FireProjectile();
    public abstract void GainExperience();
    public abstract void GetDamage(int damage, NetworkedPlayer jugador);
    public abstract void InitializeStats();
    public abstract void UseAbility();
}
