using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;


    [Header("Camera Variables")]
    [SerializeField] private Transform aimTargetTr;


    [Header ("Gravity Varaibles")]
    [SerializeField] private float groundRayDistance;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStength;
    [SerializeField] private float rideSpringDamper;
    private RaycastHit groundHit;
    [SerializeField] private float gravityForce;
    [SerializeField] private float maxFallSpeed;


    [Header("Ground Check Variables")]
    [SerializeField] private Transform groundCheckOriginTr;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundCheckLayersToCheck;
    private bool isGrounded;


    [Header("Move Varaibles")]
    [SerializeField] private float m_maxSpeed;
    public float maxSpeed
    {
        get { return m_maxSpeed; }
        set
        {
            m_maxSpeed = value;
            UIManager.Instance.SetCurrentSpeedText(m_maxSpeed.ToString());
        }
    }
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float maxAccelerationForce;
    private bool movePressed;
    private bool canMove = true;

    private Vector2 moveDir;
    private Vector2 lastMoveDir;
    private float currentMaxSpeed;

    [Header("Aim Varaibles")]
    [SerializeField] private GameObject aimDirAidGO;
    private bool aimPressed;
    private Vector2 aimDir;
    private Vector2 shootDir;


    [Header("Shoot Variables")]
    [SerializeField] private float m_shootChargeTime;
    public float shootChargeTime
    {
        get { return m_shootChargeTime; }
        set
        {
            m_shootChargeTime = value;
            UIManager.Instance.SetCurrentChargeTimeText(m_shootChargeTime.ToString());
        }
    }
    private float currentChargeTime;
    [SerializeField] private float moveDirShootInertia;
    private bool shootCD = false;
    [SerializeField] private float shootCDTime;


    [Header("Projectile Variables")]
    [SerializeField] private GameObject projectileGO;
    [SerializeField] private Transform porjectileSpawnPos;
    [SerializeField] private float m_projectileSize;
    public float projectileSize
    {
        get { return m_projectileSize; }
        set
        {
            m_projectileSize = value;
            UIManager.Instance.SetCurrentProjectileSizeText(m_projectileSize.ToString());
        }
    }

    [SerializeField] private float m_projectileSpeed;
    public float projectileSpeed
    {
        get { return m_projectileSpeed; }
        set
        {
            m_projectileSpeed = value;
            UIManager.Instance.SetCurrentProjectileSpeedText(m_projectileSpeed.ToString());
        }
    }
    private GameObject currentProjectileGO;

    [SerializeField] private float m_projectileRange;
    public float projectileRange
    {
        get { return m_projectileRange; }
        set 
        {
            m_projectileRange = value;
            UIManager.Instance.SetCurrentRangeText(m_projectileRange.ToString()); 
        }
    }

    [SerializeField] private int m_projectileDamage;
    public int projectileDamage
    {
        get { return m_projectileDamage; }
        set
        {
            m_projectileDamage = value;
            UIManager.Instance.SetCurrentDamageText(m_projectileDamage.ToString());
        }
    }


    [Header("Ammo Variables")]
    [SerializeField] private int m_maxBullets;
    public int maxBullets
    {
        get { return m_maxBullets; }
        set
        {
            m_maxBullets = value;
            UIManager.Instance.SetBulletsText(m_currentBullets.ToString() + "/" + m_maxBullets.ToString());
            UIManager.Instance.SetNewMaxBulletsImg(m_maxBullets);
        }
    }
    private int m_currentBullets;
    public int currentBullets
    {
        get { return m_currentBullets; }
        set 
        {
            if(value < m_currentBullets)
            {
                UIManager.Instance.SetUsedBulletsImg(m_currentBullets);
            }
            else
            {
                UIManager.Instance.SetReloadedBulletsImg(value, maxBullets);
            }
            m_currentBullets = value;
            UIManager.Instance.SetBulletsText(m_currentBullets.ToString() + "/" + m_maxBullets.ToString());
        }
    }


    [Header("Reload Variables")]
    [SerializeField] private float m_reloadBarSpeed;
    private float reloadBarSpeed
    {
        get { return m_reloadBarSpeed; }
        set
        {
            m_reloadBarSpeed = value;
            UIManager.Instance.SetReloadValueBar(m_reloadBarSpeed);
        }
    }
    private float reloadBarCurrentValue = 0;
    [SerializeField] private float m_successReloadRate;
    private float successReloadRate
    {
        get { return m_successReloadRate; }
        set
        {
            m_successReloadRate = value;
            UIManager.Instance.SetReloadSuccessBar(m_successReloadRate);
        }
    }
    [SerializeField] private int extraBulletsOnSuccess;
    private bool isReloading = false;

    [Header("Dash Variables")]
    [SerializeField] private float m_dashTime;
    public float dashTime
    {
        get { return m_dashTime; }
        set 
        {
            m_dashTime = value;      
        }
    }
    [SerializeField] private float m_dashSpeed;
    public float dashSpeed
    {
        get { return m_dashSpeed; }
        set
        {
            m_dashSpeed = value;
        }
    }
    private bool isDashing = false;

    [Header("Fly Variables")]
    [SerializeField] private float m_maxFuel;
    public float maxFuel
    {
        get { return m_maxFuel; }
        set
        {
            m_maxFuel = value;
            UIManager.Instance.SetFlyFuelSliderMaxValue(m_maxFuel);
        }
    }
    private float m_currentFuel = 0;
    private float currentFuel
    {
        get { return m_currentFuel; }
        set
        {
            m_currentFuel = value;
            UIManager.Instance.SetFlyFuelSlderValue(m_currentFuel);
        }
    }
    [SerializeField] private float m_fuelBurnSpeed;
    public float fuelBurnSpeed
    {
        get { return m_fuelBurnSpeed; }
        set
        {
            m_fuelBurnSpeed = value;
        }
    }
    [SerializeField] private float m_fuelRecoverSpeed;
    public float fuelRecoverSpeed
    {
        get { return m_fuelRecoverSpeed; }
        set
        {
            m_fuelRecoverSpeed = value;
        }
    }


    [SerializeField] private float m_flySpeed;
    public float flySpeed
    {
        get { return m_flySpeed; }
        set
        {
            m_flySpeed = value;
        }
    }
    private bool isFlying = false;

    private void Awake()
    {
        playerInput = new PlayerInput();
        HandleInput();

        rb = this.GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.currentPlayer = this;
        Camera.main.gameObject.GetComponent<FollowObject>().targetTr = this.gameObject.transform;

        maxBullets = m_maxBullets;
        currentBullets = maxBullets;
        successReloadRate = m_successReloadRate;
        maxFuel = m_maxFuel;
        currentFuel = maxFuel;
        currentMaxSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.levelStarted)
        {
            Aim();
            ChargeShot();
            Dash();
            ReloadQTE();
            //Fly();
        }
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.levelStarted)
        {
            if (!isDashing && !isFlying) AddGravityForce();
            GroundCheck();
            Movement();
            FloatOnGround();
        }        
    }


    private void MoveStarted()
    {

    }

    private void AimStarted()
    {
        if(currentBullets > 0)
        {
            aimDirAidGO.SetActive(true);
            Camera.main.gameObject.GetComponent<FollowObject>().targetTr = aimTargetTr;
        }
    }

    private void AimFinished()
    {
        Shoot();
        aimDirAidGO.SetActive(false);
        Camera.main.gameObject.GetComponent<FollowObject>().targetTr = this.gameObject.transform;
    }

    private void ReloadStarted()
    {
        if (!isReloading)
        {
            if (currentBullets <= 0)
            {
                ReloadBullets();
            }
            else
            {
                EnterDash();
            }
        }
        else
        {
            StopReloadQTE();
        }
        //EnterFly();
    }

    private void ReloadEnded()
    {
        EndFly();
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundCheckOriginTr.position, Vector3.down, out hit, groundCheckDistance, groundCheckLayersToCheck))
        {
            isGrounded = true;
        }
        else isGrounded = false;

        Debug.DrawRay(groundCheckOriginTr.position, Vector3.down * groundCheckDistance, Color.red);
    }

    private void AddGravityForce()
    {
        if (rb.linearVelocity.y > maxFallSpeed) rb.linearVelocity += new Vector3(0, gravityForce, 0);
        else rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxFallSpeed, rb.linearVelocity.z);
    }
    private void FloatOnGround()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out groundHit, groundRayDistance))
        {
            if (!groundHit.collider.isTrigger)
            {
                Vector3 vel = rb.linearVelocity;
                Vector3 rayDir = transform.TransformDirection(Vector3.down);

                Vector3 otherVel = Vector3.zero;
                Rigidbody hitbody = groundHit.rigidbody;
                if (hitbody != null)
                {
                    otherVel = hitbody.linearVelocity;
                }

                float rayDirVel = Vector3.Dot(rayDir, vel);
                float otherDirVel = Vector3.Dot(rayDir, otherVel);

                float relVel = rayDirVel - otherDirVel;

                float x = groundHit.distance - rideHeight;
                float springForce = (x * rideSpringStength) - (relVel * rideSpringDamper);

                rb.AddForce(rayDir * springForce);

                Debug.DrawLine(this.transform.position, this.transform.position + (rayDir * springForce / 2), Color.yellow);
            }
            
        }
    }

    Vector3 m_GoalVel;
    private void Movement()
    {
        if (canMove)
        {
            Vector3 unitGoal = new Vector3(moveDir.x, 0, moveDir.y);
            Vector3 goalVel = unitGoal * currentMaxSpeed;

            m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, accelerationSpeed * Time.fixedDeltaTime);

            Vector3 neededAccel = (m_GoalVel - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z)) / Time.fixedDeltaTime;

            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccelerationForce);
            rb.AddForce(neededAccel * rb.mass);

            if (movePressed && !aimPressed) this.transform.rotation = Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.y));
        }
    }

    private void Aim()
    {
        if (aimPressed)
        {
            this.transform.rotation = Quaternion.LookRotation(new Vector3(aimDir.x, 0, aimDir.y));
        }
    }

    private void ChargeShot()
    {
        if (aimPressed && !shootCD && !isDashing && !isFlying)
        {
            if(currentBullets > 0)
            {
                if (currentChargeTime <= 0)
                {
                    currentProjectileGO = Instantiate(projectileGO, porjectileSpawnPos.position, Quaternion.identity, porjectileSpawnPos);
                    currentProjectileGO.GetComponent<PlayerProjectile>().ProjectileSetUp(this, projectileDamage, projectileRange);
                }
                if (currentChargeTime <= shootChargeTime)
                {
                    currentChargeTime += Time.deltaTime;
                    currentProjectileGO.transform.localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 0.01f), new Vector3(projectileSize, projectileSize, projectileSize), (currentChargeTime / shootChargeTime));
                }
                else
                {
                    currentProjectileGO.GetComponent<PlayerProjectile>().SetCharged();
                }
            }
        }
    }

    private void Shoot()
    {
        if(currentChargeTime >= shootChargeTime)
        {
            currentProjectileGO.transform.parent = null;
            currentProjectileGO.GetComponent<PlayerProjectile>().LaunchProjectile(new Vector3(shootDir.x, 0, shootDir.y) + (new Vector3(moveDir.x, 0, moveDir.y) * moveDirShootInertia), projectileSpeed);
            currentProjectileGO = null;
            currentChargeTime = 0;
            --currentBullets;
        }
        else
        {
            ResetCharge();
        }
        StartCoroutine(ShootCD());
    }

    public void ResetCharge()
    {
        if(currentProjectileGO != null)
        {
            currentProjectileGO.transform.parent = null;
            Destroy(currentProjectileGO);
            currentProjectileGO = null;
        }
        currentChargeTime = 0;
    }

    private IEnumerator ShootCD()
    {
        shootCD = true;
        yield return new WaitForSeconds(shootCDTime);
        shootCD = false;
    }

    private void ReloadBullets()
    {
        reloadBarCurrentValue = 1;
        UIManager.Instance.SetReloadValueBar(reloadBarCurrentValue);
        UIManager.Instance.SetReloadQTEActive(true);
        isReloading = true;        
    }
    
    private void ReloadQTE()
    {
        if (isReloading)
        {
            reloadBarCurrentValue -= reloadBarSpeed * Time.deltaTime;
            UIManager.Instance.SetReloadValueBar(reloadBarCurrentValue);
            if (reloadBarCurrentValue <= 0) StopReloadQTE();
        }
    }
    private void StopReloadQTE()
    {
        if(reloadBarCurrentValue < successReloadRate + 0.1f && reloadBarCurrentValue > 0.1f)
        {
            currentBullets = maxBullets + extraBulletsOnSuccess;
        }
        else
        {
            currentBullets = maxBullets;
        }
        isReloading = false;
        UIManager.Instance.SetReloadQTEActive(false);
        reloadBarCurrentValue = 1;
    }



    Vector2 dashDir;
    private void EnterDash()
    {
        --currentBullets;
        if (movePressed) dashDir = moveDir;
        else dashDir = lastMoveDir;

        ResetCharge();
        rb.linearVelocity = Vector3.zero;
        isDashing = true;
        canMove = false;
    }

    private float currentDashTime = 0;
    private void Dash()
    {
        if (isDashing)
        {
            currentDashTime += Time.deltaTime;
            if(currentDashTime<= dashTime)
            {
                rb.linearVelocity = new Vector3(dashDir.x * dashSpeed, rb.linearVelocity.y, dashDir.y * dashSpeed);
            }
            else
            {
                canMove = true;
                currentDashTime = 0;
                isDashing = false;
            }
        }
    }

    private void EnterFly()
    {
        if (currentFuel > 0)
        {
            isFlying = true;
            currentMaxSpeed = flySpeed;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            ResetCharge();
        }
    }
    private void Fly()
    {
        if(isFlying && currentFuel > 0)
        {
            currentFuel -= fuelBurnSpeed * Time.deltaTime;
        }
        else
        {
            EndFly();
            if(currentFuel < maxFuel && isGrounded) currentFuel += fuelRecoverSpeed * Time.deltaTime;
        }
    }

    public void EndFly()
    {
        if (isFlying)
        {
            currentMaxSpeed = maxSpeed;
            isFlying = false;
        }
    }

    public void ForcedMovement(Vector3 targetPos)
    {
        canMove = false;
        this.transform.position = targetPos;
        canMove = true;
    }

    public void ResetPlayer()
    {
        rb.linearVelocity = Vector3.zero;
        ResetCharge();
        currentBullets = maxBullets;
        currentFuel = maxFuel;
        canMove = true;
        isReloading = false;
        UIManager.Instance.SetReloadQTEActive(false);
        reloadBarCurrentValue = 1;
    }

    private void HandleInput()
    {
        playerInput.PlayerControls.Move.started += ctx =>
        {
            MoveStarted();
        };
       //When a move input is used its value is read and stored as the move direction and as a bool
        playerInput.PlayerControls.Move.performed += ctx =>
        {
            moveDir = ctx.ReadValue<Vector2>();
            movePressed = moveDir.x != 0 || moveDir.y != 0;
        };
        //When the move input is canceled it resets the move direction to 0 and the moving bool to false
        playerInput.PlayerControls.Move.canceled += ctx =>
        {
            movePressed = false;
            lastMoveDir = moveDir;
            moveDir = Vector2.zero;
        };


        playerInput.PlayerControls.Aim.started += ctx =>
        {
            AimStarted();
            aimPressed = true;
        };

        playerInput.PlayerControls.Aim.performed += ctx =>
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 tempAimDir = ctx.ReadValue<Vector2>();
                tempAimDir.x -= Screen.width / 2;
                tempAimDir.y -= Screen.height / 2;
                aimDir = tempAimDir.normalized;
            }
            else
            {
                aimDir = ctx.ReadValue<Vector2>();
            }
            
        };

        playerInput.PlayerControls.Aim.canceled += ctx =>
        {
            shootDir = aimDir;
            aimPressed = false;
            aimDir = Vector2.zero;

            AimFinished();
        };


        playerInput.PlayerControls.Reload.started += ctx =>
        {
            ReloadStarted();
        };

        playerInput.PlayerControls.Reload.performed += ctx =>
        {

        };

        playerInput.PlayerControls.Reload.canceled += ctx =>
        {
            ReloadEnded();
        };

    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector3.zero;
        playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        rb.linearVelocity = Vector3.zero;
        playerInput.PlayerControls.Disable();
    }
}
