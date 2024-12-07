using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DashAbility : ActiveAbility
{
    public float dashSpeed = 20f; // Velocidad del dash
    public float dashDuration = 0.5f; // Duraci�n del dash en segundos
    public int dashDamage = 25; // Da�o causado durante el dash
    public Vector3 dashDirection;

    public ControladorNave nave;

    private void Awake()
    {
        type = ActiveType.Dash;
    }

    protected IEnumerator PerformDash()
    {
        if(nave == null)
        {
            nave = networkedPlayer.nave;
        }

        nave.isDashing = true;

        Rigidbody rb = nave.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se encontr� el Rigidbody en la nave.");
            yield break;
        }

        // Asignar la velocidad del dash
        rb.velocity = nave.transform.TransformDirection(dashDirection.normalized) * dashSpeed;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            // Se espera hasta el final del frame
            yield return null;
        }

        // Restaurar velocidad original al terminar el dash
        rb.velocity = Vector3.zero;
        nave.isDashing = false;
        
    }

    public void CollidesWith(Collision other)
    {
        // Detectar si el objeto impactado puede recibir da�o
        IDamageable target = other.gameObject.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.GetDamage((int)(dashDamage + dashDamage * ((float)networkedPlayer.dmgBalance.Value/100)), networkedPlayer);

            // Finalizar el dash al impactar
            nave.GetComponent<Rigidbody>().velocity = Vector3.zero;
            nave.isDashing = false;
        }
    }
}
