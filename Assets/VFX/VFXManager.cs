using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Netcode;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    private bool isReturningAllVFX = false;
    private int gameID;

    public void SetGameID(int pGameID)
    {
        gameID = pGameID;
    }

    public int GetGameID()
    {
        return gameID;
    }

    [System.Serializable]
    public class VFXPool
    {
        public GameObject prefab; // Prefab del efecto visual.
        public int poolSize;      // N�mero de instancias iniciales.
        public Queue<GameObject> poolQueue;
        private List<GameObject> activeObjects = new List<GameObject>(); // Lista de objetos activos.

        public void Initialize(Transform parent)
        {
            
            poolQueue = new Queue<GameObject>();
            if (prefab != null)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    GameObject instance = Instantiate(prefab, parent);
                    instance.SetActive(false);
                    poolQueue.Enqueue(instance);
                }
            }

            Debug.Log("Pool Initialized with " + poolQueue.Count + " objects");
        }

        public VFXPrefab Get(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (poolQueue.Count > 0)
            {
                GameObject obj = poolQueue.Dequeue();
                obj.SetActive(true);

                obj.transform.SetParent(parent); // Parentear el objeto al transform dado
                obj.transform.position = position;
                obj.transform.rotation = rotation;

                activeObjects.Add(obj); // A�adir a la lista de activos.
                return obj.GetComponent<VFXPrefab>();
            }

            Debug.LogWarning($"No quedan instancias en la pool de {prefab.name}. Expande el tama�o de la pool.");
            return null;
        }

        public void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(null); // Desparentar al devolver al pool
            activeObjects.Remove(obj); // Eliminar de la lista de activos.
            poolQueue.Enqueue(obj);
        }

        public void ReturnAll()
        {
            foreach (var obj in activeObjects)
            {
                if(obj != null)
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(null);
                    poolQueue.Enqueue(obj);
                }
            }
            activeObjects.Clear(); // Vaciar la lista de activos.
        }
    }


    public Partida partidaActual; // Referencia a la partida en curso

    [Header("VFX Pools")]
    public VFXPool meteoriteDestructionPool;
    public VFXPool greenShotPool;
    public VFXPool redShotPool;
    public VFXPool orangeShotPool;
    public VFXPool explosionPool;
    public VFXPool shipSmokePool;
    public VFXPool shipPropulsionPool;
    public VFXPool fireRingPool;
    public VFXPool getHealPool;
    public VFXPool getPassiveDamagePool;

    public int GameID { get => gameID; set => gameID = value; }

    public enum VFXType
    {
        meteorite,
        greenMF,
        redMF,
        orangeMF,
        explosion,
        shipSmoke,
        shipFire,
        fireRing,
        getHeal,
        getPassiveDamage
    }

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize all pools
        InitializePools();

        //Deja los VFX ready
        PreloadVFX();
    }

    public bool IsWebGLOnMobile()
    {
        // Verificar si es WebGL y un dispositivo m�vil
        return Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform;
    }

    private void ReduceVFXPoolSize()
    {
        // Aqu� reduces el tama�o de las pools de VFX o ajustas configuraciones
        // Ejemplo:
        VFXManager.Instance.meteoriteDestructionPool.poolSize = 4; // Cambia seg�n tu sistema�de�VFX
        VFXManager.Instance.greenShotPool.poolSize = 8;
        VFXManager.Instance.redShotPool.poolSize = 10;
        VFXManager.Instance.orangeShotPool.poolSize = 8;
        VFXManager.Instance.explosionPool.poolSize = 4;
        VFXManager.Instance.shipSmokePool.poolSize = 3;
        VFXManager.Instance.shipPropulsionPool.poolSize = 10;
        VFXManager.Instance.fireRingPool.poolSize = 3;
        VFXManager.Instance.getHealPool.poolSize = 4;
        VFXManager.Instance.getPassiveDamagePool.poolSize = 1;
    }

    private void InitializePools()
    {
        if (IsServer()) return;

        if (IsWebGLOnMobile())
        {
            Debug.Log("El juego est� corriendo en un navegador de m�vil. Ajustando configuraciones...");
            ReduceVFXPoolSize();
        }

        meteoriteDestructionPool.Initialize(transform);
        greenShotPool.Initialize(transform);
        redShotPool.Initialize(transform);
        orangeShotPool.Initialize(transform);
        explosionPool.Initialize(transform);
        shipSmokePool.Initialize(transform);
        shipPropulsionPool.Initialize(transform);
        fireRingPool.Initialize(transform);
        getHealPool.Initialize(transform);
        getPassiveDamagePool.Initialize(transform);
        Debug.Log("El juego no est� corriendo en un navegador de m�vil.");

    }

    public VFXPrefab SpawnVFX(VFXType type, Vector3 position, Quaternion rotation, Transform parent = null)//, int pGameID, Transform parent = null)
    {
        if (IsServer()) return null;

        //if(gameID != pGameID) return null;

        Debug.Log("VFXSpawned: " + type);

        VFXPool pool = GetPoolByType(type);
        if (pool != null)
        {
            VFXPrefab toReturn = pool.Get(position, rotation, parent);
            //toReturn.gameID = gameID;

            if (toReturn != null && (toReturn.animType == VFXPrefab.AnimationType.Simple || toReturn.animType == VFXPrefab.AnimationType.StaysAtEnd))
            {
                toReturn.ActivateVFX();
            }
            return toReturn;
        }
        else return null;
    }


    public void ReturnVFX(GameObject obj, VFXType type)//, int pGameID)
    {
        if (IsServer()) return;

        //if (gameID != pGameID) return;

        VFXPool pool = GetPoolByType(type);
        obj.GetComponent<VFXPrefab>().DeactivateVFX();

        if (pool != null && obj != null)
        {
            Debug.Log(type + " Devuelto al pool de VFX");
            pool.ReturnToPool(obj);
        }
        else
        {
            Debug.LogError($"No se encontr� un pool para el tipo '{type}' al intentar devolver el VFX.");
        }
    }

    public VFXPool GetPoolByType(VFXType type)
    {
        if (IsServer()) return null;

        return type switch
        {
            VFXType.meteorite => meteoriteDestructionPool,
            VFXType.greenMF => greenShotPool,
            VFXType.redMF => redShotPool,
            VFXType.orangeMF => orangeShotPool,
            VFXType.explosion => explosionPool,
            VFXType.shipSmoke => shipSmokePool,
            VFXType.shipFire => shipPropulsionPool,
            VFXType.fireRing => fireRingPool,
            VFXType.getHeal => getHealPool,
            VFXType.getPassiveDamage => getPassiveDamagePool,
            _ => null,
        };
    }

    public void ReturnAllVFX()
    {
        if (IsServer()) return;

        isReturningAllVFX = true;

        meteoriteDestructionPool.ReturnAll();
        greenShotPool.ReturnAll();
        redShotPool.ReturnAll();
        orangeShotPool.ReturnAll();
        explosionPool.ReturnAll();
        shipSmokePool.ReturnAll();
        shipPropulsionPool.ReturnAll();
        fireRingPool.ReturnAll();
        getHealPool.ReturnAll();
        getPassiveDamagePool.ReturnAll();

        isReturningAllVFX = false;
        Debug.Log("Todos los VFX han sido devueltos a sus pools.");
    }

    // Comprueba si un cliente pertenece a la partida actual
    private bool EsClienteDePartida(ulong clientId)
    {
        if (partidaActual == null)
        {
            Debug.LogWarning("No hay partida actual asignada.");
            return false;
        }

        foreach (var jugador in partidaActual.jugadores)
        {
            if (jugador != null && jugador.NetworkObject.OwnerClientId == clientId)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsServer()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
    }

    private void PreloadVFX()
    {
        if (IsServer()) return;

        // Pre-cargar los efectos visuales de cada pool
        PreloadPool(meteoriteDestructionPool);
        PreloadPool(greenShotPool);
        PreloadPool(redShotPool);
        PreloadPool(orangeShotPool);
        PreloadPool(explosionPool);
        PreloadPool(shipSmokePool);
        PreloadPool(shipPropulsionPool);
        PreloadPool(fireRingPool);
        PreloadPool(getHealPool);
        PreloadPool(getPassiveDamagePool);
    }

    private void PreloadPool(VFXPool pool)
    {
        if(pool.poolQueue.Count > 0)
        {
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = pool.poolQueue.Dequeue();  // Extrae del pool
                obj.SetActive(true);  // Activa el objeto para inicializarlo
                pool.poolQueue.Enqueue(obj);  // Vuelve a meterlo en el pool
            }
        }
        Debug.Log("Pool preloaded");
    }
}
