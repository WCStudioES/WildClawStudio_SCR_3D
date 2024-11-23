using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class NetworkedPlayer : NetworkBehaviour, IDamageable
{
    //CONTROLADOR DE LA NAVE
    [SerializeField] public ControladorNave nave;
    public GameObject cuerpoNave;  //Gameobject con el collider de la nave, evita autohit
    public Partida partida;
    
    //INFORMACIÓN DEL USUARIO
    public string userName;
    public int battlePassLevel;
    //CONTIENE LAS OPCIONES DE CONFIGURACION DEL JUGADOR
    public OpcionesJugador opcionesJugador;

    //LISTA CON LAS NAVES Y CUÁLES ESTÁN DESBLOQUEADAS
    public List<GameObject> allShips;
    public List<int> unlockedShips;
    public NetworkVariable<int> selectedShip;

    //LISTA CON LOS PROYECTILES Y LOS QUE ESTÁN DISPONIBLES PARA CADA NAVE
    public List<GameObject> allProjectiles;
    public List<int> possibleProjectiles;
    public int selectedProjectile;

    //LISTA CON LOS OBJETOS DE APOYO
    public List<GameObject> allSupport;

    //INDICA SI LA NAVE HA SIDO DESTRUIDA O NO
    //TODO AÚN SIN UTILIZAR, UTILIZAR CUANDO SE IMPLEMENTE PERDER VIDA
    public bool naveDestruida = false;
    
    //VARIABLES DE DISPARO
    private int proyectilBasico = 0; // Prefab del proyectil basico
    private int proyectilMejorado; // Prefab del proyectil escogido
    
    //Variables de apoyo
    private bool canUseAbility;     //Bool para saber si puedes usar la habilidad
    private bool isSupportAvailable;    //Bool para saber si se aplica el apoyo
    
    [SerializeField] private Transform[] puntoDisparo; // Lugar donde se instancia el proyectil
    
    private bool isShooting = false; //Booleano para saber si el jugador esta disparando
    private float shotTimer = 0f; // Temporizador para la cadencia
    
    
    //VARIABLES DE MOVIMIENTO
    public bool isMoving = false; // Indica si la tecla de movimiento está presionada
    private float rotationInput = 0f; // Almacena el input de rotación
    
    //VARIABLES DE ESTADISTICAS
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(100);      //Vida maxima de la nave
    public NetworkVariable<int> actualHealth = new NetworkVariable<int>(100);   //Vida de la nave
    public NetworkVariable<int> armor = new NetworkVariable<int>(10);           //Armadura de la nave
    public NetworkVariable<int> dmgBalance = new NetworkVariable<int>(0);       //Daño porcentual extra o reducido de la nave
    public NetworkVariable<int> xp = new NetworkVariable<int>(00);              //Experiencia de la nave
    public NetworkVariable<int> lvl = new NetworkVariable<int>(1);              //Nivel de la nave
    public NetworkVariable<int> projectile = new NetworkVariable<int> (0);      //Proyectil usado
    public NetworkVariable<int> selectedSupport = new NetworkVariable<int> (0);         //Objeto de apoyo usado
    public NetworkVariable<int> xpADar = new NetworkVariable<int>(200);         //Experiencia que da al otro jugador al destruir tu nave

    
    // UI
    public Image barraDeVida; //Imagen de la barra de vida
    [SerializeField]private TextMeshProUGUI textoVida; //Texto dentro de la barra de vida

    public Image barraDeExperiencia; //Imagen de la barra de experiencia
    [SerializeField] private TextMeshProUGUI textoExperiencia; //Texto dentro de la barra de experiencia
    
    [SerializeField] private TextMeshProUGUI textoNivel; //Texto que muestra el nivel de la nave
    public UIBoosters uiBoosters;
    
    [SerializeField] private Animator animator;
    [SerializeField] private Animator animator2;
    
    //UI de barra enemiga 
    [SerializeField] private Image circuloDeVidaEnemigo;
    [SerializeField] private TextMeshProUGUI nivelEnemigo;
    [SerializeField] private Canvas UIEnemigo;
    
    
    
    //public int equipo;  Para luego que no haya fuego amigo entre equipos

    private void Start()
    {
        animator.Play(1);
        if (IsOwner)
        {
            OnPlayerStartServerRpc();
            if(!IsHost)
            nave.AssignMainCamera(ControladorNave.CameraType.Customization);
            UIEnemigo.gameObject.SetActive(false);
        }
        else
        {
            CambiarNave(allShips[selectedShip.Value], 0);
            UiEnemigoClientRpc();
        }
    }

    [ClientRpc]
    private void UiEnemigoClientRpc()
    {
        UIEnemigo.gameObject.SetActive(true);
        UIEnemigo.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Funcion para resetear los valores predefinidos antes de empezar partida
    public void ResetPrePartida()
    {
        if (IsServer)
        {
            PlayerShip playerShip = cuerpoNave.GetComponent<PlayerShip>();
            // Se inicializan las estadísticas de la nave elegida
            playerShip.InitializeStats();
            //playerShip.ResetRonda();

            // Se copian dentro del NetworkedPlayer como Network Variables
            armor.Value = playerShip.initialArmor;
            actualHealth.Value = playerShip.initialHealth;
            dmgBalance.Value = playerShip.dmgBalance;
            maxHealth.Value = actualHealth.Value;

            // Otras inicializaciones
            xp.Value = 0;
            lvl.Value = 1;
            projectile.Value = proyectilBasico;
            canUseAbility = false;
            isSupportAvailable = false;

            //Inicializar UI
            UpdateHealthBarClientRpc(actualHealth.Value, maxHealth.Value);
            UpdateExperienceBarClientRpc(0, 1);
            uiBoosters.activeAbility.gameObject.SetActive(false);
            uiBoosters.supportAbility.gameObject.SetActive(false);
        }
    }
    
    //RESTAURA LA POSICIÓN DE LAS NAVES Y SU VIDA
    //TODO RESTAURAR EXPERIENCIA Y NIVEL DEL ARMA
    public void resetDeRonda(GameObject spawnPosition)
    {
        if (IsServer)
        {
            //DEVUELVE LA NAVE A LA POSICION DE SPAWN
            nave.SetToSpawn(spawnPosition, false);
            nave.velocity = Vector3.zero;

            //RESTAURA LA VIDA DE LA NAVE
            actualHealth.Value = maxHealth.Value;

            if(isSupportAvailable)
                ApplySuppItem();
        }
        cuerpoNave.GetComponent<PlayerShip>().ResetRonda();
        UpdateHealthBarClientRpc(actualHealth.Value, maxHealth.Value);
    }

    public void ApplySuppItem()
    {
        allSupport[selectedSupport.Value].GetComponent<SupportItem>().owner = this;

        switch (selectedSupport.Value)
        {
            case 0:
                // Destruye cualquier escudo anterior
                if (GetComponentInChildren<VisualShield>() != null)
                {
                    Destroy(GetComponentInChildren<VisualShield>().gameObject);
                }

                // Aplica el nuevo escudo en el servidor
                allSupport[selectedSupport.Value].GetComponent<SupportItem>().AddToPlayer();
                break;

            case 1:
                allSupport[selectedSupport.Value].GetComponent<SupportItem>().AddToPlayer();
                break;

            default:
                break;
        }
    }

    public void ResetSupportItems()
    {
        switch (selectedSupport.Value)
        {
            case 0:
                // Destruye cualquier escudo anterior
                if (GetComponentInChildren<VisualShield>() != null)
                {
                    Destroy(GetComponentInChildren<VisualShield>().gameObject);
                }
                break;

            case 1:
                allSupport[selectedSupport.Value].GetComponent<CDReductionSupportItem>().applied = false;
                break;

            default:
                break;
        }
    }

    //Funcion que maneja la barra de vida
    [ClientRpc]
    public void UpdateHealthBarClientRpc(int vida, int maxVida)
    {
        float healthPercentage = (float) vida / (float) maxVida;
        if (IsOwner)
        {
            barraDeVida.fillAmount = healthPercentage;
        
            textoVida.text = vida + " / " + maxVida;
        }
        else
        {
            circuloDeVidaEnemigo.fillAmount = healthPercentage;
        }
        

        //Debug.Log(healthPercentage);
    }
    
    //Funcion que maneja la barra de experiencia
    [ClientRpc]
    public void UpdateExperienceBarClientRpc(int experience, int lvl)
    {
        if (cuerpoNave.GetComponent<PlayerShip>().xpByLvl.Length != 0)
        {
            float maxExperience = (float) cuerpoNave.GetComponent<PlayerShip>().xpByLvl[lvl - 1];
            float xpPercentage = experience / maxExperience;
            barraDeExperiencia.fillAmount = xpPercentage;
        
            textoExperiencia.text = experience.ToString() + " / " + maxExperience.ToString();
            textoNivel.text = lvl.ToString();
            //Debug.Log(healthPercentage);
        }
        
        Debug.Log("UpodateClientLevel");
        if (!IsOwner)
        {
            nivelEnemigo.text = lvl.ToString();
            Debug.Log("UpodateClientLevelAA");
        }
    }

    private void Update()
    {
        //Debug.Log("Transform NetworkedPlayer: " + this.transform.position);
        if (!opcionesJugador.movimientoActivado)
        {
            isMoving = false;
            rotationInput = 0.0f;
            isShooting = false;
        }

        if (IsOwner && opcionesJugador.movimientoActivado)
        {
            // Llama a la función de movimiento continuamente si la tecla de movimiento está presionada
            if (isMoving)
            {
                OnMoveServerRpc();
            }
            else
            {
                OnStopMoveServerRpc();
            }

            // Llama a la función de rotación si hay input de rotación
            if (rotationInput != 0f)
            {
                //Debug.Log("ROTANDO");
                OnRotateServerRpc(rotationInput);
            }

            //Disparos
            if (isShooting)
            {
                //Debug.Log("Disparando");
                if (ComprobadorDeCadencia()) // Verifica la cadencia en el cliente
                {
                    OnShootServerRpc(); // Solo llama a esta función si se cumple la cadencia
                }
            }
            shotTimer += Time.deltaTime;
        }
    }

    // RECOGE LOCALMENTE EL INPUT DEL JUGADOR
    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log(context.action.name);
        if (IsOwner && opcionesJugador.movimientoActivado)
        {
            switch (context.action.name)
            {
                case "Move":
                    if (context.performed)
                    {
                        isMoving = true;  // Activa el movimiento continuo cuando la tecla se presiona
                    }
                    else if (context.canceled)
                    {
                        isMoving = false; // Detiene el movimiento cuando se suelta la tecla
                    }
                    break;

                case "Shoot":
                    if (context.performed)
                    {
                        isShooting = true;
                        Debug.Log("Disparando");
                    }
                    else if (context.canceled)
                    {
                        isShooting = false;
                    }
                    break;

                case "Rotate":
                    if (context.performed)
                    {
                        rotationInput = context.ReadValue<float>(); // Almacena el valor de rotación
                    }
                    else if (context.canceled)
                    {
                        rotationInput = 0f; // Detiene la rotación cuando se suelta la tecla
                        OnRotateServerRpc(rotationInput);
                    }
                    //Debug.Log(rotationInput);
                    break;

                case "Hability":
                    if (context.performed)
                    {
                       HabilityServerRpc();
                    }
                    break;
            }
        }
    }

    // Función que aplica daño a la nave CONTANDO ARMADURA
    public void GetDamage(int dmg, NetworkedPlayer dueñoDaño)
    {
        if (partida.rondaEnmarcha)
        {
            //Restar el daño
            actualHealth.Value -= (dmg - dmg * armor.Value / 100);  // Resta la cantidad de daño a la vida de la nave

            // Si la vida llega a 0, destruye la nave (puedes modificar esto para otro comportamiento)
            if (actualHealth.Value <= 0)
            {
                dueñoDaño.GetXP(xp.Value);
                Debug.Log("Xp de jugador: " + dueñoDaño.xp.Value);
                //Pierdes;
            }

            //Debug.Log("Vida actual de la nave: " + actualHealth);
            UpdateHealthBarClientRpc(actualHealth.Value, maxHealth.Value); //Actualizar barra de vida
                  
            ChangeMaterialColorClientRpc(Color.red, 0.1f);
        }
    }

    // Funnción que aplica daño a la nave SIN CONTAR ARMADURA
    public void GetTrueDamage(int dmg, NetworkedPlayer dueñoDaño)
    {
        //Restar el daño
        actualHealth.Value -= dmg;  // Resta la cantidad de daño a la vida de la nave

        // Si la vida llega a 0, destruye la nave (puedes modificar esto para otro comportamiento)
        if (actualHealth.Value <= 0)
        {
            dueñoDaño.GetXP(xp.Value);
            Debug.Log("Xp de jugador: " + dueñoDaño.xp.Value);
            //Pierdes;
        }
        else
        {
            // Llamar al ClientRpc para cambiar el color del material
            ChangeMaterialColorClientRpc(Color.red, 0.1f);
        }

        //Debug.Log("Vida actual de la nave: " + actualHealth);
        UpdateHealthBarClientRpc(actualHealth.Value, maxHealth.Value); //Actualizar barra de vida
    }

    [ClientRpc]
    private void ChangeMaterialColorClientRpc(Color hitColor, float duration)
    {
        StartCoroutine(cuerpoNave.GetComponent<PlayerShip>().FlashMaterialsInChildren(hitColor, duration));
    }

    public void GetHeal(int heal, NetworkedPlayer dueñoDaño)
    {
        int health = Mathf.Min(actualHealth.Value + heal, maxHealth.Value);
        actualHealth.Value = health;  // Suma la cantidad de daño a la vida de la nave
        
        Debug.Log("Vida actual de la nave: " + actualHealth.Value);
        //Debug.Log("Vida actual de la nave: " + actualHealth);
        UpdateHealthBarClientRpc(health, maxHealth.Value); //Actualizar barra de vida
    }
    
    //Funcion que gestiona la obtención de xp del jugador
    public void GetXP(int xpRecibida)
    {
        //Sumar experiencia
        xp.Value += xpRecibida;

        // Si no eres nivel máximo y tienes += experiencia necesaria, subes de nivel
        if (lvl.Value < cuerpoNave.GetComponent<PlayerShip>().maxLevel && 
            xp.Value >= cuerpoNave.GetComponent<PlayerShip>().xpByLvl[lvl.Value - 1])
        {
            
            AnimacionSubidaNivelCLientRpc();
            // La xp se resta, para que vuelva a estar cerca de 0
            xp.Value -= cuerpoNave.GetComponent<PlayerShip>().xpByLvl[lvl.Value - 1];
            lvl.Value++;
            
            // Cambios de estadísticas por nivel
            xpADar.Value += 50;
            armor.Value += cuerpoNave.GetComponent<PlayerShip>().armorIncrement;
            maxHealth.Value += cuerpoNave.GetComponent<PlayerShip>().healthIncrement;
            actualHealth.Value += cuerpoNave.GetComponent<PlayerShip>().healthIncrement;
            

            // Se actualiza la barra de vida

            Debug.Log("Level " + lvl.Value);

            // Mejoras de nivel
            switch(lvl.Value)
            {
                default:
                    break;

                //Proyectil mejorado
                case 2:
                    CambiarArma(proyectilMejorado);
                    break;
                case 3:
                    canUseAbility = true;
                    DesbloquearHabilidadClientRpc();
                    break;
                case 6:
                    isSupportAvailable = true;
                    ApplySuppItem();
                    DesbloquearApoyoClientRpc();
                    break;
                //...
            }
            
        }
        
        // Actualizar barras
        UpdateHealthBarClientRpc(actualHealth.Value, maxHealth.Value); 
        UpdateExperienceBarClientRpc(xp.Value, lvl.Value);  //Actualizar barra de experiencia

    }

    [ClientRpc]
    private void AnimacionSubidaNivelCLientRpc()
    {
        animator2.SetTrigger("SubidaNivel");
        animator.SetTrigger("SubidaNivel");
    }
    
    [ClientRpc]
    private void DesbloquearHabilidadClientRpc()
    {
        //activa la UI
        uiBoosters.activeAbility.gameObject.SetActive(true);
    }
    
    private void CambiarArma(int proyectilNuevo)
    {
        //Cambia la arma
        projectile.Value = proyectilNuevo;
        CambiarArmaClientRpc(proyectilNuevo);
    }
    
    
    [ClientRpc]
    private void CambiarArmaClientRpc(int proyectilNuevo)
    {
        uiBoosters.SetWeaponAbility(allProjectiles[proyectilNuevo].GetComponent<Proyectil>().sprite);
    }

    [ClientRpc]
    private void DesbloquearApoyoClientRpc()
    {
        //activa la UI
        uiBoosters.supportAbility.gameObject.SetActive(true);
    }

    // ACTUALIZA LA DIRECCIÓN DEL JUGADOR
    [ServerRpc]
    private void OnMoveServerRpc()
    {
        nave.Move(); // Llama a la función de movimiento en la nave
    }

    [ServerRpc]
    private void OnStopMoveServerRpc()
    {
        nave.Stop(); // Llama a la función de movimiento en la nave
    }

    // GESTIONA LA ROTACIÓN DE LA NAVE
    [ServerRpc]
    private void OnRotateServerRpc(float input)
    {
        nave.Rotate(input); // Llama a la función de rotación en la nave
    }

    // LLEVA AL JUGADOR A LA POSICIÓN DE SPAWN
    [ServerRpc]
    private void OnPlayerStartServerRpc()
    {
        //nave.SetToSpawn();
    }

    // PARA AL JUGADOR
    [ServerRpc]
    public void GameEndServerRpc()
    {
        nave.Stop();
    }

    // GESTIONA EL DISPARO DE PROYECTILES

    private bool ComprobadorDeCadencia()
    {
        //Debug.Log("Cadencia");

        // Verificar si ya ha pasado el tiempo suficiente para disparar nuevamente
        if (shotTimer >= allProjectiles[projectile.Value].GetComponent<Proyectil>().cadencia)
        {
            // Reiniciar el temporizador
            shotTimer = 0f;

            //Debug.Log("ComprobadorCadencia aceptado");
            return true;
        }
        else
        {
            //Debug.Log("ComprobadorCadencia denegado: " + shotTimer + ", " + cadenciaDeDisparo);
            return false;
        }
    }

    [ServerRpc]
    private void OnShootServerRpc()
    {
        // Dependiendo del tipo de proyectil, gestiona uno o dos disparos
        if (allProjectiles[projectile.Value].GetComponent<Proyectil>().type == Proyectil.Type.Double) // Proyectil doble
        {
            CrearYEnviarProyectil(puntoDisparo[1].position, puntoDisparo[1].rotation, puntoDisparo[1].forward);
            CrearYEnviarProyectil(puntoDisparo[2].position, puntoDisparo[2].rotation, puntoDisparo[2].forward);
        }
        else // Proyectil único
        {
            //Debug.Log("DISPARANDO EN SERVIDOR");
            CrearYEnviarProyectil(puntoDisparo[0].position, puntoDisparo[0].rotation, puntoDisparo[0].forward);
        }
    }

    // Método auxiliar para crear el proyectil en el servidor y replicarlo en los clientes
    private void CrearYEnviarProyectil(Vector3 posicion, Quaternion rotacion, Vector3 direccion)
    {
        //Debug.Log("Proyectil creado");
        GameObject proyectil = Instantiate(allProjectiles[projectile.Value], posicion, rotacion);
        Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();

        // Inicializamos el proyectil en el servidor
        proyectilScript.Inicializar(direccion, nave.GetComponent<CapsuleCollider>(), this, IsServer);

        // Programamos la destrucción del proyectil después de 2 segundos
        Destroy(proyectil, 2f);

        // Enviamos los datos de posición y dirección a los clientes
        SpawnProyectilClientRpc(posicion, rotacion, direccion);
    }

    [ClientRpc]
    private void SpawnProyectilClientRpc(Vector3 posicion, Quaternion rotacion, Vector3 direccion)
    {
        if (!IsServer) // Solo se ejecuta en los clientes
        {
            //Debug.Log("DISPARANDO EN CLIENTE");
            GameObject proyectil = Instantiate(allProjectiles[projectile.Value], posicion, rotacion);

            // Configura la dirección del proyectil en el cliente
            Proyectil proyectilScript = proyectil.GetComponent<Proyectil>();
            proyectilScript.Inicializar(direccion, nave.GetComponent<CapsuleCollider>(), this, IsServer);
            uiBoosters.UpdateWeaponImage(allProjectiles[projectile.Value].GetComponent<Proyectil>().cadencia);

            // El proyectil se destruirá automáticamente tras 2 segundos en el cliente
            Destroy(proyectil, 2f);
        }
    }
    
    
    //////////////////////////////////
    /// USO DE HABILIDADES////////////
    //////////////////////////////////
    [ServerRpc]
    private void HabilityServerRpc()
    {
        if (IsServer && canUseAbility)
        {
            Debug.Log("Lanzando habilidad en el servidor");

            //METER CODIGO DE SERVIDOR AQUI
            switch (cuerpoNave.GetComponent<PlayerShip>().activeAbility.type)
            {
                case ActiveAbility.ActiveType.MovementBuff: 
                case ActiveAbility.ActiveType.TogglePassive:
                case ActiveAbility.ActiveType.Shield:
                    cuerpoNave.GetComponent<PlayerShip>().UseAbility();
                    break;

                default:
                    cuerpoNave.GetComponent<PlayerShip>().UseAbility();
                    AbilityClientRpc();
                    break;
            }
        
        }
    }
    
    //Metodo para actualizar la UI de la habilidad en el cliente
    [ClientRpc]
    private void AbilityClientRpc()
    {
        if (!IsServer)
        {
            PlayerShip playerShip = cuerpoNave.GetComponent<PlayerShip>();
            //METER CODIGO DE CLIENTE AQUI
            Debug.Log("Lanzando habilidad en el cliente");
            playerShip.UseAbility();
        }
    }

    //Metodo para actualizar la UI de la habilidad en el cliente con un contador ahcia atras para su CD
    [ClientRpc]
    public void UpdateCDAbilityUIClientRpc(float value)
    {
        uiBoosters.UpdateActiveImageWithCD(value);
    }
    
    //Metodo para actualizar la UI de la habilidad sin CD con un color
    [ClientRpc]
    public void UpdateAbilityUIClientRpc(float value, Color color)
    {
        uiBoosters.UpdateActiveImage(value, color);
    }
    
    //Metodo para actualizar la UI de la habilidad sin CD sin color
    [ClientRpc]
    public void UpdateAbilityUIClientRpc(float value)
    {
        uiBoosters.UpdateActiveImage(value);
    }
    
    //Metodo para actualizar el color de la activa
    [ClientRpc]
    public void UpdateAbilityUIClientRpc(Color color)
    {
        uiBoosters.UpdateActiveImage(color);
    }


    //////////////////////////////////
    /// PERSONALIZACIÓN DE LA NAVE ///
    //////////////////////////////////

    // ServerRpc que registra los cambios y los propaga a otros clientes
    [ServerRpc]
    public void ApplyCustomizationServerRpc(int shipIndex, int skinIndex, int projectileIndex, int supportIndex)
    {
        //Debug.Log(shipIndex + ", " + allShips.Count);
        selectedShip.Value = shipIndex;
        selectedProjectile = projectileIndex;
        selectedSupport.Value = supportIndex;

        ApplyCustomizationLocally(shipIndex, skinIndex, projectileIndex, supportIndex);

        ApplyCustomizationClientRpc(shipIndex, skinIndex, projectileIndex, supportIndex, OwnerClientId);
    }

    // Aplicación local de la personalización (solo en el propietario)
    public void ApplyCustomizationLocally(int shipIndex, int skinIndex, int projectileIndex, int supportIndex)
    {
        GameObject selectedShipPrefab = allShips[shipIndex];

        CambiarNave(selectedShipPrefab, skinIndex);  // Cambia la nave del propietario
        CambiarArmaMejorada(projectileIndex);  // Cambia el proyectil del propietario

        //Debug.Log($"Personalización aplicada localmente en el propietario: Nave {shipIndex}, Proyectil {projectileIndex}");
    }

    [ClientRpc]
    private void ApplyCustomizationClientRpc(int shipIndex, int skinIndex, int projectileIndex, int supportIndex, ulong ownerClientId)
    {
        // Verifica si este cliente es el propietario que llamó al ServerRpc
        if (NetworkManager.Singleton.LocalClientId == ownerClientId)
        {
            ApplyCustomizationLocally(shipIndex, skinIndex, projectileIndex, supportIndex);
            
            uiBoosters.SetActiveAbility(allShips[shipIndex].GetComponent<PlayerShip>().activeAbility.Sprite);
            uiBoosters.SetWeaponAbility(allProjectiles[0].GetComponent<Proyectil>().sprite);
            uiBoosters.SetSupportAbility(allSupport[selectedSupport.Value].GetComponent<SupportItem>().suppItemSprite);
            
            
            //Debug.Log($"Personalización aplicada en el cliente propietario: Nave {shipIndex}, Proyectil {projectileIndex}");
        }
        else if(!IsOwner)
        {
            CambiarNave(allShips[shipIndex], skinIndex);
        }
    }

    // Método para cambiar la nave en el propietario
    private void CambiarNave(GameObject navePrefab, int skinIndex)
    {
        if (cuerpoNave != null)
        {
            Destroy(cuerpoNave); // Destruye la nave anterior si existe
        }
        cuerpoNave = Instantiate(navePrefab, nave.transform.position, nave.transform.rotation);
        cuerpoNave.GetComponent<PlayerShip>().ChangeSkin(skinIndex);
        cuerpoNave.transform.SetParent(nave.transform);

        cuerpoNave.GetComponent<PlayerShip>().shipController = nave;

        cuerpoNave.transform.position = Vector3.zero;
        cuerpoNave.transform.localRotation = Quaternion.identity;

        nave.AssignShip(cuerpoNave);

        for (int i = 0; i < puntoDisparo.Length; i++)
        {
            puntoDisparo[i] = cuerpoNave.GetComponent<PlayerShip>().proyectileSpawns[i].transform;
        }
    }

    private void CambiarArmaMejorada(int proyectilNuevo)
    {
        proyectilMejorado = proyectilNuevo;
        uiBoosters.SetWeaponAbility(allProjectiles[proyectilNuevo].GetComponent<Proyectil>().sprite);
    }
}
