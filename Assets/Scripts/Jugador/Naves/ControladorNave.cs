using System.Collections;
using Cinemachine;
using DefaultNamespace;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

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

    public float initialRotationSpeed = 150f;
    public float rotationAcceleration = 50f;
    public float rotationSpeed = 0f; // Velocidad de rotación
    public float maxRotationSpeed = 250;

    [SerializeField]private Vector3 direccionMovimiento = Vector3.zero; // Dirección del movimiento (hacia adelante)
    public Vector2 velocity = Vector2.zero;
    public float reductionMultiplier = 0.025f; // Controla la intensidad de la reducción
    public float initialSpeed = 2.0f;
    public float acceleration = 50f; // Aceleración
    public float deceleration = 50f; // Desaceleración (fricción)
    public float maxSpeed = 10f; // Velocidad máxima alcanzable
    public int maxSpeedDmg = 10; // Velocidad máxima alcanzable

    public bool canBounce = true;
    private Vector3 targetDirection; // Dirección hacia la que la nave debería girar
    private int rotationInput; // Dirección hacia la que la nave debería girar

    [SerializeField] public OpcionesJugador opcionesJugador;
    [SerializeField] public PlayerShip playerShip;
    public MeshCollider colliderNave;

    [Header("SFX")]
    public AudioSource accelerationAudioSource;
    public AudioClip collisionSFX;
    public AudioClip accelerateSFX;

    [Header("Camera Orbit Settings")]
    public float orbitSpeed = 1.5f; // Velocidad de giro constante
    public float orbitDistance = 5f; // Distancia desde el objetivo
    public Vector2 orbitAngles = new Vector2(30, 0); // Ángulos iniciales (elevación, azimut)
    public float minVerticalAngle = 10f;
    public float maxVerticalAngle = 80f;

    //private bool isOrbiting = false;

    //Si la nave está dasheando
    public bool isDashing = false;

    public bool startedMoving = false;
    public bool stoppedMoving = true;

    void Start()
    {
        if (IsOwner)
        {
            opcionesJugador = GetComponentInParent<OpcionesJugador>();

            actualCamera = CameraPositionCustomization;
        }
    }

    void Update()
    {
        //if (actualCamera == CameraPositionCustomization && isOrbiting)
        //{
        //    UpdateOrbit();
        //}

        if (opcionesJugador != null && !isDashing && opcionesJugador.movimientoActivado && IsServer)
        {
            if (canBounce)
            {
                UpdateMovement();
            }
            UpdateRotation();
        }
    }

    private void UpdateMovement()
    {
        Vector2 previousVelocity = velocity;

        // Si hay dirección de movimiento, acelerar
        if (direccionMovimiento != Vector3.zero && canBounce)
        {
            Vector2 forwardDirection = new Vector2(transform.forward.x, transform.forward.z).normalized;

            if (previousVelocity == Vector2.zero)
            {
                //Si estás quieto empiezas con boost de velocidad
                velocity = forwardDirection * initialSpeed;
            }
            else
            {
                // Aumenta la velocidad en la dirección de movimiento
                velocity = forwardDirection * velocity.magnitude + (forwardDirection * (acceleration * Time.deltaTime));
            }

            // Limitar la magnitud de la velocidad a maxSpeed
            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }
        }
        else
        {
            // Decelerar para detenerse si no hay input de movimiento
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);

            //velocity -= deceleration * Time.deltaTime * velocity;
            if (velocity.magnitude < 0.05f)
            {
                velocity = Vector2.zero;
            }
        }

        GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, 0, velocity.y);

        // Aplicar la velocidad calculada a la posición del objeto
        //transform.position += new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime;

        // Aplica la pasiva de la nave si ocurre todo el rato
        if (playerShip.passiveAbility is StatBuffPassive)
        {
            playerShip.passiveAbility.Execute();
        }
    }

    private void UpdateRotation()
    {
        if(rotationInput != 0)
        {
            if (rotationSpeed == 0)
            {
                rotationSpeed += initialRotationSpeed;
            }
            else
            {
                // Aumentar la velocidad de rotación con aceleración
                rotationSpeed += rotationAcceleration * Time.deltaTime;

                // Limitar la velocidad de rotación al máximo permitido
                rotationSpeed = Mathf.Min(rotationSpeed, maxRotationSpeed);
            }

            transform.Rotate(0, rotationInput * rotationSpeed * Time.deltaTime, 0);

            if (velocity.magnitude > 0.01f)
            {
                float angle = Vector3.Angle(transform.forward, velocity.normalized);

                float reductionFactor = Mathf.Clamp01(1 - (angle / 180f));

                // Multiplica el factor de reducción para hacer que el efecto sea más o menos fuerte
                velocity *= Mathf.Lerp(1.0f, reductionFactor, reductionMultiplier);
            }
        }
        else
        {
            rotationSpeed = 0;
        }
    }

    //INPUT DEL JUGADOR
    public void Move()
    {
        if (!startedMoving)
        {
            stoppedMoving = false;
            startedMoving = true;
            AccelerateVFXClientRpc(true);
        }

        direccionMovimiento = Vector3.forward; // Apuntar hacia adelante
        AccelerateSFXClientRpc(true);
    }

    public void Stop()
    {
        if (!stoppedMoving)
        {
            stoppedMoving = true;
            startedMoving = false;
            AccelerateVFXClientRpc(false);
        }

        direccionMovimiento = Vector3.zero;
        AccelerateSFXClientRpc(false);
    }

    public void Rotate(float input)
    {
        rotationInput = (int) input;
    }

    public void StopRotating()
    {
        rotationInput = 0;
    }


    // CLIENT RPC DE SFX Y VFX
    [ClientRpc]
    public void AccelerateSFXClientRpc(bool play)
    {
        if (play)
        {
            // Si el AudioSource no ha sido asignado o está detenido, reproducir el sonido
            if (accelerationAudioSource == null || !accelerationAudioSource.isPlaying)
            {
                // Asignar el AudioSource devuelto por PlaySFX
                accelerationAudioSource = AudioManager.Instance.PlaySFX(accelerateSFX, transform.position);
            }
        }
        else
        {
            // Detener el sonido si está activo
            if (accelerationAudioSource != null)
            {
                AudioManager.Instance.StopSFX(accelerationAudioSource);
                accelerationAudioSource = null; // Liberar referencia
            }
        }
    }

    [ClientRpc]
    public void AccelerateVFXClientRpc(bool active)
    {
        if (!IsServer)
        {
            playerShip.ToggleFireVFX(active);
        }
    }


    [ClientRpc]
    public void LowHealthVFXClientRpc(bool active)
    {
        if (!IsServer)
        {
            playerShip.ToggleLowHealthVFX(active);
        }
    }

    [ClientRpc]
    public void PlayCollisionSFXClientRpc()
    {
        AudioManager.Instance.PlaySFX(collisionSFX, transform.position);
    }


    // DETECCIÖN DE COLISIONES
    private void OnCollisionEnter(Collision collision)
    {
        //COLLSION SFX
        PlayCollisionSFXClientRpc();

        if (isDashing)
        {
            DashAbility dash = playerShip.activeAbility as DashAbility;
            if(dash != null)
            {
                dash.CollidesWith(collision);
                return;
            }
        }

        //SPEED CHANGES
        if (velocity.magnitude > maxSpeed / 3 && collision.gameObject.GetComponent<IProyectil>() == null)
        {
            //Daño por impacto, y pasivas OnCollision
            if (playerShip.passiveAbility is not OnCollisionPassive)
            {
                Debug.Log("Daño por choque" + opcionesJugador.controladorDelJugador);
                opcionesJugador.controladorDelJugador.GetDamage((int)(velocity.magnitude * maxSpeedDmg / maxSpeed), opcionesJugador.controladorDelJugador);
            }
            else
            {
                OnCollisionPassive passive = playerShip.passiveAbility as OnCollisionPassive;

                if (passive.recievesCollisionDamage)
                {
                    opcionesJugador.controladorDelJugador.GetDamage((int)(velocity.magnitude * maxSpeedDmg / maxSpeed), opcionesJugador.controladorDelJugador);
                }
                passive.CollideWith(collision);
            }

            //Si te chocas contra un IDamageable
            var enemy = collision.gameObject.GetComponentInParent<IDamageable>();
            if (enemy != null)
            {
                enemy.GetDamage((int)(velocity.magnitude * maxSpeedDmg / maxSpeed), opcionesJugador.controladorDelJugador);
            }

            if(canBounce)
            {
                // Calcula la dirección del rebote
                Vector3 collisionNormal = collision.contacts[0].normal;
                targetDirection = Vector3.Reflect(velocity.normalized, collisionNormal); // Dirección reflejada

                // Actualiza la velocidad
                velocity = targetDirection * velocity.magnitude / 2;

                // Actualiza la velocidad de rotación;
                rotationSpeed = rotationSpeed / 2;

                canBounce = false;
                StartCoroutine("SetCanBounce", true);
            }

            GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, 0, velocity.y);
        }
    }

    public IEnumerator SetCanBounce(bool toSet)
    {
        Debug.Log("Espera para colisionar");
        yield return new WaitForSeconds(0.2f); // Delay
        canBounce = toSet;
    }

    public void SetToSpawn(GameObject spawnPoint, bool enSpawnGlobal)
    {
        if (IsServer)
        {
            //Debug.Log("Set To Spawn: " + spawnPoint.transform.position);
            if(enSpawnGlobal)
                transform.position = spawnPoint.transform.position + new Vector3(0,0,30 * OwnerClientId);
            else
                transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
            velocity = Vector3.zero;
        }
    }

    public void AssignShip(GameObject ship)
    {
        playerShip = ship.GetComponent<PlayerShip>();

        initialSpeed = playerShip.initialSpeed;
        maxSpeed = playerShip.maxSpeed;

        // Eliminamos cualquier CapsuleCollider existente en el padre
        if (colliderNave != null)
        {
            Destroy(colliderNave.gameObject);
        }

        // Obtenemos el CapsuleCollider del hijo, asumiendo que siempre estará presente
        GameObject colliderHijo = ship.gameObject.GetComponentInChildren<MeshCollider>().gameObject;
        if (colliderHijo != null)
        {
            // Agregamos un nuevo CapsuleCollider en el padre y copiamos las propiedades del hijo
            colliderNave = Instantiate(colliderHijo, transform).GetComponent<MeshCollider>();

            // Desactivamos el CapsuleCollider en el hijo para evitar duplicidad en colisiones
            colliderHijo.SetActive(false);

            //Debug.Log("CapsuleCollider del hijo copiado al padre y desactivado en el hijo.");
        }
        else
        {
            Debug.LogWarning("La nave asignada no tiene un collider.");
        }
    }

    public void AssignMainCamera(CameraType cameraType)
    {
        CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();

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

                    //isOrbiting = false;
                    break;

                case CameraType.Customization:
                    actualCamera = CameraPositionCustomization;
                    CameraPositionCustomization.GetComponent<CustomizationCamPos>().SetCustomizationCamPos(opcionesJugador.controladorDelJugador.selectedShip.Value);

                    VC.m_Lens.Orthographic = false;

                    VC.Follow = null; // Detach Follow para manejo manual
                    VC.LookAt = CameraTarget.transform;

                    // Ajusta la posición inicial de la cámara
                    VC.transform.position = CameraPositionCustomization.transform.position;

                    //isOrbiting = true;
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No se encontró la cámara virtual Cinemachine.");
        }
    }

    private void UpdateOrbit()
    {
        //CinemachineVirtualCamera VC = FindObjectOfType<CinemachineVirtualCamera>();

        //// Actualiza el ángulo horizontal automáticamente
        //orbitAngles.x += orbitSpeed * Time.deltaTime;

        //// Convierte los ángulos en posición de cámara
        //Quaternion rotation = Quaternion.Euler(orbitAngles.y, orbitAngles.x, 0);
        //Vector3 offset = rotation * Vector3.back * orbitDistance;

        //// Mantén la altura fija
        //Vector3 cameraPosition = CameraTarget.transform.position + offset;
        //cameraPosition.y = CameraPositionCustomization.transform.position.y;

        //// Ajusta la posición y la rotación de la cámara
        //VC.transform.position = cameraPosition;
        //VC.transform.LookAt(CameraTarget.transform);
    }


}