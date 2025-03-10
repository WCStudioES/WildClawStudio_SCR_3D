using System;
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
    public TMP_Text skinName;

    public Image equippedCraftImage;
    public Image equippedSkinImage;

    public int equippedCraftIndex;
    public int equippedSkinIndex;

    [SerializeField] private List<GameObject> craftImages;
    [SerializeField] private List<Skin> skinImages;
    //[SerializeField] private List<Sprite> skinImages;

    // HABILIDADES DE LA NAVE
    public Image equippedShipActive;
    public Image equippedShipPassive;
    
    [SerializeField] private Image[] LifePoints;
    [SerializeField] private Image[] PowerPoints;
    [SerializeField] private Image[] SpeedPoints;

    public TMP_Text equippedShipActiveDescription;
    public TMP_Text equippedShipActiveName;
    public TMP_Text equippedShipPassiveDescription;
    public TMP_Text equippedShipPassiveName;

    // BUILD DEL JUGADOR
    public int equippedAmmoIndex;
    public int equippedSupportIndex;

    public Image equippedAmmoImage;
    public Image equippedSupportImage;

    [SerializeField] private List<GameObject> ammoImages;
    [SerializeField] private List<GameObject> supportImages;

    // IMAGENES POP-UP
    public List<Image> popUpAmmoImages;
    public List<Image> popUpSupportImages;

    public Image selectedItem;
    public TMP_Text selectedItemName;
    public TMP_Text selectedItemDescription;
    
    //PANEL ABOUT
    public TMP_Text nameAboutText;
    public TMP_Text descriptionAboutText;
    
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
            supportImages.Clear();

            // Cargar sprites de availableShips en craftImages
            foreach (var ship in networkedPlayer.allShips)
            {
                craftImages.Add(ship);
            }

            foreach (var suppItem in networkedPlayer.allSupport)
            {
                supportImages.Add(suppItem);
            }

            // Cargar sprites de availableAmmo en ammoImages
            for (int i = 1; i < networkedPlayer.allProjectiles.Count; i++)
            {
                if ( networkedPlayer.allProjectiles[i].GetComponent<Proyectil>().mostrarEnSeleccion)
                {
                    GameObject ammo = networkedPlayer.allProjectiles[i];
                    ammoImages.Add(ammo);
                }
            }

            UpdateSkins(equippedCraftIndex);
        }
    }

    private void UpdateSkins(int index)
    {
        equippedSkinIndex = 0;
        skinImages.Clear();
        foreach (var skin in networkedPlayer.allShips[index].GetComponent<PlayerShip>().skins)
        {
            skinImages.Add(skin);
        }
    }

    //CAMBIA EL �NDICE DEL EQUIPAMIENTO (AL PULSAR UN BOT�N)
    public void ChangeEquipment(int index, bool isNext)
    {
        int maxIndex;
        int equippedIndex;

        //SFX DEL BOTON
        AudioManager.Instance.PlayButtonSFX();

        // Determina el �ndice m�ximo y el �ndice actual seg�n el tipo de equipamiento
        switch (index)
        {
            case 0:
                maxIndex = craftImages.Count - 1;
                equippedIndex = equippedCraftIndex;
                break;
            case 1:
                maxIndex =  skinImages.Count - 1;
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
                UpdateSkins(equippedCraftIndex);
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

        if(playerShip.skins.Count > 0)
        {
            skinName.text = playerShip.skins[equippedSkinIndex].skinName;
            equippedSkinImage.sprite = skinImages[equippedSkinIndex].skinSprite;
        }
        
        //Stats de la nave
        UpdateShipStatsUI(playerShip.lifeUi,playerShip.powerUi ,playerShip.speedUi);

        // HABILIDADES DE LA NAVE
        equippedShipActive.sprite = playerShip.activeAbility.Sprite;
        equippedShipPassive.sprite = playerShip.passiveAbility.Sprite;

        equippedShipActiveDescription.text = playerShip.activeAbility.Description + "\n" + playerShip.activeAbility.upgradeDescription;
        equippedShipActiveName.text = playerShip.activeAbility.Name;
        equippedShipPassiveDescription.text = playerShip.passiveAbility.Description;
        equippedShipPassiveName.text = playerShip.passiveAbility.Name;

        equippedAmmoImage.sprite = ammoImages[equippedAmmoIndex].GetComponent<Proyectil>().sprite;
        equippedSupportImage.sprite = supportImages[equippedSupportIndex].GetComponent<SupportItem>().suppItemSprite;

        UpdateNetworkedPlayerEquipment();

        UpdateAboutInformation();
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
        selectedItemDescription.text = ammoImages[equippedAmmoIndex].GetComponent<Proyectil>().description + "\n" +ammoImages[equippedAmmoIndex].GetComponent<Proyectil>().Upgradedescription ;
    }

    private void UpdatePopUpSupport(List<Image> popUpImages, List<GameObject> equipmentImages, int equippedIndex)
    {
        int totalImages = equipmentImages.Count;

        // Verifica si hay suficientes im�genes para mostrar en el pop-up
        if (totalImages == 1)
        {
            // Solo una imagen: colocar en el centro y vaciar las otras posiciones
            popUpImages[0].sprite = null;
            popUpImages[1].sprite = equipmentImages[0].GetComponent<SupportItem>().suppItemSprite;
            popUpImages[2].sprite = null;
        }
        else if (totalImages == 2)
        {
            // Dos im�genes: colocar en la izquierda y el centro
            popUpImages[0].sprite = equipmentImages[0].GetComponent<SupportItem>().suppItemSprite;
            popUpImages[1].sprite = equipmentImages[1].GetComponent<SupportItem>().suppItemSprite;
            popUpImages[2].sprite = null;
        }
        else
        {
            // Tres o m�s im�genes: mostrar la imagen actual en el centro y las adyacentes en los laterales
            int leftIndex = (equippedIndex - 1 + totalImages) % totalImages;
            int rightIndex = (equippedIndex + 1) % totalImages;

            popUpImages[0].sprite = equipmentImages[leftIndex].GetComponent<SupportItem>().suppItemSprite;
            popUpImages[1].sprite = equipmentImages[equippedIndex].GetComponent<SupportItem>().suppItemSprite;
            popUpImages[2].sprite = equipmentImages[rightIndex].GetComponent<SupportItem>().suppItemSprite;
        }

        selectedItem.sprite = supportImages[equippedSupportIndex].GetComponent<SupportItem>().suppItemSprite;
        selectedItemName.text = supportImages[equippedSupportIndex].GetComponent<SupportItem>().suppItemName;
        selectedItemDescription.text = supportImages[equippedSupportIndex].GetComponent<SupportItem>().suppItemDescription;
    }

    public void UpdateNetworkedPlayerEquipment()
    {
        if (networkedPlayer != null && networkedPlayer.IsOwner)
        {
            // Llama al servidor para propagar los cambios a otros clientes
            networkedPlayer.ApplyCustomizationServerRpc(equippedCraftIndex, equippedSkinIndex, equippedAmmoIndex+1, equippedSupportIndex);
        }
    }

    private void UpdateAboutInformation()
    {
        PlayerShip playerShip = craftImages[equippedCraftIndex].GetComponent<PlayerShip>(); //nave escogida
        nameAboutText.text = playerShip.name;
        descriptionAboutText.text = playerShip.description + "\n"  + "\n" + playerShip.about;
    }
}
