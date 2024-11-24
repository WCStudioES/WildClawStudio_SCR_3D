using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Damageable : NetworkBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected Color hitColor;

    protected Coroutine flashCoroutine;
    protected Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();

    public NetworkVariable<int> actualHealth = new NetworkVariable<int>(0); // Vida actual

    public ResourceToGive resType;
    public NetworkVariable<int> resToGive = new NetworkVariable<int>(0); // Cantidad de recurso que da al destruirlo

    protected bool isFlashing = false;

    //public AudioClip collisionSFX;
    public AudioClip destructionSFX;


    public enum ResourceToGive
    {
        None,
        Health,
        Experience
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
                foreach (var material in renderer.materials)
                {
                    if (!originalColors.ContainsKey(material))
                    {
                        originalColors[material] = material.color;

                        // Cambiar al color de impacto
                        hitColor.a = material.color.a;
                        material.color = hitColor;
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
            foreach (var material in renderer.materials)
            {
                if (originalColors.ContainsKey(material))
                {
                    material.color = originalColors[material];
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
        AudioManager.Instance.PlaySFX(destructionSFX);
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
