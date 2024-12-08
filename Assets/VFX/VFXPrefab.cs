using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class VFXPrefab : MonoBehaviour
{
    // GameObjects para cada tipo de VFX
    public GameObject visualEffect;
    public GameObject ssVFX;

    // Variables para almacenar la referencia al VFX creado
    private VisualEffect visualEffectInstance;
    private Animator ssVFXInstance;

    // Booleano para decir si el cliente está en windows o no
    private bool isWindows;

    // Tipo de VFX (Simple/Loopeable) y si permanece o se elimina.
    public AnimationType animType;
    public VFXManager.VFXType type;
    public bool isPermanent = false;
    public enum AnimationType
    {
        Loopeable,
        Simple,
        StaysAtEnd
    }

    //Al instanciarse el VFXPrefab, ya tiene pos, rot y parent
    private void Start()
    {
        isWindows = false; //= Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
        Debug.Log("IsWindows: " + isWindows);

        if (isWindows)
        {
            ssVFX = null;
        }
        else
        {
            visualEffect = null;
        }

        InitializeVFX();
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
        DeactivateVFX();
    }

    private void InitializeVisualEffect()
    {
        GameObject newVFX = Instantiate(visualEffect, transform);
        if(newVFX != null && newVFX.GetComponent<VisualEffect>() != null)
        {
            visualEffectInstance = newVFX.GetComponent<VisualEffect>();
        }
    }

    private void InitializeSpriteSheetVFXAnimation()
    {
        GameObject newVFX = Instantiate(ssVFX, transform);
        if (newVFX != null && newVFX.GetComponentInChildren<Animator>() != null)
        {
            ssVFXInstance = newVFX.GetComponentInChildren<Animator>();

            SpriteRenderer spriteRenderer = ssVFXInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color currentColor = spriteRenderer.color; // Obtén el color actual
                currentColor.a = 0f;                       // Cambia solo el alfa
                spriteRenderer.color = currentColor;       // Asigna el color modificado
            }

            //StartCoroutine(WaitForInitializationAndDeactivate());
        }
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
        SetVFXState(true);
    }

    public void DeactivateVFX()
    {
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
            //Debug.Log(ssVFXInstance == null);

            if (ssVFXInstance != null)
            {
                Debug.Log("VFXPrefab en WebGL - " + state);

                SpriteRenderer spriteRenderer = ssVFXInstance.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color currentColor = spriteRenderer.color; // Obtén el color actual
                    currentColor.a = state ? 1f : 0f;          // Cambia solo el alfa
                    spriteRenderer.color = currentColor;       // Asigna el color modificado
                }

                ssVFXInstance.enabled = state;

                if (state && animType == AnimationType.Simple)
                {
                    StartCoroutine(ReturnVFXToPool());
                }
            }
        }
    }

    private IEnumerator ReturnVFXToPool()
    {
        if (ssVFXInstance == null)
        {
            Debug.LogError("Animator no encontrado en ssVFXInstance.");
            yield break; // Termina la corrutina si no hay Animator
        }

        // Espera hasta que la animación actual termine
        while (ssVFXInstance.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && ssVFXInstance.GetCurrentAnimatorStateInfo(0).loop == false)
        {
            yield return null; // Espera al siguiente frame
        }

        // Devuelve el objeto al pool
        VFXManager.Instance.ReturnVFX(gameObject, type);
    }
}
