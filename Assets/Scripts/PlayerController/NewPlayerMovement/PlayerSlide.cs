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
    }

    private void FixedUpdate()
    {
        if (pMovement.CheckOnSlope() && rb.linearVelocity.y < 0.1f)
        {
            rb.AddForce(pMovement.GetSlopeMoveDir(moveDirection) * slideForce, ForceMode.Force);
            pMovement.isSliding = true;
        }
        else
        {
            pMovement.isSliding = false;
        }
    }
}
