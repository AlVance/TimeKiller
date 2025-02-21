using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;

    [Header("Camera Variables")]
    [SerializeField] private CinemachineCamera playerCCam;
    [SerializeField] private CinemachineCamera aimCCam;
    [SerializeField] private Transform aimTargetTr;


    [Header ("Gravity Varaibles")]
    [SerializeField] private float gravityForce = -9.81f;
    [SerializeField] private float maxFallSpeed;


    [Header("Move Varaibles")]
    [SerializeField] private float m_moveSpeed;
    public float moveSpeed
    {
        get { return m_moveSpeed; }
        set
        {
            m_moveSpeed = value;
            UIManager.Instance.SetCurrentSpeedText(m_moveSpeed.ToString());
        }
    }
    private bool movePressed;
    private bool canMove = true;
    private float currentMoveSpeed;
    private Vector2 moveDir;
    private Vector2 lastMoveDir;


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
        }
    }
    private int m_currentBullets;
    public int currentBullets
    {
        get { return m_currentBullets; }
        set 
        {
            m_currentBullets = value;
            UIManager.Instance.SetBulletsText(m_currentBullets.ToString() + "/" + m_maxBullets.ToString());
        }
    }


    [Header("Ammo Variables")]
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



    private void Awake()
    {
        playerInput = new PlayerInput();
        HandleInput();

        rb = this.GetComponent<Rigidbody>();
        if(GameManager.Instance != null) GameManager.Instance.currentPlayer = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera.main.gameObject.GetComponent<FollowObject>().targetTr = this.gameObject.transform;
        currentBullets = maxBullets;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Aim();
        ChargeShot();
        Dash();
    }
    private void FixedUpdate()
    {
        if(!isDashing)AddGravityForce();
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
        if(currentBullets <= 0)
        {
            ReloadBullets();
        }
        else
        {
            EnterDash();
        }
    }

    private void AddGravityForce()
    {
        if(rb.linearVelocity.y > maxFallSpeed) rb.linearVelocity += new Vector3(0, gravityForce, 0);
        else rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxFallSpeed, rb.linearVelocity.z);

    }

    private void Movement()
    {
        if (canMove)
        {
            currentMoveSpeed = moveSpeed;
            rb.linearVelocity = new Vector3(moveDir.x * currentMoveSpeed, rb.linearVelocity.y, moveDir.y * currentMoveSpeed);
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
        if (aimPressed)
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
                    currentProjectileGO.GetComponent<MeshRenderer>().material.color = Color.green;
                    currentProjectileGO.GetComponent<PlayerProjectile>().charged = true;
                }
            }
        }
    }

    private void Shoot()
    {
        if(currentChargeTime >= shootChargeTime)
        {
            Debug.Log("ShootDir = " + shootDir);
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

    private void ReloadBullets()
    {
        currentBullets = maxBullets;
    }

    Vector2 dashDir;
    private void EnterDash()
    {
        --currentBullets;
        if (movePressed) dashDir = moveDir;
        else dashDir = lastMoveDir;

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
                rb.linearVelocity = new Vector3(dashDir.x * dashSpeed, 0, dashDir.y * dashSpeed);
            }
            else
            {
                canMove = true;
                currentDashTime = 0;
                isDashing = false;
            }
        }
    }

    public void ForcedMovement(Vector3 targetPos)
    {
        canMove = false;
        this.transform.position = targetPos;
        canMove = true;
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
            Debug.Log("Reload started");
            ReloadStarted();
        };

        playerInput.PlayerControls.Reload.performed += ctx =>
        {

        };

        playerInput.PlayerControls.Reload.canceled += ctx =>
        {

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
