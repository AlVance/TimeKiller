using UnityEngine;

public class PlayerWallRun : MonoBehaviour
{
    private PlayerInputs pInputs;
    private Rigidbody rb;
    private PlayerMovement pMovement;

    [Header("WallRunning variables")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallRunForce;
    //[SerializeField] private float maxWallRunTime;
    //[SerializeField] private float minDistanceToGround;
    private float currentWallRunTimer;

    [SerializeField] private float wallCheckDistance;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private RaycastHit groundHit;
    public bool wallRight = false;
    public bool wallLeft = false;

    [Header("ExitWall")]
    [SerializeField] private float exitWallTime;
    private float currentExitWallTime = 0;
    private bool exitWall = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pInputs = GetComponent<PlayerInputs>();
        pMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckWallHit();

        if(wallRight || wallLeft && pInputs.moveDirRelativeToCam.sqrMagnitude > 0.1f && !exitWall)
        {
            if(!pMovement.isWallRunning)StartWallRun();
        }
        else
        {
            if(pMovement.isWallRunning) StopWallRun();
        }

        if (exitWall)
        {
            currentExitWallTime += Time.deltaTime;
            if(currentExitWallTime >= exitWallTime)
            {
                currentExitWallTime = 0;
                exitWall = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (pMovement.isWallRunning) WallRun();
    }

    private void CheckWallHit()
    {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, wallMask);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, wallMask);

        Physics.Raycast(transform.position, -transform.up, out groundHit, 100f, groundMask);
    }

    private void StartWallRun()
    {
        pMovement.isWallRunning = true;
        pMovement.currentGravityForce = 0;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }

    private void WallRun()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).sqrMagnitude > (transform.forward - -wallForward).sqrMagnitude)
            wallForward = -wallForward;

        if(pMovement.currentGravityForce != 0)
        {
            pMovement.currentGravityForce = 0;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        rb.AddForce(-wallNormal * 100, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pMovement.isWallRunning = false;
        pMovement.currentGravityForce = pMovement.gravityForce;
        exitWall = true;
    }
}
