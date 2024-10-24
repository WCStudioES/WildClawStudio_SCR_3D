using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAspectRatio : MonoBehaviour
{
    public static bool letterBox = false;
    
    private float targetaspect = 16.0f / 9.0f;
    private float lastScreenHeight = 0;
    private float lastScreenWidth = 0;
    private Camera camara;

    private void Start()
    {
        camara = GetComponent<Camera>();
    }

    void Update()
    {
        if (lastScreenHeight != Screen.height || lastScreenWidth != Screen.width)
        {
            lastScreenHeight = Screen.height;
            lastScreenWidth = Screen.width;
            keepAspectRatio();
        }
    }

    private void keepAspectRatio()
    {
        // determine the game window's current aspect ratio
        float windowaspect = lastScreenWidth / lastScreenHeight;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camara.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camara.rect = rect;
            letterBox = true;
        }

        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camara.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camara.rect = rect;
            letterBox = false;
        }
    }
}
