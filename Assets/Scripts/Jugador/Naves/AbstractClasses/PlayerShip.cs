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

    public int dmgBalance;
    public int dmgIncrement;

    //Stats para la UI de seleccion de nave
    public int lifeUi;
    public int powerUi;
    public int speedUi;

    private bool isFlashingRed = false;
    
    
    public int maxLevel = 8;
    public int[] xpByLvl;

    public ControladorNave shipController;
    public List<Transform> proyectileSpawns;
    //public GameObject shieldVisual;

    public ActiveAbility activeAbility;
    public PassiveAbility passiveAbility;

    public List<Skin> skins;

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
    public void UseAbility()
    {
        activeAbility.Execute();
    }

    public void SetLevels()
    {
        xpByLvl = new int [10];

        for(int i = 0; i < xpByLvl.Length; i++)
        {
            xpByLvl[i] = 300 * (i + 1);
        }
    }

    public IEnumerator FlashMaterialsInChildren(Color hitColor, float duration)
    {
        if (!isFlashingRed)
        {
            isFlashingRed = true;
            // Busca todos los Renderers (MeshRenderer o SkinnedMeshRenderer) en los hijos
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) yield break; // Salir si no hay Renderers

            // Almacena los colores originales de todos los materiales
            var originalColors = new Dictionary<Material, Color>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (!originalColors.ContainsKey(material))
                    {
                        originalColors[material] = material.color;
                        material.color = hitColor; // Cambiar el color al de impacto
                    }
                }
            }

            yield return new WaitForSeconds(duration); // Esperar el tiempo especificado

            // Restaurar los colores originales
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (originalColors.ContainsKey(material))
                    {
                        material.color = originalColors[material];
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

        // Obtener todos los Renderers (MeshRenderer o SkinnedMeshRenderer) en los hijos
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in children.");
            return;
        }

        // Cambiar el material de cada renderer
        foreach (var renderer in renderers)
        {
            var materials = renderer.materials; // Obtenemos los materiales actuales
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = selectedMaterial; // Asignamos el nuevo material
            }
            renderer.materials = materials; // Actualizamos los materiales en el renderer
        }
    }

}
