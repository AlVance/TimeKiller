using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private float acc;
    [SerializeField] private float maxSpeed;
    private bool movePressed;
    private Vector2 moveDir;

    [Header("Ground & Gravity")]
    [SerializeField] private float rayDistance;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStength;
    [SerializeField] private float rideSpringDamper;
    private RaycastHit groundHit;


    private Rigidbody rb;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerControls.Enable();
        HandleInput();
    }
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        FloatOnGround();
        Movement();    
    }

    private void Movement()
    {
        if (movePressed)
        {
            rb.AddForce(new Vector3(moveDir.x, 0, moveDir.y) * acc);
            if (rb.linearVelocity.x > maxSpeed) rb.linearVelocity = new Vector3(maxSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            else if(rb.linearVelocity.x < -maxSpeed) rb.linearVelocity = new Vector3(-maxSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            if (rb.linearVelocity.z > maxSpeed) rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, maxSpeed);
            else if (rb.linearVelocity.z < -maxSpeed) rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, -maxSpeed);
            this.transform.rotation = Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.y));
        }
    }

    private void FloatOnGround()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out groundHit, rayDistance))
        {
            Debug.Log(groundHit.collider.gameObject.name);

            Vector3 vel = rb.linearVelocity;
            Vector3 rayDir = transform.TransformDirection(Vector3.down);

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitbody = groundHit.rigidbody;
            if(hitbody != null)
            {
                otherVel = hitbody.linearVelocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = groundHit.distance - rideHeight;
            float springForce = (x * rideSpringStength) - (relVel * rideSpringDamper);

            rb.AddForce(rayDir * springForce);

            Debug.DrawLine(this.transform.position, this.transform.position + (rayDir * springForce), Color.yellow);
        }
    }

    private void HandleInput()
    {
        playerInput.PlayerControls.Move.started += ctx =>
        {

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
