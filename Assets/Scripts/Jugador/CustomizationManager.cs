using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviour
{
    //NETWORKED PLAYER
    [SerializeField] private NetworkedPlayer networkedPlayer;

    // ASPECTO DE LA NAVE
    public Image equippedCraftImage;
    public Image equippedSkinImage;

    public int equippedCraftIndex;
    public int equippedSkinIndex;

    [SerializeField] private List<Sprite> craftImages;
    [SerializeField] private List<Sprite> skinImages;


    // BUILD DEL JUGADOR
    public Image equippedAmmoImage;
    public Image equippedSupportImage;

    public int equippedAmmoIndex;
    public int equippedSupportIndex;

    [SerializeField] private List<Sprite> ammoImages;
    [SerializeField] private List<Sprite> supportImages;

    // IMAGENES POP-UP
    public List<Image> popUpAmmoImages;
    public List<Image> popUpSupportImages;

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
                craftImages.Add(ship.GetComponent<PlayerShip>().sprite);
            }

            // Cargar sprites de availableAmmo en ammoImages
            foreach (var ammo in networkedPlayer.allProjectiles)
            {
                ammoImages.Add(ammo.GetComponent<Proyectil>().sprite);
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
                maxIndex = skinImages.Count - 1;
                equippedIndex = equippedSkinIndex;
                break;
            case 2:
                maxIndex = ammoImages.Count - 1;
                equippedIndex = equippedAmmoIndex;
                break;
            case 3:
                maxIndex = supportImages.Count - 1;
                equippedIndex = equippedSupportIndex;
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
                break;
            case 3:
                equippedSupportIndex = equippedIndex;
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
        equippedCraftImage.sprite = craftImages[equippedCraftIndex];
        equippedSkinImage.sprite = skinImages[equippedSkinIndex];
        equippedAmmoImage.sprite = ammoImages[equippedAmmoIndex];
        equippedSupportImage.sprite = supportImages[equippedSupportIndex];

        // Actualiza las imágenes de los pop-ups de soporte y munición
        UpdatePopUpImages(popUpAmmoImages, ammoImages, equippedAmmoIndex);
        UpdatePopUpImages(popUpSupportImages, supportImages, equippedSupportIndex);

        UpdateNetworkedPlayerEquipment();
    }

    private void UpdatePopUpImages(List<Image> popUpImages, List<Sprite> equipmentImages, int equippedIndex)
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
    }

    public void UpdateNetworkedPlayerEquipment()
    {
        // Llama al `NetworkedPlayer` para actualizar la personalización
        if (networkedPlayer != null && networkedPlayer.IsOwner)
        {
            networkedPlayer.ApplyCustomization(equippedCraftIndex, equippedAmmoIndex);
        }
    }

}
