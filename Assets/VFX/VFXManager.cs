using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("VFX Settings")]
    public GameObject vfxPrefab; // Prefab genérico para los VFX
    public int maxVFXSources = 15;

    private Queue<VFXPrefab> vfxSourcePool = new Queue<VFXPrefab>();
    private List<VFXPrefab> activeVFXSources = new List<VFXPrefab>();

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < maxVFXSources; i++)
        {
            VFXPrefab newVFX = Instantiate(vfxPrefab).GetComponent<VFXPrefab>();
            if (newVFX != null)
            {
                newVFX.gameObject.SetActive(false);
                vfxSourcePool.Enqueue(newVFX);
            }
        }
    }

    /// <summary>
    /// Activa un VFX en una posición específica.
    /// </summary>
    /// <param name="prefab">El prefab VFX a usar.</param>
    /// <param name="position">La posición del VFX.</param>
    /// <param name="isPermanent">Si el VFX debe permanecer activo.</param>
    public void PlayVFX(VFXPrefab prefab, Vector3 position, bool isPermanent = false)
    {
        VFXPrefab vfxInstance;

        if (vfxSourcePool.Count > 0)
        {
            vfxInstance = vfxSourcePool.Dequeue();
        }
        else
        {
            vfxInstance = Instantiate(vfxPrefab).GetComponent<VFXPrefab>();
        }

        vfxInstance.transform.position = position;
        vfxInstance.isPermanent = isPermanent;
        vfxInstance.gameObject.SetActive(true);
        vfxInstance.ActivateVFX();

        activeVFXSources.Add(vfxInstance);

        if (!isPermanent)
        {
            StartCoroutine(ReturnToPoolAfterCompletion(vfxInstance));
        }
    }

    /// <summary>
    /// Activa un VFX en varias posiciones a la vez.
    /// </summary>
    /// <param name="prefab">El prefab VFX a usar.</param>
    /// <param name="positions">Lista de posiciones (Transforms) donde se activará el VFX.</param>
    /// <param name="isPermanent">Si los VFX deben permanecer activos.</param>
    public void PlayVFXForMultipleTransforms(VFXPrefab prefab, List<Transform> positions, bool isPermanent = false)
    {
        foreach (var position in positions)
        {
            PlayVFX(prefab, position.position, isPermanent);
        }
    }

    /// <summary>
    /// Detiene todos los VFX que se encuentran en las posiciones dadas.
    /// </summary>
    /// <param name="positions">Lista de posiciones (Transforms) donde se detendrán los VFX.</param>
    public void StopVFXForMultipleTransforms(List<Transform> positions)
    {
        foreach (var vfx in activeVFXSources)
        {
            foreach (var position in positions)
            {
                if (Vector3.Distance(vfx.transform.position, position.position) < 0.1f)
                {
                    StopVFX(vfx);
                }
            }
        }
    }

    /// <summary>
    /// Detiene un VFX específico.
    /// </summary>
    /// <param name="vfxInstance">El VFXPrefab a detener.</param>
    public void StopVFX(VFXPrefab vfxInstance)
    {
        if (activeVFXSources.Contains(vfxInstance))
        {
            activeVFXSources.Remove(vfxInstance);
            vfxInstance.DeactivateVFX();
            vfxInstance.gameObject.SetActive(false);
            vfxSourcePool.Enqueue(vfxInstance);
        }
    }

    /// <summary>
    /// Detiene todos los VFX activos.
    /// </summary>
    public void StopAllVFX()
    {
        foreach (var vfx in activeVFXSources)
        {
            vfx.DeactivateVFX();
            vfx.gameObject.SetActive(false);
            vfxSourcePool.Enqueue(vfx);
        }
        activeVFXSources.Clear();
    }

    private System.Collections.IEnumerator ReturnToPoolAfterCompletion(VFXPrefab vfxInstance)
    {
        // Espera hasta que el VFX se complete (esto depende de tu lógica de tiempo).
        yield return new WaitUntil(() => !vfxInstance.isActive);
        StopVFX(vfxInstance);
    }
}
