using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;


public abstract class PlayerShip : MonoBehaviour, IPlayerShip
{
    [Header("Ship Information")]
    public string shipName;
    public string description;
    public string about;
    public Sprite sprite;

    [Header("Base Stats")]
    public int initialHealth;
    public int initialArmor;

    public int healthIncrement;
    public int armorIncrement;

    public int dmgBalance;
    public int dmgIncrement;
    public float maxSpeed;
    public float initialSpeed;

    [Header("UI Stats")]
    // Stats para la UI de selecci�n de nave
    public int lifeUi;
    public int powerUi;
    public int speedUi;

    private bool isFlashingRed = false;

    [Header("Leveling")]
    public int maxLevel = 8;
    public int[] xpByLvl;

    [Header("Ship Components")]
    public ControladorNave shipController;
    public List<Transform> proyectileSpawns;
    public List<Transform> firePropulsors;

    [Header("VFX")]
    public GameObject firePropulsionVFX;
    private List<VFXPrefab> activeFireVFX = new List<VFXPrefab>();

    [SerializeField] private GameObject lowHealthVFX;
    private VFXPrefab lowHealthVFXInstance;
    
    [Header("Abilities")]
    public ActiveAbility activeAbility;
    public PassiveAbility passiveAbility;

    [Header("Cosmetics")]
    public List<Skin> skins;


    private void Start()
    {
        SetLevels();
        InitializeStats();
    }

    protected void Update()
    {
        transform.localPosition = Vector3.zero;
    }

    public abstract void FireProjectile();
    public abstract void InitializeStats();

    public void ResetRonda()
    {
        activeAbility.ResetRonda();
    }

    public void UseAbility()
    {
        activeAbility.Execute();
    }

    public void UpgradeAbility()
    {
        activeAbility.UpgradeAbility();
    }

    public void SetLevels()
    {
        xpByLvl = new int[10];
        xpByLvl[0] = 300;

        for (int i = 1; i < xpByLvl.Length; i++)
        {
            xpByLvl[i] = 300 + 200 * (i);
        }
    }

    public IEnumerator FlashMaterialsInChildren(Color hitColor, float duration)
    {
        if (!isFlashingRed)
        {
            isFlashingRed = true;

            // Obtiene todos los Renderers
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) yield break;

            var originalColors = new Dictionary<Material, Color>();
            foreach (var renderer in renderers)
            {
                // Omite los renderers asociados a objetos con un VisualEffect
                if (renderer.GetComponent<VisualEffect>() != null)
                    continue;

                foreach (var material in renderer.materials)
                {
                    // Verifica si el material tiene el color principal (_BaseColor en URP)
                    if (material.HasProperty("_BaseColor"))
                    {
                        // Guarda el color original y aplica el color de impacto
                        if (!originalColors.ContainsKey(material))
                        {
                            originalColors[material] = material.GetColor("_BaseColor");
                            material.SetColor("_BaseColor", hitColor);
                        }
                    }
                }
            }

            // Espera el tiempo especificado
            yield return new WaitForSeconds(duration);

            // Restaura los colores originales
            foreach (var renderer in renderers)
            {
                // Omite nuevamente los renderers asociados a objetos con un VisualEffect
                if (renderer.GetComponent<VisualEffect>() != null)
                    continue;

                foreach (var material in renderer.materials)
                {
                    if (material.HasProperty("_BaseColor") && originalColors.ContainsKey(material))
                    {
                        material.SetColor("_BaseColor", originalColors[material]);
                    }
                }
            }

            isFlashingRed = false;
        }
    }


    public void ChangeSkin(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= skins.Count || skins.Count == 0)
        {
            Debug.LogError("Skin index out of range.");
            return;
        }

        Material selectedMaterial = skins[skinIndex].skinMaterial;

        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in children.");
            return;
        }

        foreach (var renderer in renderers)
        {
            var materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = selectedMaterial;
            }
            renderer.materials = materials;
        }
    }

    public void InitializeVFX()
    {
        activeAbility.InitializeVFX();
        passiveAbility.InitializeVFX();

        foreach (Transform spawn in firePropulsors)
        {
            Debug.Log("VFXPrefab firePropulsors: Creado");
            activeFireVFX.Add(VFXManager.Instance.SpawnVFX(VFXManager.VFXType.shipFire, spawn.position, Quaternion.identity, spawn));

            ToggleFireVFX(false);
        }

        if(lowHealthVFX != null)
        {
            Debug.Log("E");
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            if (firePropulsors[0] != null)
            {
                pos = new Vector3(transform.position.x, firePropulsors[0].transform.position.y, firePropulsors[0].transform.position.z - 0.2f);
            }

            VFXPrefab newVFXPrefab = VFXManager.Instance.SpawnVFX(VFXManager.VFXType.shipSmoke, pos, Quaternion.identity, transform);
            if (newVFXPrefab != null)
            { 
                lowHealthVFXInstance = newVFXPrefab;
            }
        }
    }

    public void ToggleFireVFX(bool isActive)
    {

        foreach (VFXPrefab vfx in activeFireVFX)
        {
            if (vfx != null)
            {
                vfx.Toggle(isActive);
            }
        }
    }

    public void ToggleLowHealthVFX(bool isActive)
    {
        //Debug.Log(isActive + ", " + lowHealthVFXInstance.enabled);
        if (lowHealthVFXInstance != null)
        {
            lowHealthVFXInstance.Toggle(isActive);
        }
    }

    private void OnDestroy()
    {
        lowHealthVFXInstance = null;
        activeFireVFX.Clear();
    }
}
