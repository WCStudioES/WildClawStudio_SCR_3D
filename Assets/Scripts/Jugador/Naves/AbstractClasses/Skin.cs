using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    public string skinName = "";
    public Sprite skinSprite;
    public Material skinMaterial;

    private void Start()
    {
        if(skinName == null)
        {
            skinName = "";
        }
    }
}
