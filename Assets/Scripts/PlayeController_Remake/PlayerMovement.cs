using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float maxAccelerationForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float decc;
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
        rb = this.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
    }
    private void FixedUpdate()
    {
        FloatOnGround();
        Movement();
        rb.linearVelocity += new Vector3(0, -1f, 0);
    }

    Vector3 m_GoalVel;
    private void Movement()
    {
        Vector3 unitGoal = new Vector3(moveDir.x, 0, moveDir.y);
        Vector3 goalVel = unitGoal * maxSpeed;

        m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, accelerationSpeed * Time.fixedDeltaTime);

        Vector3 neededAccel = (m_GoalVel - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z)) / Time.fixedDeltaTime;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccelerationForce);
        rb.AddForce(neededAccel * rb.mass);

        if(moveDir != Vector2.zero) this.transform.rotation = Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.y));
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

            Debug.DrawLine(this.transform.position, this.transform.position + (rayDir * springForce / 2), Color.yellow);
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
