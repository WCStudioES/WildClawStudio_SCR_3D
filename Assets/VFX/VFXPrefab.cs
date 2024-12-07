using UnityEngine;
using UnityEngine.VFX;

public class VFXPrefab : MonoBehaviour
{
    // GameObjects para cada tipo de VFX
    public GameObject visualEffect;
    public GameObject ssVFX;

    // Variables para almacenar la referencia al VFX creado
    private VisualEffect visualEffectInstance;
    private SpriteSheetVFXAnimation ssVFXInstance;

    // Booleano para decir si el cliente está en windows o no
    private bool isWindows;

    // Tipo de VFX (Simple/Loopeable) y si permanece o se elimina.
    public AnimationType animType;
    public bool isPermanent = false;
    public bool isActive = true;
    public enum AnimationType
    {
        Loopeable,
        Simple
    }

    //Al instanciarse el VFXPrefab, ya tiene pos, rot y parent
    private void Start()
    {
        isWindows = false; //= Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
        Debug.Log("IsWindows: " + isWindows);

        InitializeVFX();

        if (isPermanent)
        {
            Debug.Log("VFXPrefab permanente desactivado");
            DeactivateVFX(); // Desactiva solo si el VFX es permanente.
        }
        else
        {
            ActivateVFX(); // Activa por defecto si no es permanente.
        }
    }

    //Función para inicializar el VFX dependiendo de la plataforma y el tipo
    private void InitializeVFX()
    {
        switch (isWindows)
        {
            case true:
                Debug.Log("VFXPrefab Inicializado en windows");
                InitializeVisualEffect();
                break;

            case false:
                Debug.Log("VFXPrefab Inicializado en WebGL");
                InitializeSpriteSheetVFXAnimation();
                break;
        }
    }

    private void InitializeVisualEffect()
    {
        GameObject newVFX = Instantiate(visualEffect, transform);
        if(newVFX != null && newVFX.GetComponent<VisualEffect>() != null)
        {
            visualEffectInstance = newVFX.GetComponent<VisualEffect>();
        }
        ActivateVFX();
    }

    private void InitializeSpriteSheetVFXAnimation()
    {
        GameObject newVFX = Instantiate(ssVFX, transform);
        if (newVFX != null && newVFX.GetComponentInChildren<SpriteSheetVFXAnimation>() != null)
        {
            ssVFXInstance = newVFX.GetComponentInChildren<SpriteSheetVFXAnimation>();
        }
        ActivateVFX();
    }

    // Función de TOGGLE para los VFX que la requieren
    public void Toggle(bool active)
    {
        if(active)
        {
            ActivateVFX();
        }
        else
        {
            DeactivateVFX();
        }
    }

    public void ActivateVFX()
    {
        isActive = true;
        SetVFXState(true);
    }

    public void DeactivateVFX()
    {
        isActive = false;
        SetVFXState(false);
    }

    private void SetVFXState(bool state)
    {
        if (isWindows)
        {
            if (visualEffectInstance != null)
            {
                Debug.Log("VFXPrefab en Windows - " + state);
                visualEffectInstance.enabled = state;
            }
        }
        else
        {
            Debug.Log(ssVFXInstance == null);
            if (ssVFXInstance != null)
            {
                Debug.Log("VFXPrefab en WebGL - " + state);
                ssVFXInstance.Toggle(state); 
            }
        }
    }
}
