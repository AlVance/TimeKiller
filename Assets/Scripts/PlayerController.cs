using UnityEngine;
using TMPro;
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;

    [Header ("Gravity Varaibles")]
    [SerializeField] private float gravityForce = -9.81f;

    [Header("Move Varaibles")]
    private bool movePressed;
    [SerializeField] public float moveSpeed;
    private float currentMoveSpeed;
    [SerializeField]private float moveSpeedIncreaser;
    private Vector2 moveDir;

    [Header("Aim Varaibles")]
    private bool aimPressed;
    private Vector2 aimDir;
    private Vector2 shootDir;


    [Header("Shoot Variables")]
    private float currentChargeTime;
    [SerializeField] public float shootChargeTime;
    [SerializeField] private GameObject projectileGO;
    [SerializeField] private Transform porjectileSpawnPos;
    [SerializeField] public float projectileSize;
    [SerializeField] public float projectileSpeed;
    private GameObject currentProjectileGO;
    [SerializeField] public float projectileRange; //life time
    [SerializeField] public int projectileDamage; //life time

    [Header("Ammo Variables")]
    public int currentBullets;
    [SerializeField] public int maxBullets;
    [SerializeField] private TMP_Text bulletsText;

    private void Awake()
    {
        playerInput = new PlayerInput();
        HandleInput();

        rb = this.GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentBullets = maxBullets;
    }

    // Update is called once per frame
    void Update()
    {
        AddGravityForce();
        Movement();
        Aim();
        ChargeShot();
    }

    private void MoveStarted()
    {

    }

    private void AimStarted()
    {

    }

    private void AimFinished()
    {
        Shoot();
    }

    private void ReloadStarted()
    {
        ReloadBullets();
    }

    private void AddGravityForce()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, gravityForce, rb.linearVelocity.z);
    }

    private void Movement()
    {
        currentMoveSpeed = moveSpeed;
        rb.linearVelocity = new Vector3(moveDir.x * currentMoveSpeed, rb.linearVelocity.y, moveDir.y * currentMoveSpeed);
        if (movePressed && !aimPressed) this.transform.rotation = Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.y));
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
            currentProjectileGO.GetComponent<PlayerProjectile>().LaunchProjectile(new Vector3(shootDir.x, 0, shootDir.y), projectileSpeed);
            currentProjectileGO = null;
            currentChargeTime = 0;
            --currentBullets;
            bulletsText.text = currentBullets + "/" + maxBullets;
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
        if(currentBullets < maxBullets)
        {
            currentBullets = maxBullets;
            bulletsText.text = currentBullets + "/" + maxBullets;
        }
    }

    public void SetPlayerDamage(int damageMod)
    {
        projectileDamage += damageMod;
    }
    public void SetPlayerSpeed(float speedMod)
    {
        moveSpeed += speedMod;
    }
    public void SetPlayerRange(float rangeMod)
    {
        projectileRange += rangeMod;
    }
    public void SetPlayerProjectileSize(float projectileSizeMod)
    {
        projectileSize += projectileSizeMod;
    }
    public void SetPlayerProjectileSpeed(float projectileSpeedMod)
    {
        projectileSpeed += projectileSpeedMod;
    }
    public void SetPlayerChargeTime(float chargeTimeMod)
    {
        if(shootChargeTime > 0.1f) shootChargeTime += chargeTimeMod;
        if (shootChargeTime > 0.1f) shootChargeTime = 0.1f;
    }
    public void SetPlayerMaxBullets(int bulletsMod)
    {
        maxBullets += bulletsMod;
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
            moveDir = Vector2.zero;
        };


        playerInput.PlayerControls.Aim.started += ctx =>
        {
            AimStarted();
            aimPressed = true;
        };

        playerInput.PlayerControls.Aim.performed += ctx =>
        {
            aimDir = ctx.ReadValue<Vector2>();
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
        playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerControls.Disable();
    }
}
