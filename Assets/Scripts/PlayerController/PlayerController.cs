using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;

    [SerializeField] private Collider playerPhisicalCollider;

    [Header("Camera Variables")]
    [SerializeField] private Transform aimTargetTr;
    [SerializeField] private Transform flyTargetTr;


    [Header ("Gravity Varaibles")]
    [SerializeField] private float groundRayDistance;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStength;
    [SerializeField] private float rideSpringDamper;
    private RaycastHit groundHit;
    private float currentGravityForce = 0;
    [SerializeField] private float gravityForce;
    [SerializeField] private float dashGravityForce;
    [SerializeField] private float maxFallSpeed;
    public bool affectedByGravity = true;


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
    [SerializeField] private float currentAccelerationSpeed;
    [SerializeField] private float maxAccelerationForce;
    private bool movePressed;
    private bool canMove = true;

    private Vector2 moveDir;
    private Vector3 moveDirRelativeToCam;
    private Vector2 lastMoveDir;
    private float currentMaxSpeed;

    [Header("Aim Varaibles")]
    [SerializeField] private GameObject aimDirAidGO;
    private bool aimPressed;
    private Vector2 aimDir;
    private Vector3 aimDirRelativeToCam;
    private Vector2 shootDir;
    private Vector3 shootDirRelativeToCam;
    private bool isAiming = false;
    private bool canAim = true;


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
    [SerializeField] private float onShootTpMoveSpeed;


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

    [Header("Drift Variables")]
    [SerializeField] private float driftSpeed;
    float currentDriftCharge = 0;
    private float currentDriftSpeed;
    [SerializeField] private float driftBoostChargeSpeed = 2;
    [SerializeField] private float boostChargeSpeed = 2;
    [SerializeField] private float driftConsumeSpeed = 2;
    private float maxDriftChargeTime = 1f;
    bool isChargingDrift = false;
    Vector3 targetDriftChargeVel;
    Vector3 currentDriftChargeVel;
    [SerializeField] private float driftRotationForce;
    [SerializeField] private GameObject driftPS;
    private bool driftPressed = false;
    private bool canDrift = false;
    [SerializeField] private float stearingFactor;
    [SerializeField] private Vector2 minMaxDriftSpeed;
    [SerializeField] private AnimationCurve speedModOverStearing;


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
    public float currentFuel
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
    public bool isFlying = false;
    private bool canFly = true;

    [Header("Hit variables")]
    [SerializeField] private float stunnedTime;
    [SerializeField] private float hitForce;
    private bool canGetHitted = true;
    private bool isHitted = false;

    [Header("Ledge grab variables")]
    [SerializeField] private Transform upRayTr;
    [SerializeField] private Transform[] frontRayTr;
    [SerializeField] private float upRayDistance;
    [SerializeField] private float frontRayDistance;

    [Header("Animation variables")]
    [SerializeField] public Animator anim;
    [SerializeField] private GameObject backGunGO;

    [Header("Player Events")]
    public UnityEvent OnStartFlyEvent;

    [Header("Sound Variables")]
    [SerializeField] private AudioSource playerAS;
    [SerializeField] private AudioSource flyAS;
    [SerializeField] private AudioClip playerGetHitAC, playerStartFlyAC, playerFlyAC;
    [SerializeField] private AudioClip deathSound;
    private float initialPitchAS;

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player onHitFeedback;
    private Tween aimRotationTween;
    private Tween moveRotationTween;

    private void Awake()
    {
        playerInput = new PlayerInput();
        HandleInput();
        rb = this.GetComponent<Rigidbody>();
        DOTween.Init();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.currentPlayer = this;
        maxBullets = m_maxBullets;
        currentBullets = maxBullets;
        maxFuel = m_maxFuel;
        currentFuel = maxFuel;
        currentMaxSpeed = maxSpeed;
        currentGravityForce = gravityForce;
        currentAccelerationSpeed = accelerationSpeed;
        ProjectilePooling();

        initialPitchAS = playerAS.pitch;
    }

    private List<GameObject> projectilePool = new List<GameObject>();
    private int currentProjectilePooled = 0;
    private void ProjectilePooling()
    { 
        for (int i = 0; i < (projectileRange/shootChargeTime) + 1; i++)
        {
            GameObject newProj = Instantiate(projectileGO, porjectileSpawnPos.position, Quaternion.identity, porjectileSpawnPos);
            projectilePool.Add(newProj);
            newProj.GetComponent<Projectile>().spawnPos = porjectileSpawnPos;
            newProj.GetComponent<Projectile>().SetProjectileInactive();           
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playerWork)
        {
            Aim();
            ChargeShot();
            Fly();
        }
        HandleAnimations();
    }
    private void FixedUpdate()
    {
        GroundCheck();
        FloatOnGround();
        if (GameManager.Instance.playerWork)
        {
            if (!isChargingDrift) Movement();
            Drift();

            if (affectedByGravity) AddGravityForce();
            CheckLedgeGrab();
        }
        
    }


    private void MoveStarted()
    {

    }

    private void AimStarted()
    {
        if(canAim && currentBullets > 0)
        {
            if (moveRotationTween.IsActive() && moveRotationTween.IsPlaying()) moveRotationTween.Kill();
            if (aimDir != Vector2.zero) aimRotationTween = transform.DORotate(Quaternion.LookRotation(aimDirRelativeToCam).eulerAngles, 0f, RotateMode.Fast);
            if (CameraManager.Instance.currentCam.GetComponent<FollowObject>().followPlayer) CameraManager.Instance.currentCam.GetComponent<FollowObject>().targetTr = aimTargetTr;
            aimDirAidGO.SetActive(true);
            ExitDrift();

            isAiming = true;
            aimPressed = true;
        }
        
    }
    private void Aim()
    {
        if (canAim && aimPressed)
        {
            if (!isAiming)
            {
                AimStarted();
            }

            if (aimDir != Vector2.zero)
            {
                aimRotationTween = transform.DORotate(Quaternion.LookRotation(aimDirRelativeToCam).eulerAngles, 0.25f, RotateMode.Fast);
            }

            isAiming = true;
        }
        else isAiming = false;
    }

    private void AimFinished()
    {
        Shoot();
        EndAim();
    }

    private void EndAim()
    {
        aimDirAidGO.SetActive(false);
        if (CameraManager.Instance.currentCam.GetComponent<FollowObject>().followPlayer) CameraManager.Instance.currentCam.GetComponent<FollowObject>().targetTr = this.gameObject.transform;
        isAiming = false;
    }

    private void ReloadStarted()
    {
        EnterFly();
    }
    private void ReloadPerformed()
    {
        if (!isFlying) EnterFly();
    }
    private void ReloadEnded()
    {
        EndFly();
    }

    private void DriftStarted()
    {
        EnterDrift();
    }
    private void DriftPerformed()
    {
        
    }
    private void DriftEnded()
    {
        ExitDrift();
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
        if (isFlying && rb.linearVelocity.y <= 0)
        {
            currentGravityForce = 0;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
        else if (isFlying && rb.linearVelocity.y > 0)
        {
            currentGravityForce = dashGravityForce;
        }
        else
        {
            currentGravityForce = gravityForce;
        }
        if (rb.linearVelocity.y > maxFallSpeed) rb.linearVelocity += new Vector3(0, currentGravityForce, 0);
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
            Vector3 otherVel = Vector3.zero;
            if (Physics.Raycast(this.transform.position, Vector3.down, out groundHit, groundRayDistance))
            {
                if (groundHit.rigidbody != null)
                {
                    otherVel = new Vector3(groundHit.rigidbody.linearVelocity.x, 0, groundHit.rigidbody.linearVelocity.z);
                }
                else otherVel = Vector3.zero;
            }
            else otherVel = Vector3.zero;

            Vector3 goalVel = moveDirRelativeToCam * currentMaxSpeed + otherVel;

            m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, currentAccelerationSpeed * Time.fixedDeltaTime);

            Vector3 neededAccel = (m_GoalVel - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z)) / Time.fixedDeltaTime;

            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccelerationForce);
            rb.AddForce(neededAccel * rb.mass);
            if (!isAiming && moveDirRelativeToCam != Vector3.zero)
            {
                if (aimRotationTween.IsActive()) aimRotationTween.Kill();
                moveRotationTween = transform.DORotate(Quaternion.LookRotation(moveDirRelativeToCam).eulerAngles, 0.2f, RotateMode.Fast);
            }
                
        }
    }

    

    private void ChargeShot()
    {
        if (canAim && aimPressed && !shootCD)
        {
            if(currentBullets > 0)
            {
                if (currentChargeTime <= 0)
                {
                    currentProjectileGO = projectilePool[currentProjectilePooled];
                    if (currentProjectilePooled < projectilePool.Count - 1) ++currentProjectilePooled;
                    else currentProjectilePooled = 0;
                    currentProjectileGO.GetComponent<PlayerProjectile>().ProjectileSetUp(projectileDamage, projectileRange, porjectileSpawnPos);
                }
                if (currentChargeTime <= shootChargeTime)
                {
                    currentChargeTime += Time.deltaTime;
                    currentProjectileGO.transform.localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 0.01f), new Vector3(projectileSize, projectileSize, projectileSize), (currentChargeTime / shootChargeTime));
                }
                else
                {
                    if(!currentProjectileGO.GetComponent<PlayerProjectile>().charged) currentProjectileGO.GetComponent<PlayerProjectile>().SetCharged();
                }
            }
        }
    }

    private void Shoot()
    {
        if(currentChargeTime >= shootChargeTime)
        {
            currentProjectileGO.transform.parent = null;
            currentProjectileGO.GetComponent<PlayerProjectile>().LaunchProjectile(shootDirRelativeToCam + moveDirRelativeToCam * moveDirShootInertia, projectileSpeed);
            currentProjectileGO = null;
            currentChargeTime = 0;
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
            currentProjectileGO.GetComponent<Projectile>().SetProjectileInactive();
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


    
   

    ////////////////////////////////////////////////
    private void EnterDrift()
    {
        if (isGrounded && !isFlying && canDrift)
        {
            targetDriftChargeVel = transform.forward;
            currentDriftChargeVel = targetDriftChargeVel;
            isChargingDrift = true;
            currentDriftSpeed = driftSpeed;

            driftPS.SetActive(true);
        }
    }
    private void Drift()
    {
        if (isGrounded)
        {
            if (isChargingDrift)
            {
                targetDriftChargeVel = transform.forward;
                if (currentDriftChargeVel != targetDriftChargeVel)
                {
                    currentDriftChargeVel = Vector3.Lerp(currentDriftChargeVel, targetDriftChargeVel, driftRotationForce).normalized;
                }
                Debug.Log(speedModOverStearing.Evaluate(Vector3.Distance(transform.forward, rb.linearVelocity.normalized)));

                currentDriftSpeed += speedModOverStearing.Evaluate(Vector3.Distance(transform.forward, rb.linearVelocity.normalized)) * Time.deltaTime;
                if (currentDriftSpeed > minMaxDriftSpeed.y) currentDriftSpeed = minMaxDriftSpeed.y;
                if (currentDriftSpeed < minMaxDriftSpeed.x) currentDriftSpeed = minMaxDriftSpeed.x;


                if (Vector3.Distance(transform.forward, rb.linearVelocity.normalized) > stearingFactor)
                {
                    if (currentFuel < maxDriftChargeTime) currentFuel += driftBoostChargeSpeed * Time.deltaTime;
                    else currentFuel = maxDriftChargeTime;

                    var main = driftPS.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.red;
                }
                else
                {
                    var main = driftPS.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.blue;
                }

                if (moveDir != Vector2.zero)
                {
                    transform.DORotate(Quaternion.LookRotation(moveDirRelativeToCam).eulerAngles, 0.25f, RotateMode.Fast);
                }

                rb.linearVelocity = new Vector3(currentDriftChargeVel.x * currentDriftSpeed, rb.linearVelocity.y, currentDriftChargeVel.z * currentDriftSpeed);
            }
            else if(driftPressed)
            {
                EnterDrift();
            }
        }       
        else if (isChargingDrift)
        {
            ExitDrift();
        }

        if (!isFlying)
        {
            if (currentFuel < maxDriftChargeTime) currentFuel += boostChargeSpeed * Time.deltaTime;
            else currentFuel = maxDriftChargeTime;
        }       
    }
    private void ExitDrift()
    {
        StartCoroutine(_ExitDrift());
        
    }
    private IEnumerator _ExitDrift()
    {
        currentAccelerationSpeed = maxAccelerationForce;
        isChargingDrift = false;
        driftPS.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        currentAccelerationSpeed = accelerationSpeed;
    }
    ///////////////////////////////////////////////////////
    private void EnterFly()
    {
        if (currentFuel > 0 && canFly)
        {
            ExitDrift();
            isFlying = true;
            currentMaxSpeed = flySpeed;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            OnStartFlyEvent.Invoke();

            playerAS.PlayOneShot(playerStartFlyAC);
            StartCoroutine(PlayFlySound());
        }
    }

    private void Fly()
    {
        if(isFlying && currentFuel > 0 && canFly)
        {
            if(m_GoalVel.magnitude > 0) currentFuel -= fuelBurnSpeed * Time.deltaTime;
            else currentFuel -= fuelBurnSpeed / 4 * Time.deltaTime;
        }
        else
        {
            EndFly();
        }
    }

    public void EndFly()
    {
        if (isFlying)
        {
            currentMaxSpeed = maxSpeed;
            isFlying = false;

            flyAS.clip = null;
            flyAS.Stop();
        }
    }

    private IEnumerator PlayFlySound()
    {
        yield return new WaitForSeconds(0.1f);
        if (isFlying)
        {
            flyAS.clip = playerFlyAC;
            flyAS.Play();
        }
       
    }
    private bool UpRayHitted;
    private bool FrontRayHitted;
    private void CheckLedgeGrab()
    {
        RaycastHit upRayHit;
        RaycastHit frontRayHit;

        Debug.DrawRay(upRayTr.position, Vector3.down * upRayDistance, Color.green);
        for (int i = 0; i < frontRayTr.Length; i++)
        {
            Debug.DrawRay(frontRayTr[i].position, this.transform.forward * frontRayDistance, Color.green);
        }
        if (Physics.Raycast(upRayTr.position, Vector3.down, out upRayHit, upRayDistance, groundCheckLayersToCheck))
        {
            UpRayHitted = true;
        }
        else UpRayHitted = false;
        for (int i = 0; i < frontRayTr.Length; i++)
        {
            if (Physics.Raycast(frontRayTr[i].position, this.transform.forward, out frontRayHit, frontRayDistance, groundCheckLayersToCheck))
            {
                FrontRayHitted = true;
                break;
            }
            else FrontRayHitted = false;
        }

        if (UpRayHitted && FrontRayHitted && !isGrounded && m_GoalVel.magnitude > 0)
        {
            LedgeGrab(upRayHit.point);
        }
    }

    private void LedgeGrab(Vector3 newPos)
    {
        Debug.Log("LedgeGrab");
        this.transform.position = newPos + new Vector3(0,1,0);
    }

    public void ForcedMovement(Vector3 targetPos)
    {
        StartCoroutine(_ForcedMovement(targetPos));
    }
    private IEnumerator _ForcedMovement(Vector3 _targetPos)
    {
        BlockPlayer();
        currentMaxSpeed = 0;
        playerPhisicalCollider.enabled = false;
        while (Vector3.Distance(this.transform.position, _targetPos) > 1f)
        {
            rb.linearVelocity = (_targetPos - this.transform.position).normalized * onShootTpMoveSpeed;
            yield return null;
        }
        rb.linearVelocity = Vector3.zero;
        playerPhisicalCollider.enabled = true;
        UnblockPlayer();
    }

    public void ResetPlayer()
    {
        canMove = false;
        m_GoalVel = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        moveDir = Vector2.zero;
        ResetCharge();
        currentFuel = maxFuel;
        UIManager.Instance.SetFlyFuelSliderColor(Color.white);
        canMove = true;
    }

    public void GetHit(Vector3 hitPos, float hitForce)
    {
        if(GameManager.Instance.playerWork && canGetHitted) StartCoroutine(_GetHit(hitPos, hitForce));
    }
    private IEnumerator _GetHit(Vector3 hitPos, float hitForce)
    {
        onHitFeedback.PlayFeedbacks();
        playerAS.pitch = initialPitchAS + Random.Range(-0.1f, 0.1f);
        playerAS.PlayOneShot(playerGetHitAC);

        canGetHitted = false;
        isHitted = true;
        m_GoalVel = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        ResetCharge();

        rb.AddForce((this.transform.position - hitPos) * hitForce);
        BlockPlayer();
        yield return new WaitForSeconds(stunnedTime);

        isHitted = false;
        UnblockPlayer();
        yield return new WaitForSeconds(0.6f);
        canGetHitted = true;
    }
    private bool isBlocked = false;
    public JumpPlatformController lastPlatformTouched;
    public void BlockPlayer(bool blockAim = true)
    {
        isBlocked = true;
        canFly = false;
        EndFly();
        ExitDrift();
        isFlying = false;
        canMove = false;
        canDrift = false;
        if (blockAim)
        {
            canAim = false;
            EndAim();
        }
    }
    public void UnblockPlayer()
    {
        canFly = true;
        canMove = true;
        canAim = true;
        canDrift = true;
        isBlocked = false;
    }

    private bool isOffLimits = false;
    public void PlayerOffLimits(Transform tpPos)
    {
        if(!isOffLimits) StartCoroutine(_PlayerOfLimits(tpPos));
    }

    private IEnumerator _PlayerOfLimits(Transform tpPos)
    {
        isOffLimits = true;
        canGetHitted = false;
        TimeManager.Instance.timerStarted = false;
        playerAS.PlayOneShot(playerGetHitAC);
        SoundManager.Instance.PlayOneShootAudio(deathSound);
        m_GoalVel = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        canFly = false;
        canMove = false;
        currentMaxSpeed = 0;
        affectedByGravity = false;
        canAim = false;
        ResetCharge();
        EndAim();
        this.GetComponentInChildren<PlayerVFX>().DissolvePlayer(0);
       
        yield return new WaitForSeconds(1f);
        currentFuel = maxFuel;
        this.transform.position = tpPos.position;
        
        yield return new WaitForSeconds(0.5f);
        this.GetComponentInChildren<PlayerVFX>().DissolvePlayer(1);

        yield return new WaitForSeconds(1f);

        canFly = true;
        currentMaxSpeed = maxSpeed;
        canMove = true;
        affectedByGravity = true;
        canAim = true;
        this.GetComponentInChildren<PlayerVFX>().ChangeMaterialProperties(2, 0, 1);
        TimeManager.Instance.timerStarted = true;
        isOffLimits = false;
        yield return new WaitForSeconds(1f);
        canGetHitted = true;
    }
    private Vector3 GetV3RelativeToCamera(Vector2 baseDir)
    {
        Vector3 camForward = CameraManager.Instance.currentCam.transform.forward;
        Vector3 camRight = CameraManager.Instance.currentCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 forwardRelativeVertical = baseDir.y * camForward;
        Vector3 rightRelativeVertical = baseDir.x * camRight;

        Vector3 cameraRelativeDir = (forwardRelativeVertical + rightRelativeVertical).normalized;
        return cameraRelativeDir;
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

            moveDirRelativeToCam = GetV3RelativeToCamera(moveDir);
        };
        //When the move input is canceled it resets the move direction to 0 and the moving bool to false
        playerInput.PlayerControls.Move.canceled += ctx =>
        {
            movePressed = false;
            lastMoveDir = moveDir;
            moveDir = Vector2.zero;
            moveDirRelativeToCam = GetV3RelativeToCamera(moveDir);
        };


        playerInput.PlayerControls.Aim.started += ctx =>
        {
            if (GameManager.Instance.playerWork && Mouse.current.leftButton.isPressed)
            {
                Vector2 tempAimDir = ctx.ReadValue<Vector2>();
                Vector3 PlayerScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
                tempAimDir.x -= PlayerScreenPos.x;
                tempAimDir.y -= PlayerScreenPos.y;
                aimDir = tempAimDir.normalized;
                aimDirRelativeToCam = GetV3RelativeToCamera(aimDir);

                AimStarted();
            }
                
        };

        playerInput.PlayerControls.Aim.performed += ctx =>
        {

            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 tempAimDir = ctx.ReadValue<Vector2>();
                Vector3 PlayerScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
                tempAimDir.x -= PlayerScreenPos.x;
                tempAimDir.y -= PlayerScreenPos.y;
                aimDir = tempAimDir.normalized;
            }
            else 
            {
                Vector2 tempAimDir = ctx.ReadValue<Vector2>();
                if (tempAimDir.x > 0.1f || tempAimDir.x < -0.1f || tempAimDir.y > 0.1f || tempAimDir.y < -0.1f) 
                {
                    aimDir = tempAimDir;
                    if (GameManager.Instance.playerWork) AimStarted();
                } 
                
            }

            aimDirRelativeToCam = GetV3RelativeToCamera(aimDir);
        };

        playerInput.PlayerControls.Aim.canceled += ctx =>
        {
            shootDir = aimDir;
            shootDirRelativeToCam = GetV3RelativeToCamera(shootDir);
            aimPressed = false;
            aimDir = Vector2.zero;
            aimDirRelativeToCam = GetV3RelativeToCamera(aimDir);
            AimFinished();
        };


        playerInput.PlayerControls.Reload.started += ctx =>
        {
            if (GameManager.Instance.playerWork) ReloadStarted();
        };

        playerInput.PlayerControls.Reload.performed += ctx =>
        {
            if (GameManager.Instance.playerWork) ReloadPerformed();
        };

        playerInput.PlayerControls.Reload.canceled += ctx =>
        {
            if (GameManager.Instance.playerWork) ReloadEnded();
        };

        playerInput.PlayerControls.Shoot.started += ctx =>
        {
            if (currentChargeTime >= shootChargeTime)
            {
                shootDir = aimDir;
                shootDirRelativeToCam = GetV3RelativeToCamera(shootDir);
                AimFinished();
            } 
        };

        playerInput.PlayerControls.Drift.started += ctx =>
        {
            driftPressed = true;
            if (GameManager.Instance.playerWork) DriftStarted();
        };

        playerInput.PlayerControls.Drift.performed += ctx =>
        {
            if (GameManager.Instance.playerWork) DriftPerformed();
        };

        playerInput.PlayerControls.Drift.canceled += ctx =>
        {
            driftPressed = false;
            if (GameManager.Instance.playerWork) DriftEnded();
        };
    }
    private void HandleAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", isFlying);
        anim.SetBool("isChargingDrift", isChargingDrift);
        anim.SetBool("isMoving", canMove && m_GoalVel.magnitude > 0 && moveDir != Vector2.zero);
        if (!isAiming)
        {
            anim.SetLayerWeight(1, 0f);
            backGunGO.SetActive(true);
        }
        else 
        {
            anim.SetLayerWeight(1, 100f);
            backGunGO.SetActive(false);
        }
        anim.SetBool("IsHit", isHitted);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyHitBox")
        {
            GetHit(other.gameObject.transform.position, hitForce);
        }
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
