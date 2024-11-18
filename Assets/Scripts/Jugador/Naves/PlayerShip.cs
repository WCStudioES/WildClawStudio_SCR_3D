using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerShip : MonoBehaviour, IPlayerShip
{
    public string shipName;
    public string description;
    public Sprite sprite;

    public int initialHealth;
    public int initialArmor;

    public int healthIncrement;
    public int armorIncrement;

    public int maxLevel = 8;
    public int[] xpByLvl;

    public ControladorNave shipController;
    public List<Transform> proyectileSpawns;

    public ActiveAbility activeAbility;
    public PassiveAbility passiveAbility;

    public List<Sprite> skinSprites;

    private void Start()
    {
        SetLevels();
        InitializeStats();
    }

    protected void Update()
    {
        //Debug.Log("NaveRavager TRANSFORM: " + transform.position);
        transform.localPosition = Vector3.zero;
    }

    public abstract void FireProjectile();
    public abstract void InitializeStats();
    public abstract void UseAbility();

    public void SetLevels()
    {
        xpByLvl = new int [10];

        for(int i = 0; i < xpByLvl.Length; i++)
        {
            xpByLvl[i] = 300 * (i + 1);
        }
    }
}
