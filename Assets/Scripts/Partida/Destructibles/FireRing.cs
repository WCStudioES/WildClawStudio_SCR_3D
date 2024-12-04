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

    [SerializeField] private Material ringMaterial;
    private Color safeZoneColor = new Color(0, 1, 0, 0.25f); // Color del �rea segura
    private Color dangerZoneColor = new Color(1, 0, 0, 0.25f); // Color del �rea peligrosa

    private GameObject ringVisual;                         // Objeto visual para el anillo
    private MeshFilter ringMeshFilter;                     // Componente MeshFilter del anillo
    private MeshRenderer ringMeshRenderer;                 // Componente MeshRenderer del anillo

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
        if (canHit)
        {
            CheckPlayersInRing();
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
        // Crear un GameObject para la visual del anillo
        ringVisual = new GameObject("FireRingVisual");
        ringVisual.transform.SetParent(transform);
        ringVisual.transform.localPosition = Vector3.zero;

        // Agregar componentes necesarios
        ringMeshFilter = ringVisual.AddComponent<MeshFilter>();
        ringMeshRenderer = ringVisual.AddComponent<MeshRenderer>();
        ringMeshRenderer.material = ringMaterial;

        UpdateRingVisual();
    }

    private void UpdateRingVisual()
    {
        if (ringMeshFilter == null || IsInServidor) return;

        // Crear un anillo entre los radios
        ringMeshFilter.mesh = GenerateRingMesh(currentInnerRadius, currentOuterRadius);
    }

    private Mesh GenerateRingMesh(float innerRadius, float outerRadius)
    {
        Mesh mesh = new Mesh();
        int segments = 32; // N�mero de segmentos para suavizar el anillo

        int vertexCount = segments * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 6];
        Vector2[] uv = new Vector2[vertexCount];

        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;

            // V�rtices del radio interior
            vertices[i * 2] = new Vector3(Mathf.Cos(angle) * innerRadius, 0, Mathf.Sin(angle) * innerRadius);
            // V�rtices del radio exterior
            vertices[i * 2 + 1] = new Vector3(Mathf.Cos(angle) * outerRadius, 0, Mathf.Sin(angle) * outerRadius);

            // Definir UVs
            uv[i * 2] = new Vector2(0, 0);
            uv[i * 2 + 1] = new Vector2(1, 1);

            // Crear tri�ngulos
            int nextIndex = (i + 1) % segments;
            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = nextIndex * 2;
            triangles[i * 6 + 2] = i * 2 + 1;

            triangles[i * 6 + 3] = nextIndex * 2;
            triangles[i * 6 + 4] = nextIndex * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = safeZoneColor;
        Gizmos.DrawSphere(transform.position, currentInnerRadius); // �rea segura

        Gizmos.color = dangerZoneColor;
        Gizmos.DrawWireSphere(transform.position, currentOuterRadius); // �rea peligrosa
    }
}
