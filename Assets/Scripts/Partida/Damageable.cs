using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Damageable : NetworkBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected Color hitColor;

    [SerializeField] protected Collider assetCollider;
    [SerializeField] protected Renderer assetRenderer;

    protected Coroutine flashCoroutine;
    protected Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();

    public NetworkVariable<int> actualHealth = new NetworkVariable<int>(0); // Vida actual

    public ResourceToGive resType;
    public NetworkVariable<int> resToGive = new NetworkVariable<int>(0); // Cantidad de recurso que da al destruirlo

    protected bool isFlashing = false;

    public AudioClip destructionSFX;

    //public bool isDamageable = true;

    public enum ResourceToGive
    {
        None,
        Health,
        Experience
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public abstract void GetDamage(int damage, NetworkedPlayer dmgDealer);

    [ClientRpc]
    protected void ChangeMaterialColorClientRpc(float duration)
    {
        StartCoroutine(FlashMaterialsInChildren(duration));
    }

    protected IEnumerator FlashMaterialsInChildren(float duration)
    {
        if (!isFlashing)
        {
            isFlashing = true;

            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                isFlashing = false;
                yield break;
            }

            // Almacenar los colores originales
            foreach (var renderer in renderers)
            {
                // Omite los renderers asociados a objetos con un VisualEffect
                if (renderer.GetComponent<VisualEffect>() != null)
                    continue;

                foreach (var material in renderer.materials)
                {
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

            yield return new WaitForSeconds(duration);

            RestoreOriginalColors(); // Restaurar colores originales
            isFlashing = false;
        }
    }

    protected void RestoreOriginalColors()
    {
        var renderers = GetComponentsInChildren<Renderer>();

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

        originalColors.Clear();
    }

    public void StopFlashingAndCleanUp()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine); // Detener la corrutina
            flashCoroutine = null;
        }
        RestoreOriginalColors(); // Restaurar colores originales si no ha terminado
    }

    [ClientRpc]
    public void PlayDestructionSFXClientRpc()
    {
        if(destructionSFX != null)
        {
            AudioManager.Instance.PlaySFX(destructionSFX, transform.position);
        }
    }

    protected abstract void DestroyDamageable(NetworkedPlayer damageDealer);
    //{
    //    if (IsServer)
    //    {
    //        switch (resType)
    //        {
    //            case ResourceToGive.None:
    //                break;

    //            case ResourceToGive.Experience:
    //                due�oDa�o.GetXP(resToGive.Value);
    //                break;

    //            case ResourceToGive.Health:
    //                due�oDa�o.GetHeal(resToGive.Value, due�oDa�o);
    //                break;
    //        }
    //        StopFlashingAndCleanUp(); // Detener el flashing y restaurar colores
    //    }
    //}
}
