using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VisualShield : Shield
{
    [SerializeField] private List<GameObject> shieldModels; // Lista de prefabs de los diferentes modelos visuales
    private List<GameObject> instantiatedModels = new List<GameObject>(); // Instancias de los modelos en la escena

    public NetworkVariable<int> selectedModelIndex = new NetworkVariable<int>(0); // Índice del modelo seleccionado

    private void Awake()
    {
        // Instancia todos los modelos y desactívalos
        foreach (var model in shieldModels)
        {
            var instance = Instantiate(model, transform);
            instance.SetActive(false);
            instantiatedModels.Add(instance);
        }
    }

    public void Initialize(NetworkedPlayer pOwner, Transform pSpawn, int index)
    {
        base.Initialize(pOwner, pSpawn);

        // Establece el índice en el servidor
        if (IsServer)
        {
            selectedModelIndex.Value = index;
            owner.AddShield(this);
        }

        // Asegúrate de aplicar el modelo correcto al inicio
        ApplyModel(selectedModelIndex.Value);

        // Suscríbete a los cambios de la NetworkVariable
        selectedModelIndex.OnValueChanged += OnModelIndexChanged;

        // Llama a un ClientRpc para sincronizar el modelo en todos los clientes
        ForceSyncModelClientRpc(selectedModelIndex.Value);
    }

    public void SetModel(int index)
    {
        if (IsServer)
        {
            // Asegúrate de que el índice sea válido
            if (index >= 0 && index < instantiatedModels.Count)
            {
                selectedModelIndex.Value = index; // Actualiza la NetworkVariable
            }
        }
    }

    private void OnModelIndexChanged(int oldIndex, int newIndex)
    {
        // Aplica el modelo actualizado
        ApplyModel(newIndex);
    }

    private void ApplyModel(int index)
    {
        // Desactiva todos los modelos
        foreach (var model in instantiatedModels)
        {
            model.SetActive(false);
        }

        // Activa el modelo seleccionado
        if (index >= 0 && index < instantiatedModels.Count)
        {
            instantiatedModels[index].SetActive(true);
            instantiatedModels[index].GetComponent<MeshCollider>().enabled = false;
        }
    }

    [ClientRpc]
    private void ForceSyncModelClientRpc(int modelIndex)
    {
        // Asegura que todos los clientes apliquen el modelo inicial
        ApplyModel(modelIndex);
    }

    private new void OnDestroy()
    {
        // Limpia el evento si el objeto se destruye
        selectedModelIndex.OnValueChanged -= OnModelIndexChanged;
    }
}
