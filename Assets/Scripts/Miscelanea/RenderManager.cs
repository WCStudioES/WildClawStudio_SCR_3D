using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RenderManager : MonoBehaviour
{
    private static RenderManager _instance;
    private HashSet<GameObject> registeredObjects = new HashSet<GameObject>();

    public static RenderManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("RendererManager");
                _instance = obj.AddComponent<RenderManager>();
                DontDestroyOnLoad(obj); // Asegura que no se destruya al cambiar de escena
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject); // Solo necesario en el servidor
        }
    }

    public void RegisterNewObject(GameObject newObject)
    {
        if (newObject == null || !newObject.activeInHierarchy || registeredObjects.Contains(newObject))
            return;

        DisableRenderers(newObject);
        registeredObjects.Add(newObject);
    }

    public void UnregisterObject(GameObject obj)
    {
        if (obj == null) return;
        registeredObjects.Remove(obj);
    }

    private void DisableRenderers(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Light[] lights = obj.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
