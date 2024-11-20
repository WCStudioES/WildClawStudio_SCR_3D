using System.Collections;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ControladorNave : NetworkBehaviour
{
    [SerializeField] private Transform CameraPositionInGame;
    [SerializeField] private Transform CameraPositionCustomization;
    [SerializeField] private GameObject CameraTarget;
    public Transform actualCamera;

    public enum CameraType{
        InGame,
        Customization
    };

    private Vector3 direccionMovimiento = Vector3.zero; // Dirección del movimiento (hacia adelante)
    public Vector3 velocity = Vector3.zero;

    public float rotationSpeed = 100f; // Velocidad de rotación
    public float acceleration = 5f; // Aceleración
    public float deceleration = 50f; // Desaceleración (fricción)
    private float reductionMultiplier = 0.025f; // Controla la intensidad de la reducción
    public float maxSpeed = 10f; // Velocidad máxima alcanzable
    public bool canCollide = true;

    private Vector3 targetDirection; // Dirección hacia la que la nave debería girar
    private bool shouldRotate = false; // Si la nave debe girar automáticamente

    [SerializeField] public OpcionesJugador opcionesJugador;
    [SerializeField] public PlayerShip playerShip;
    public CapsuleCollider colliderNave;

    void Start()
    {
        if (IsOwner)
        {
            opcionesJugador = GetComponentInParent<OpcionesJugador>();

            actualCamera = CameraPositionCustomization;
            //AssignMainCamera();
        }
    }

    void Update()
    {
        if (opcionesJugador != null && opcionesJugador.movimientoActivado && IsServer)
        {
            // Si hay dirección de movimiento, acelerar
            if (direccionMovimiento != Vector3.zero)
            {
                Vector3 forwardDirection = transform.forward.normalized;

                // Aumenta la velocidad en la dirección de movimiento
                velocity += forwardDirection * acceleration * Time.deltaTime;

                // Limitar la magnitud de la velocidad a maxSpeed
                if (velocity.magnitude > maxSpeed)
                {
                    velocity = velocity.normalized * maxSpeed;
                }
            }
            else
            {
                // Decelerar para detenerse si no hay input de movimiento
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * Time.deltaTime);
                if (velocity.magnitude < 0.2f)
                {
                    velocity = Vector3.zero;
                }
            }

            // Rotación automática hacia la dirección objetivo
            if (shouldRotate)
            {
                RotateTowardsTarget();
            }

            // Aplicar la velocidad calculada a la posición del objeto
            transform.position += velocity * Time.deltaTime;

            // Aplica la pasiva de la nave si ocurre todo el rato
            if(playerShip.passiveAbility is StatBuffPassive)
            {
                playerShip.passiveAbility.Execute();
            }
        }
    }

    private void RotateTowardsTarget()
    {
        // Calcula la rotación hacia la dirección objetivo
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Interpola suavemente la rotación actual hacia la deseada
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Si el ángulo es pequeño, detén el giro automático
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            shouldRotate = false;
        }
    }

    public void Move()
    {
        direccionMovimiento = Vector3.forward; // Apuntar hacia adelante
    }

    public void Rotate(float input)
    {
        transform.Rotate(0, input * rotationSpeed * Time.deltaTime, 0);

        if (velocity.magnitude > 0.01f)
        {
            float angle = Vector3.Angle(transform.forward, velocity.normalized);

            float reductionFactor = Mathf.Clamp01(1 - (angle / 180f));

            // Multiplica el factor de reducción para hacer que el efecto sea más o menos fuerte
            velocity *= Mathf.Lerp(1.0f, reductionFactor, reductionMultiplier);
        }
    }

    public void Stop()
    {
        direccionMovimiento = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(velocity.magnitude > maxSpeed/4 && collision.gameObject.GetComponent<IProyectil>() == null && canCollide)
        {
            SetCanCollide(false);
            StartCoroutine("SetCanCollide", true);
            
            //Daño por impacto, Ravager no posee por su pasiva
            if (playerShip.passiveAbility is not OnCollisionPassive)
            {
                Debug.Log("Daño por choque" + opcionesJugador.controladorDelJugador);
                opcionesJugador.controladorDelJugador.GetDamage(20, opcionesJugador.controladorDelJugador);
            }
            else
            {
                playerShip.passiveAbility.Execute();
            }

            // Calcula la dirección del rebote
            Vector3 collisionNormal = collision.contacts[0].normal;
            targetDirection = Vector3.Reflect(velocity.normalized, collisionNormal); // Dirección reflejada

            // Actualiza la velocidad
            velocity = targetDirection * velocity.magnitude / 3;

            // Marca que debe girar automáticamente hacia la dirección objetivo
            shouldRotate = true;
            StartCoroutine("SetShouldRotate", false);
        }
    }

    public IEnumerator SetCanCollide(bool toSet)
    {
        Debug.Log("Espera para colisionar");
        yield return new WaitForSeconds(0.1f); // Delay de 0.1 segundos
        canCollide = toSet;
    }

    public IEnumerator SetShouldRotate(bool toSet)
    {
        Debug.Log("Espera para colisionar");
        yield return new WaitForSeconds(0.3f);
        shouldRotate = toSet;
    }

    public void SetToSpawn(GameObject spawnPoint)
    {
        if (IsServer)
        {
            //Debug.Log("Set To Spawn: " + spawnPoint.transform.position);
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
            velocity = Vector3.zero;
        }
    }

    public void AssignShip(GameObject ship)
    {
        playerShip = ship.GetComponent<PlayerShip>();


        // Eliminamos cualquier CapsuleCollider existente en el padre
        if (colliderNave != null)
        {
            Destroy(colliderNave);
        }

        // Obtenemos el CapsuleCollider del hijo, asumiendo que siempre estará presente
        CapsuleCollider colliderHijo = ship.GetComponent<CapsuleCollider>();
        if (colliderHijo != null)
        {
            // Agregamos un nuevo CapsuleCollider en el padre y copiamos las propiedades del hijo
            colliderNave = gameObject.AddComponent<CapsuleCollider>();
            colliderNave.center = colliderHijo.center;
            colliderNave.radius = colliderHijo.radius;
            colliderNave.height = colliderHijo.height;
            colliderNave.direction = colliderHijo.direction;
            colliderNave.providesContacts= colliderHijo.providesContacts;

            // Desactivamos el CapsuleCollider en el hijo para evitar duplicidad en colisiones
            colliderHijo.enabled = false;

            //Debug.Log("CapsuleCollider del hijo copiado al padre y desactivado en el hijo.");
        }
        else
        {
            Debug.LogWarning("La nave asignada no tiene un CapsuleCollider.");
        }
    }

    public void AssignMainCamera(CameraType cameraType)
    {
        CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();
        //Debug.Log(VC.m_Lens.OrthographicSize);
        if (VC != null)
        {
            switch (cameraType)
            {
                case CameraType.InGame:
                    actualCamera = CameraPositionInGame;

                    VC.m_Lens.Orthographic = true;
                    VC.m_Lens.OrthographicSize = 15;

                    VC.Follow = actualCamera;
                    VC.LookAt = this.transform;

                    break;

                case CameraType.Customization:
                    actualCamera = CameraPositionCustomization;

                    VC.m_Lens.Orthographic = false;

                    VC.Follow = actualCamera;
                    VC.LookAt = CameraTarget.transform;

                    break;
            }
        }
        else
        {
            Debug.LogWarning("No se encontró la cámara virtual Cinemachine.");
        }
    }
}