using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs pInputs;

    [Header("Movement variables")]
    [SerializeField] private float standardMoveSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float groundedDamp;
    [SerializeField] private float airMovementMultiplier;
    [SerializeField] private float speedDiffToLerp;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private float currentMoveSpeed;
    public bool isSliding = false;
    private Vector3 moveDirection;
    private Rigidbody rb;
    [SerializeField] private float speedIncreaseMultiplier;
    [SerializeField] private float slopeIncreaseMultiplier;

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
    private MovementStates state;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pInputs = GetComponent<PlayerInputs>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);

        StateHandler();

        SpeedControl();

        if (isGrounded)
        {
            rb.linearDamping = groundedDamp;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
        FloatOnGround();
        ApplyGravity();
    }

    private void StateHandler()
    {
         if(isGrounded && rb.linearVelocity != Vector3.zero)
        {
            if (!isSliding) 
            {
                state = MovementStates.Walking;
                desiredMoveSpeed = standardMoveSpeed;
            } 
            else
            {
                state = MovementStates.Sliding;
                desiredMoveSpeed = slideSpeed;
            }
        }
         else if(isGrounded && rb.linearVelocity == Vector3.zero)
        {
            state = MovementStates.Idle;
            desiredMoveSpeed = 0;
        }
        else if (!isGrounded)
        {
            state = MovementStates.Air;
        }
        
         if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > speedDiffToLerp && currentMoveSpeed != 0)
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

    private void SpeedControl()
    {
        if(rb.linearVelocity.magnitude > desiredMoveSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * desiredMoveSpeed;
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

        if (rb.linearVelocity.y > maxFallSpeed) rb.AddForce(Vector3.down * gravityForce, ForceMode.Force);
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
