using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs pInputs;

    [SerializeField] private GameObject playerModel;

    [Header("Movement variables")]
    [SerializeField] private float standardMoveSpeed;
    [SerializeField] private float startMoveSpeed;
    [SerializeField] private float flySpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float groundedDamp;
    [SerializeField] private float slideDamp;
    [SerializeField] private float flyingDamp;
    [SerializeField] private float airMovementMultiplier;
    [SerializeField] private float speedDiffToLerp;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private float m_currentmoveSpeed;
    public float currentMoveSpeed
    {
        get { return m_currentmoveSpeed; }
        set
        {
            m_currentmoveSpeed = value;
            
        }
    }
    public bool isSliding = false;
    private Vector3 moveDirection;
    private Rigidbody rb;
    [SerializeField] private float speedIncreaseMultiplier;
    [SerializeField] private float slopeIncreaseMultiplier;

    [Header("Fly variables")]
    private bool isFlying = false;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayDistance;
    private RaycastHit groundHit;
    private bool isGrounded;

    [Header("Slope variables")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Float on ground")]
    [SerializeField] private float floatRayDistance;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStength;
    [SerializeField] private float rideSpringDamper;

    [Header("Gravity variables")]
    [SerializeField] private float gravityForce;
    private float currentGravityForce = 0;
    [SerializeField] private float maxFallSpeed;

    [SerializeField] private Transform orientation;


    public enum MovementStates { Idle, Walking, Air, Flying, Sliding}
    public MovementStates state;

    [SerializeField] private TMP_Text speedText;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pInputs = GetComponent<PlayerInputs>();

        rb.freezeRotation = true;

        currentGravityForce = gravityForce;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);

        SpeedControl();
        StateHandler();

        if (state == MovementStates.Flying)
        {
            rb.linearDamping = flyingDamp;
        }
        else if (state == MovementStates.Walking)
        {
            rb.linearDamping = groundedDamp;
        }
        else if(state == MovementStates.Sliding)
        {
            rb.linearDamping = slideDamp;
        }
        else if(state == MovementStates.Air)
        {
            rb.linearDamping = 0;
        }

        speedText.text = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).sqrMagnitude.ToString("00.0") + "\n" + state;

        if(moveDirection != Vector3.zero) playerModel.transform.DORotate(Quaternion.LookRotation(moveDirection).eulerAngles, 0.2f, RotateMode.Fast);
    }
    private void FixedUpdate()
    {
        MovePlayer();
        FloatOnGround();
        ApplyGravity();
        Fly();
    }

    private void StateHandler()
    {
        if (isFlying)
        {
            state = MovementStates.Flying;
            desiredMoveSpeed = flySpeed;
            if (currentMoveSpeed < flySpeed) currentMoveSpeed = flySpeed;
        }
        else if(isGrounded && rb.linearVelocity.magnitude > 0.1f)
        {
            if (isSliding && CheckOnSlope() && rb.linearVelocity.y < -0.1f) 
            {
                state = MovementStates.Sliding;
                desiredMoveSpeed = slideSpeed;
                
            } 
            else
            {
                state = MovementStates.Walking;
                desiredMoveSpeed = standardMoveSpeed;
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
        }

        float speedDifference = Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed);
        if (speedDifference > speedDiffToLerp && currentMoveSpeed != 0)
        {   
            StopAllCoroutines();
            StartCoroutine(SmoothLerpMoveSpeed());
        }
        else
        {
            currentMoveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - currentMoveSpeed);
        float startValue = currentMoveSpeed;

        while (time < difference)
        {
            currentMoveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (CheckOnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        currentMoveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        moveDirection = pInputs.moveDirRelativeToCam;

        if (CheckOnSlope())
        {
            rb.AddForce(GetSlopeMoveDir(moveDirection) * currentMoveSpeed * 10, ForceMode.Force);
        }
        else if(isGrounded)rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10, ForceMode.Force);
        else rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10 * airMovementMultiplier, ForceMode.Force);
    }

    private void Fly()
    {
        if(!isFlying && pInputs.flyPressed)
        {
            currentGravityForce = 0;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            isFlying = true;
        }
        else if(isFlying && !pInputs.flyPressed)
        {
            currentGravityForce = gravityForce;
            isFlying = false;
        }

        if (isFlying)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (CheckOnSlope())
        {
            if (rb.linearVelocity.magnitude > currentMoveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * currentMoveSpeed;
        }

        else
        {           
            if (flatVel.magnitude > currentMoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    public bool CheckOnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundRayDistance))
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

        if (rb.linearVelocity.y > maxFallSpeed) rb.AddForce(Vector3.down * currentGravityForce, ForceMode.Force);
        else rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxFallSpeed, rb.linearVelocity.z);
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
}
