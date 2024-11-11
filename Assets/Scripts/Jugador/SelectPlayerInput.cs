using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectPlayerInput : MonoBehaviour
{
    public PlayerInput playerInput;
    private void Start()
    {

        // Asigna el esquema de control en función del dispositivo detectado
        //if (Gamepad.current != null)
        //{
        //    playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
        //}
        //else if (Touchscreen.current != null)
        //{
        //    playerInput.SwitchCurrentControlScheme("Touch", Touchscreen.current);
        //}
        //else
        //{
        //    playerInput.SwitchCurrentControlScheme("Keyboard&Mouse");
        //}
    }
}
