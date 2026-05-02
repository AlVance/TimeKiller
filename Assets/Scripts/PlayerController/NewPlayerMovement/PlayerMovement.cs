using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs pInputs;
    private PlayerFlyStamina pFlyStamina;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject playerParentModel;

    [Header("Movement variables")]
    [SerializeField] public float standardMoveSpeed;
    [SerializeField] private float startMoveSpeed;
    [SerializeField] private float flySpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideDownSpeed;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float walkAcceleration;
    [SerializeField] private float walkDeceleration;
    [SerializeField] private float flyAcceleration;
    [SerializeField] private float flyDeceleration;
    [SerializeField] private float slideAcceleration;
    [SerializeField] private float slideDeceleration;
    [SerializeField] private float slideDownAcceleration;
    [SerializeField] private float slideDownDeceleration;
    private float currentAcceleration;
    private float currentDeceleration;
    [SerializeField] private float slopeUpDecelerationMult;
    [SerializeField] private float slopeDownAccelerationMult;
    private float currentSlopeAccMult = 1;
    [SerializeField] private float groundedDamp;
    [SerializeField] private float slideDamp;
    [SerializeField] private float flyingDamp;
    [SerializeField] private float airDamp;
    [SerializeField] private float airMovementMultiplier;
    [SerializeField] private float speedDiffToLerp;
    public float desiredMoveSpeed;
    public float currentMoveSpeed;
    
    public Vector3 moveDirection;
    private Rigidbody rb;

    public bool movementBlocked = false;

    [Header("Fly variables")]
    private bool isFlying = false;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayDistance;
    private RaycastHit groundHit;
    public bool isGrounded;

    [Header("Slope variables")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    public bool isSliding = false;

    [Header("Float on ground")]
    [SerializeField] private float floatRayDistance;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStength;
    [SerializeField] private float rideSpringDamper;

    [Header("Gravity variables")]
    [SerializeField] public float gravityForce;
    public float currentGravityForce = 0;
    [SerializeField] private float maxFallSpeed;

    [SerializeField] private Transform orientation;

    [Header("Shoot variables")]
    public bool isAiming = false;

    [Header("WallRun variables")]
    public bool isWallRunning = false;

    [Header("Player Events")]
    public UnityEvent OnStartFlyEvent;

    public enum MovementStates { Idle, Walking, Air, Flying, Sliding, SlidingDown, WallRunning, Hitted}
    public MovementStates state;

    [SerializeField] public Collider playerCollider;

    [SerializeField] private TMP_Text speedText;
    public JumpPlatformController lastJumpPlatform;

    public float airSpeedLimit = 0f;  // Límite temporal de velocidad en el aire
    [SerializeField] private float airSpeedDecayRate = 5f;  // Velocidad de decaimiento (ajusta según necesites)
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pInputs = GetComponent<PlayerInputs>();
        pFlyStamina = GetComponent<PlayerFlyStamina>();

        rb.freezeRotation = true;

        currentGravityForce = gravityForce;

        desiredMoveSpeed = startMoveSpeed;
        state = MovementStates.Idle;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.5f, groundLayer);

        //if (!movementBlocked) SpeedControl();
        StateHandler();
        DampControl();
        SmoothSpeed();

        if(speedText != null)speedText.text = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude.ToString("00.0") + "\n" + state;

        //MoveRotationStuff
        if(!isAiming)
        {
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(moveDirection).eulerAngles.y, 0);
                playerModel.transform.localRotation = Quaternion.Euler(0, Quaternion.LookRotation(moveDirection).eulerAngles.y, 0);
            }

            Vector3 groundHitAngle = Quaternion.FromToRotation(Vector3.up, groundHit.normal).eulerAngles;
            if (moveDirection != Vector3.zero && isGrounded) playerParentModel.transform.rotation = Quaternion.Euler(new Vector3(groundHitAngle.x, 0, groundHitAngle.z));
            else if (!isGrounded) playerParentModel.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
    private void FixedUpdate()
    {
        if (!movementBlocked)
        {
            MovePlayer();
            
        }
        Fly();
        FloatOnGround();
        ApplyGravity();
        SpeedControl();
    }

    private void StateHandler()
    {
        if (isWallRunning)
        {
            state = MovementStates.WallRunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        else if (isFlying)
        {
            state = MovementStates.Flying;
            desiredMoveSpeed = flySpeed;
            currentAcceleration = flyAcceleration;
            currentDeceleration = flyDeceleration;
        }
        else if(isGrounded && rb.linearVelocity.magnitude > 0.1f)
        {
            if (isSliding) 
            {
                if(CheckOnSlope() && rb.linearVelocity.y < -0.1f)
                {
                    state = MovementStates.SlidingDown;
                    desiredMoveSpeed = slideDownSpeed;
                    currentAcceleration = slideDownAcceleration;
                    currentDeceleration = slideDownDeceleration;
                }
                else
                {
                    state = MovementStates.Sliding;
                    desiredMoveSpeed = slideSpeed;
                    currentAcceleration = slideAcceleration;
                    currentDeceleration = slideDeceleration;
                }   
            } 
            else
            {
                state = MovementStates.Walking;
                desiredMoveSpeed = standardMoveSpeed;
                currentAcceleration = walkAcceleration;
                currentDeceleration = walkDeceleration;
            }
        }
        else if(isGrounded && moveDirection == Vector3.zero)
        {
            state = MovementStates.Idle;
            currentMoveSpeed = startMoveSpeed;
            desiredMoveSpeed = startMoveSpeed;
            
        }
        else if (!isGrounded)
        {
            state = MovementStates.Air;
            desiredMoveSpeed = standardMoveSpeed;
        }
    }

    private void SmoothSpeed()
    {
        if (CheckOnSlope())
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            float slopeAngleIncrease = 1 + (slopeAngle / 90f);
            if (rb.linearVelocity.y > 0.1f)
            {
                currentSlopeAccMult = slopeUpDecelerationMult * slopeAngleIncrease;
            }
            else if(rb.linearVelocity.y < -0.1f)
            {
                currentSlopeAccMult = slopeDownAccelerationMult * slopeAngleIncrease;
            }
        }
        else
        {
            currentSlopeAccMult = 1f;
        }

        float diff = Mathf.Abs(desiredMoveSpeed - currentMoveSpeed);
        if(diff > speedDiffToLerp && currentMoveSpeed != 0)
        {
            if(currentMoveSpeed < desiredMoveSpeed)
            {
                currentMoveSpeed += currentAcceleration * currentSlopeAccMult * Time.deltaTime;
            }
            else
            {
                currentMoveSpeed -= currentDeceleration * currentSlopeAccMult * Time.deltaTime;
            }
        }
        else
        {
            currentMoveSpeed = desiredMoveSpeed;
        }
    }

    private void DampControl()
    {
        if (state == MovementStates.Flying)
        {
            rb.linearDamping = flyingDamp;
        }
        else if (state == MovementStates.Walking)
        {
            rb.linearDamping = groundedDamp;
        }
        else if (state == MovementStates.Sliding || state == MovementStates.SlidingDown)
        {
            rb.linearDamping = slideDamp;
        }
        else if (state == MovementStates.Air)
        {
            rb.linearDamping = airDamp;
        }
    }
    public float force = 10;
    private void MovePlayer()
    {

        moveDirection = pInputs.moveDirRelativeToCam;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        
        if (CheckOnSlope())
        {
            rb.AddForce(GetSlopeMoveDir(moveDirection) * currentMoveSpeed * 10, ForceMode.Force);
            if (moveDirection.sqrMagnitude > 0.1f) rb.AddForce(-slopeHit.normal * (force * rb.linearVelocity.sqrMagnitude), ForceMode.Force);
        }
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10, ForceMode.Force);
            rb.AddForce(-slopeHit.normal * (force * rb.linearVelocity.sqrMagnitude / 2), ForceMode.Force);
        } 
        else rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10 * airMovementMultiplier, ForceMode.Force);
    }

    

    private void Fly()
    {
        if(!isFlying && pInputs.flyPressed && pFlyStamina.currentFuel > pFlyStamina.minFuelToFly && !movementBlocked)
        {
            currentGravityForce = 0;
            if(rb.linearVelocity.y < 0) rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            isFlying = true;

            OnStartFlyEvent.Invoke();
        }
        else if(isFlying && (!pInputs.flyPressed || pFlyStamina.currentFuel <= 0 || movementBlocked))
        {
            currentGravityForce = gravityForce;
            isFlying = false;
        }

        if (isFlying)
        {
            if (rb.linearVelocity.y < 0) rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }
    public float extraForce = 0;
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (CheckOnSlope())
        {
            if (rb.linearVelocity.magnitude > currentMoveSpeed + extraForce)
                rb.linearVelocity = rb.linearVelocity.normalized * currentMoveSpeed;
        }
        else
        {
            //float maxSpeed = (state == MovementStates.Air) ? Mathf.Max(currentMoveSpeed, airSpeedLimit) : currentMoveSpeed + extraForce;
            //if (flatVel.magnitude > maxSpeed)
            //{
            //    Vector3 limitedVel = flatVel.normalized * maxSpeed;
            //    rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            //}
            float maxSpeed = Mathf.Max(currentMoveSpeed + extraForce, airSpeedLimit);
            if (flatVel.magnitude > maxSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        if (airSpeedLimit > currentMoveSpeed)
        {
            float groundedDecrease = 1;
            if (isGrounded) groundedDecrease = 2f;
            airSpeedLimit -= airSpeedDecayRate * groundedDecrease * Time.deltaTime;

            if(state == MovementStates.Flying) airSpeedLimit = currentMoveSpeed;
        }
        else
        {
            airSpeedLimit = currentMoveSpeed;
        }

        if (flatVel.magnitude <= desiredMoveSpeed) extraForce = 0;
    }

    public bool CheckOnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundRayDistance) && !slopeHit.collider.isTrigger)
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDir(Vector3 dir)
    {
        return Vector3.ProjectOnPlane(dir, slopeHit.normal).normalized;
    }

    private void ApplyGravity()
    {

        if (rb.linearVelocity.y < maxFallSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxFallSpeed, rb.linearVelocity.z);
        }
        else
        {
            rb.AddForce(Vector3.down * currentGravityForce, ForceMode.Force);
        }
    }

    private void FloatOnGround()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out groundHit, floatRayDistance))
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

    public void ApplyExternalImpulse(Vector3 direction, float force)
    {
        Vector3 combDir = direction * force;
        Vector3 combDirFlat = new Vector3(combDir.x, 0, combDir.z);

        extraForce = combDirFlat.magnitude;

        Vector3 currentFlatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        airSpeedLimit = currentFlatVel.magnitude + combDirFlat.magnitude;
        rb.AddForce(combDir, ForceMode.Impulse);
    }

    private void OnDisable()
    {
        state = MovementStates.Hitted;
        desiredMoveSpeed = standardMoveSpeed;
        currentMoveSpeed = standardMoveSpeed;
        extraForce = 0;
        rb.linearVelocity = Vector3.zero;
    }
}
