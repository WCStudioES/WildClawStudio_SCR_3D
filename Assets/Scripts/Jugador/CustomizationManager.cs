using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviour
{
    //NETWORKED PLAYER
    [SerializeField] private NetworkedPlayer networkedPlayer;

    // ASPECTO DE LA NAVE
    public TMP_Text shipName;

    public Image equippedCraftImage;
    public Image equippedSkinImage;

    public int equippedCraftIndex;
    public int equippedSkinIndex;

    [SerializeField] private List<GameObject> craftImages;
    //[SerializeField] private List<Sprite> skinImages;

    // BUILD DEL JUGADOR
    public Image equippedAmmoImage;
    public Image equippedSupportImage;

    public int equippedAmmoIndex;
    public int equippedSupportIndex;

    [SerializeField] private List<GameObject> ammoImages;
    [SerializeField] private List<Sprite> supportImages;

    // IMAGENES POP-UP
    public List<Image> popUpAmmoImages;
    public List<Image> popUpSupportImages;

    public Image selectedItem;
    public TMP_Text selectedItemName;
    public TMP_Text selectedItemDescription;

    // Start is called before the first frame update
    void Start()
    {
        //PONE TODO A 0 MIENTRAS NO GUARDAMOS LAS OPCIONES
        equippedCraftIndex = 0;
        equippedSkinIndex = 0;
        equippedAmmoIndex = 0;
        equippedSupportIndex = 0;

        UpdateUILists();
        UpdateImages();
        UpdatePopUpAmmo(popUpAmmoImages, ammoImages, equippedAmmoIndex);
        UpdatePopUpSupport(popUpSupportImages, supportImages, equippedSupportIndex);
    }

    private void UpdateUILists()
    {
        // Carga los sprites de las listas del NetworkedPlayer
        if (networkedPlayer != null)
        {
            Debug.Log("Actualiza las listas de la UI");
            // Asegúrate de que las listas están vacías antes de cargar los nuevos sprites
            craftImages.Clear();
            ammoImages.Clear();

            // Cargar sprites de availableShips en craftImages
            foreach (var ship in networkedPlayer.allShips)
            {
                craftImages.Add(ship);
            }

            // Cargar sprites de availableAmmo en ammoImages
            for (int i = 1; i < networkedPlayer.allProjectiles.Count; i++)
            {
                GameObject ammo = networkedPlayer.allProjectiles[i];
                ammoImages.Add(ammo);
            }
        }
    }

    //CAMBIA EL ÍNDICE DEL EQUIPAMIENTO (AL PULSAR UN BOTÓN)
    public void ChangeEquipment(int index, bool isNext)
    {
        int maxIndex;
        int equippedIndex;

        // Determina el índice máximo y el índice actual según el tipo de equipamiento
        switch (index)
        {
            case 0:
                maxIndex = craftImages.Count - 1;
                equippedIndex = equippedCraftIndex;
                break;
            case 1:
                maxIndex = 0; // skinImages.Count - 1;
                equippedIndex = equippedSkinIndex;
                break;
            case 2:
                maxIndex = ammoImages.Count - 1;
                equippedIndex = equippedAmmoIndex;
                UpdatePopUpAmmo(popUpAmmoImages, ammoImages, equippedAmmoIndex);
                break;
            case 3:
                maxIndex = supportImages.Count - 1;
                equippedIndex = equippedSupportIndex;
                UpdatePopUpSupport(popUpSupportImages, supportImages, equippedSupportIndex);
                break;
            default:
                return;
        }

        // Ajusta el índice según si es "Next" o "Previous"
        if (isNext && equippedIndex < maxIndex)
            equippedIndex++;
        else if (!isNext && equippedIndex > 0)
            equippedIndex--;

        // Actualiza el índice del equipo seleccionado
        switch (index)
        {
            case 0:
                equippedCraftIndex = equippedIndex;
                break;
            case 1:
                equippedSkinIndex = equippedIndex;
                break;
            case 2:
                equippedAmmoIndex = equippedIndex;
                UpdatePopUpAmmo(popUpAmmoImages, ammoImages, equippedAmmoIndex);
                break;
            case 3:
                equippedSupportIndex = equippedIndex;
                UpdatePopUpSupport(popUpSupportImages, supportImages, equippedSupportIndex);
                break;
        }

        UpdateImages();
    }

    // Llama a ChangeEquipment con true para Next y false para Previous
    public void PreviousEquipment(int index) => ChangeEquipment(index, false);
    public void NextEquipment(int index) => ChangeEquipment(index, true);

    public void UpdateImages()
    {
        Debug.Log("Hola");
        // Actualiza las imágenes de la nave y de la build del jugador
        shipName.text = craftImages[equippedCraftIndex].GetComponent<PlayerShip>().shipName;
        equippedCraftImage.sprite = craftImages[equippedCraftIndex].GetComponent<PlayerShip>().sprite;
        //equippedSkinImage.sprite = skinImages[equippedSkinIndex];
        equippedAmmoImage.sprite = ammoImages[equippedAmmoIndex].GetComponent<Proyectil>().sprite;
        equippedSupportImage.sprite = supportImages[equippedSupportIndex];

        UpdateNetworkedPlayerEquipment();
    }

    private void UpdatePopUpAmmo(List<Image> popUpImages, List<GameObject> equipmentImages, int equippedIndex)
    {
        int totalImages = equipmentImages.Count;

        // Verifica si hay suficientes imágenes para mostrar en el pop-up
        if (totalImages == 1)
        {
            // Solo una imagen: colocar en el centro y vaciar las otras posiciones
            popUpImages[0].sprite = null;
            popUpImages[1].sprite = equipmentImages[0].GetComponent<Proyectil>().sprite;
            popUpImages[2].sprite = null;
        }
        else if (totalImages == 2)
        {
            // Dos imágenes: colocar en la izquierda y el centro
            popUpImages[0].sprite = equipmentImages[0].GetComponent<Proyectil>().sprite;
            popUpImages[1].sprite = equipmentImages[1].GetComponent<Proyectil>().sprite;
            popUpImages[2].sprite = null;
        }
        else
        {
            // Tres o más imágenes: mostrar la imagen actual en el centro y las adyacentes en los laterales
            int leftIndex = (equippedIndex - 1 + totalImages) % totalImages;
            int rightIndex = (equippedIndex + 1) % totalImages;

            popUpImages[0].sprite = equipmentImages[leftIndex].GetComponent<Proyectil>().sprite;
            popUpImages[1].sprite = equipmentImages[equippedIndex].GetComponent<Proyectil>().sprite;
            popUpImages[2].sprite = equipmentImages[rightIndex].GetComponent<Proyectil>().sprite;
        }

        selectedItem.sprite = ammoImages[equippedAmmoIndex].GetComponent<Proyectil>().sprite;
        selectedItemName.text = ammoImages[equippedAmmoIndex].GetComponent<Proyectil>().weaponName;
        selectedItemDescription.text = ammoImages[equippedAmmoIndex].GetComponent<Proyectil>().description;
    }

    private void UpdatePopUpSupport(List<Image> popUpImages, List<Sprite> equipmentImages, int equippedIndex)
    {
        int totalImages = equipmentImages.Count;

        // Verifica si hay suficientes imágenes para mostrar en el pop-up
        if (totalImages == 1)
        {
            // Solo una imagen: colocar en el centro y vaciar las otras posiciones
            popUpImages[0].sprite = null;
            popUpImages[1].sprite = equipmentImages[0];
            popUpImages[2].sprite = null;
        }
        else if (totalImages == 2)
        {
            // Dos imágenes: colocar en la izquierda y el centro
            popUpImages[0].sprite = equipmentImages[0];
            popUpImages[1].sprite = equipmentImages[1];
            popUpImages[2].sprite = null;
        }
        else
        {
            // Tres o más imágenes: mostrar la imagen actual en el centro y las adyacentes en los laterales
            int leftIndex = (equippedIndex - 1 + totalImages) % totalImages;
            int rightIndex = (equippedIndex + 1) % totalImages;

            popUpImages[0].sprite = equipmentImages[leftIndex];
            popUpImages[1].sprite = equipmentImages[equippedIndex];
            popUpImages[2].sprite = equipmentImages[rightIndex];
        }

        selectedItem.sprite = supportImages[equippedSupportIndex];
        selectedItemName.text = "Not implemented yet";
        selectedItemDescription.text = "Not implemented yet";
    }

    public void UpdateNetworkedPlayerEquipment()
    {
        if (networkedPlayer != null && networkedPlayer.IsOwner)
        {
            // Llama al servidor para propagar los cambios a otros clientes
            networkedPlayer.ApplyCustomizationServerRpc(equippedCraftIndex, equippedAmmoIndex+1);
        }
    }

}
