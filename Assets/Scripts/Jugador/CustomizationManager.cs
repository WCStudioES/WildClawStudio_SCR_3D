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

    // HABILIDADES DE LA NAVE
    public Image equippedShipActive;
    public Image equippedShipPassive;
    
    [SerializeField] private Image[] LifePoints;
    [SerializeField] private Image[] PowerPoints;
    [SerializeField] private Image[] SpeedPoints;

    public TMP_Text equippedShipActiveDescription;
    public TMP_Text equippedShipPassiveDescription;

    // BUILD DEL JUGADOR
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
            //Debug.Log("Actualiza las listas de la UI");
            // Aseg�rate de que las listas est�n vac�as antes de cargar los nuevos sprites
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

    //CAMBIA EL �NDICE DEL EQUIPAMIENTO (AL PULSAR UN BOT�N)
    public void ChangeEquipment(int index, bool isNext)
    {
        int maxIndex;
        int equippedIndex;

        // Determina el �ndice m�ximo y el �ndice actual seg�n el tipo de equipamiento
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

        // Ajusta el �ndice seg�n si es "Next" o "Previous"
        if (isNext && equippedIndex < maxIndex)
            equippedIndex++;
        else if(isNext && equippedIndex == maxIndex)
            equippedIndex = 0;
        else if (!isNext && equippedIndex > 0)
            equippedIndex--;
        else if(!isNext && equippedIndex == 0)
            equippedIndex = maxIndex;

        // Actualiza el �ndice del equipo seleccionado
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
         // Debug.Log("Actualizar im�genes UI");
        PlayerShip playerShip = craftImages[equippedCraftIndex].GetComponent<PlayerShip>(); //nave escogida
        
        // NAVE ELEGIDA
        shipName.text = playerShip.shipName;
        equippedCraftImage.sprite = playerShip.sprite;
        //equippedSkinImage.sprite = skinImages[equippedSkinIndex];
        
        //Stats de la nave
        UpdateShipStatsUI(playerShip.lifeUi,playerShip.powerUi ,playerShip.speedUi);

        // HABILIDADES DE LA NAVE
        equippedShipActive.sprite = playerShip.activeAbility.Sprite;
        equippedShipPassive.sprite = playerShip.passiveAbility.Sprite;

        equippedShipActiveDescription.text = playerShip.activeAbility.Description;
        equippedShipPassiveDescription.text = playerShip.passiveAbility.Description;

        UpdateNetworkedPlayerEquipment();
    }

    private void UpdateShipStatsUI(int life, int power, int speed)
    {
        //Establecer puntos de vida
        for (int i = 0; i < LifePoints.Length; i++)
        {
            if (i <= life)
            {
                LifePoints[i].gameObject.SetActive(true);
            }
            else
            {
                LifePoints[i].gameObject.SetActive(false);
            }
        }
        
        //Establecer puntos de poder
        for (int i = 0; i < PowerPoints.Length; i++)
        {
            if (i <= power)
            {
                PowerPoints[i].gameObject.SetActive(true);
            }
            else
            {
                PowerPoints[i].gameObject.SetActive(false);
            }
        }
        
        //Establecer puntos de velocidad
        for (int i = 0; i < SpeedPoints.Length; i++)
        {
            if (i <= speed)
            {
                SpeedPoints[i].gameObject.SetActive(true);
            }
            else
            {
                SpeedPoints[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdatePopUpAmmo(List<Image> popUpImages, List<GameObject> equipmentImages, int equippedIndex)
    {
        int totalImages = equipmentImages.Count;

        // Verifica si hay suficientes im�genes para mostrar en el pop-up
        if (totalImages == 1)
        {
            // Solo una imagen: colocar en el centro y vaciar las otras posiciones
            popUpImages[0].sprite = null;
            popUpImages[1].sprite = equipmentImages[0].GetComponent<Proyectil>().sprite;
            popUpImages[2].sprite = null;
        }
        else if (totalImages == 2)
        {
            // Dos im�genes: colocar en la izquierda y el centro
            popUpImages[0].sprite = equipmentImages[0].GetComponent<Proyectil>().sprite;
            popUpImages[1].sprite = equipmentImages[1].GetComponent<Proyectil>().sprite;
            popUpImages[2].sprite = null;
        }
        else
        {
            // Tres o m�s im�genes: mostrar la imagen actual en el centro y las adyacentes en los laterales
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

        // Verifica si hay suficientes im�genes para mostrar en el pop-up
        if (totalImages == 1)
        {
            // Solo una imagen: colocar en el centro y vaciar las otras posiciones
            popUpImages[0].sprite = null;
            popUpImages[1].sprite = equipmentImages[0];
            popUpImages[2].sprite = null;
        }
        else if (totalImages == 2)
        {
            // Dos im�genes: colocar en la izquierda y el centro
            popUpImages[0].sprite = equipmentImages[0];
            popUpImages[1].sprite = equipmentImages[1];
            popUpImages[2].sprite = null;
        }
        else
        {
            // Tres o m�s im�genes: mostrar la imagen actual en el centro y las adyacentes en los laterales
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
