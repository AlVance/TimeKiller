using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    private PlayerInputs pInputs;

    private PlayerMovement pMovement;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Slide Variables")]
    [SerializeField] private float slideForce;
    [SerializeField] private float slideTimer;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private Transform slideDesiredDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pInputs = GetComponent<PlayerInputs>();
        pMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = pInputs.moveDirRelativeToCam;
        if (pInputs.driftPressed && !pMovement.isSliding && pMovement.state != PlayerMovement.MovementStates.Air)
        {
            StartSlide();
        }
        else if(!pInputs.driftPressed && pMovement.isSliding || pMovement.state == PlayerMovement.MovementStates.Air)
        {
            EndSlide();
        }
    }

    private void FixedUpdate()
    {
        Slide();
    }

    private void StartSlide()
    {
        pMovement.isSliding = true;

        playerModel.transform.localScale = new Vector3(playerModel.transform.localScale.x, 0.5f, playerModel.transform.localScale.z);
    }

    private void Slide()
    {
        if(pMovement.isSliding)
        {
            if (pMovement.CheckOnSlope() && rb.linearVelocity.y < -0.1f)
            {
                rb.AddForce(pMovement.GetSlopeMoveDir(moveDirection) * slideForce, ForceMode.Force);
            }
        }
    }

    private void EndSlide()
    {
        pMovement.isSliding = false;
        playerModel.transform.localScale = new Vector3(playerModel.transform.localScale.x, 1f, playerModel.transform.localScale.z);
    }

    private void OnDisable()
    {
        EndSlide();
    }
}
