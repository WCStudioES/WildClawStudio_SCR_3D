using DefaultNamespace;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;


public class FireRing : AreaDmg
{
    [SerializeField] private float startOuterRadius = 30f; // Radio inicial del anillo exterior
    [SerializeField] private float startInnerRadius = 30f;  // Radio inicial del anillo interior
    [SerializeField] private float endOuterRadius = 1f;    // Radio final del anillo exterior
    [SerializeField] private float endInnerRadius = 0.5f;  // Radio final del anillo interior
    [SerializeField] private float shrinkDuration = 15f;   // Tiempo total para que se cierre
    [SerializeField] private List<Transform> players;          // Lista de jugadores para comprobar sus posiciones

    private float checkInterval = 0.1f; // Intervalo en segundos
    private float checkTimer = 0f;

    private Color safeZoneColor = new Color(0, 1, 0, 0.25f); // Color del �rea segura
    private Color dangerZoneColor = new Color(1, 0, 0, 0.25f); // Color del �rea peligrosa

    private float currentOuterRadius;
    private float currentInnerRadius;
    private float elapsedTime = 0f;
    public bool isShrinking = false;

    private void Start()
    {
        currentOuterRadius = startOuterRadius;
        currentInnerRadius = startInnerRadius;

        CreateRingVisual();
    }

    private void Update()
    {
        if (isShrinking)
        {
            ShrinkRing();
        }

        UpdateRingVisual();
    }

    private void ShrinkRing()
    {
        // Calculamos el progreso del cierre del anillo
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / shrinkDuration);

        currentOuterRadius = Mathf.Lerp(startOuterRadius, endOuterRadius, t);
        currentInnerRadius = Mathf.Lerp(startInnerRadius, endInnerRadius, t);

        if (t >= 1f)
        {
            isShrinking = false;
        }
    }

    protected override void AdditionalEffectsOnEnter( Collider other)
    {
        throw new System.NotImplementedException();
    }

    protected override void AdditionalEffectsOnStay(Collider other)
    {
        throw new System.NotImplementedException();
    }

    private new void FixedUpdate()
    {
        if (canHit && IsInServidor)
        {
            checkTimer += Time.fixedDeltaTime;
            if (checkTimer >= checkInterval)
            {
                checkTimer = 0f;
                CheckPlayersInRing();
            }
        }
    }

    private void CheckPlayersInRing()
    {
        foreach (var player in players)
        {
            if (player == null) continue;

            float distance = Vector3.Distance(player.position, transform.position);
            //Debug.Log(distance + ", " + currentInnerRadius + ", " + currentOuterRadius);

            // Est� en la zona peligrosa (fuera del anillo seguro, pero dentro del exterior)
            if (distance > currentInnerRadius && distance <= currentOuterRadius)
            {
                IDamageable target = player.GetComponentInParent<IDamageable>();
                //Debug.Log("Target: " + target);

                if (target != null && IsInServidor)
                {
                    canHit = false;
                    OnHit(target, ControladorNaveDueña);
                    StartCoroutine(ResetHitCooldown());
                }
            }
        }
    }

    public void AddPlayer(Transform player)
    {
        players.Add(player);
    }
    
    public void ClearPlayers()
    {
        players.Clear();
    }

    public override void OnHit(IDamageable target, NetworkedPlayer dmgDealer)
    {
        target.GetDamage(dmg, dmgDealer);
    }

    public void Reset()
    {
        ClearPlayers();
        
        isShrinking = false;
        elapsedTime = 0f;

        currentOuterRadius = startOuterRadius;
        currentInnerRadius = startOuterRadius;

        UpdateRingVisual();
    }

    private void CreateRingVisual()
    {

    }

    private void UpdateRingVisual()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = safeZoneColor;
        Gizmos.DrawSphere(transform.position, currentInnerRadius); // �rea segura

        Gizmos.color = dangerZoneColor;
        Gizmos.DrawWireSphere(transform.position, currentOuterRadius); // �rea peligrosa
    }
}
