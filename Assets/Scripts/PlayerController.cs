using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header ("Gravity Varaibles")]
    [SerializeField] private float gravityForce = -9.81f;

    private PlayerInput playerInput;

    [Header("Move Varaibles")]
    private bool movePressed;
    [SerializeField] private float moveSpeed;
    private float currentMoveSpeed;
    [SerializeField]private float moveSpeedIncreaser;
    private Vector2 moveDir;

    [Header("Aim Varaibles")]
    private bool aimPressed;
    private Vector2 aimDir;
    private Vector2 shootDir;


    [Header("Shoot Variables")]
    private float currentChargeTime;
    [SerializeField] private float shootChargeTime;
    [SerializeField] private GameObject projectileGO;
    [SerializeField] private Transform porjectileSpawnPos;
    [SerializeField] private float projectileSize;
    [SerializeField] private float projectileSpeed;
    private GameObject currentProjectileGO;
    [SerializeField] private float projectileRange; //life time
    [SerializeField] private int projectileDamage; //life time

    private void Awake()
    {
        playerInput = new PlayerInput();
        HandleInput();

        rb = this.GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            if(currentChargeTime <= 0)
            {
                currentProjectileGO = Instantiate(projectileGO, porjectileSpawnPos.position, Quaternion.identity , porjectileSpawnPos);
                currentProjectileGO.GetComponent<PlayerProjectile>().ProjectileSetUp(this, projectileDamage, projectileRange);
            }
            if(currentChargeTime <= shootChargeTime)
            {
                currentChargeTime += Time.deltaTime;
                currentProjectileGO.transform.localScale = Vector3.Lerp(currentProjectileGO.transform.localScale, new Vector3(projectileSize, projectileSize, projectileSize), (currentChargeTime / shootChargeTime) * Time.deltaTime);
            }
            else
            {
                currentProjectileGO.GetComponent<MeshRenderer>().material.color = Color.green;
                currentProjectileGO.GetComponent<PlayerProjectile>().charged = true;
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
        }
        else
        {
            ResetCharge();
        }
    }

    public void ResetCharge()
    {
        currentProjectileGO.transform.parent = null;
        Destroy(currentProjectileGO);
        currentProjectileGO = null;
        currentChargeTime = 0;
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
