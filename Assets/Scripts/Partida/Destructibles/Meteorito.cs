using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Meteorito : NetworkBehaviour, IDamageable
{
    public int hpTotal = 50;
    
    //Rango de valores de vida que puede tener el meteorito
    public int hpMaximo = 100;
    public int hpMinimo= 20;
    
    //Rango de escalas de tamaño que puede tener el meteorito
    public float escalaMaxima = 4f;
    public float escalaMinima = 2f;

    private bool isFlashingRed = false;

    public NetworkVariable<int> hpActual = new NetworkVariable<int>(100); // Vida actual del meteorito
    public NetworkVariable<int> xpADar = new NetworkVariable<int>(100); // Experiencia que da el meteorito al destruirlo

    private void Start()
    {
        if (IsServer)
        {
            hpActual.Value = hpTotal;
            //xpADar.Value = 100;
        }
    }

    // M�todo para recibir da�o en el meteorito
    public void GetDamage(int dmg, NetworkedPlayer dueñoDaño)
    {
        if (IsServer)
        {
            hpActual.Value -= dmg;
            Debug.Log("Vida del meteorito: " + hpActual.Value);

            // Si la vida llega a 0, destruir el meteorito
            if (hpActual.Value <= 0)
            {
                DestruirMeteorito(dueñoDaño);
            }
            else
            {
                ChangeMaterialColorClientRpc(Color.red, 0.1f);
            }
        }
    }

    [ClientRpc]
    private void ChangeMaterialColorClientRpc(Color hitColor, float duration)
    {
        StartCoroutine(FlashMaterialsInChildren(hitColor, duration));
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

    // Metodo para destruir el meteorito
    private void DestruirMeteorito(NetworkedPlayer dueñoDaño)
    {
        if (IsServer)
        {
            dueñoDaño.GetXP(xpADar.Value);
            Debug.Log("XpDada del meteorito: " + xpADar.Value);
            Debug.Log("Xp de jugador: " + dueñoDaño.xp.Value);
            //Para testear, de moemnto se resetea cada 1 segundo
            //Invoke("RestaurarMeteorito", 1f); 

            StartCoroutine("DestroyWithDelay");
        }
    }
    public IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
        gameObject.SetActive(false); // Desactiva el meteorito en el servidor
        DestruirMeteoritoClientRpc(); // Sincroniza la desactivación en los clientes
    }

    // Metodo para destruir el meteorito en el cliente
    [ClientRpc]
    private void DestruirMeteoritoClientRpc()
    {
        gameObject.SetActive(false);
    }

    //Funcion para restaurar el meteorito con su hp
    public void RestaurarMeteorito()
    {
        if (IsServer)
        {
            hpActual.Value = hpTotal;
            RestaurarMeteoritoClientRpc();
        }
        gameObject.SetActive(true);
    }
    //Funcion para restaurar el meteorito con su hp con tamaño y experiencia aleatoria
    public void RestaurarMeteoritoAleatorio()
    {
        gameObject.SetActive(true);
        if (IsServer)
        {
            //int hpAponer
            
            hpActual.Value = hpTotal;
            RestaurarMeteoritoClientRpc();
        }
    }
    
    [ClientRpc]
    private void RestaurarMeteoritoClientRpc()
    {
        gameObject.SetActive(true);
    }


}
