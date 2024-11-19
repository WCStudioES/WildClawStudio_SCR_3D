using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SupportItem : MonoBehaviour, ISupportItem 
{
    public string suppItemName;
    public string suppItemDescription;
    public Sprite suppItemSprite;
    public NetworkedPlayer owner; // Controlador de la nave a la que pertenece

    public GameObject supportItemPrefab;

    public abstract void AddToPlayer();
}
