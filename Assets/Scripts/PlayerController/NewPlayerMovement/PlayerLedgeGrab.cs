using UnityEngine;

public class PlayerLedgeGrab : MonoBehaviour
{
    private PlayerInputs pInputs;
    private Rigidbody rb;

    [SerializeField] private float upForce;
    [SerializeField] private float upRayDistance;
    [SerializeField] private LayerMask groundLayer;
    private bool UpRayHitted = false;
    [SerializeField] private Transform upRayTr;
    [SerializeField] private Transform parentModel;
    private RaycastHit ledgeGrabHit;
    [SerializeField] private Vector3 offset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pInputs = GetComponent<PlayerInputs>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLedgeGrab();
        //if (UpRayHitted && pInputs.moveDir.sqrMagnitude > 0.1f)
        //{
        //    this.transform.position = ledgeGrabHit.point + new Vector3(0, 1, 0);
        //}
        //
    }

    private void FixedUpdate()
    {
        if(UpRayHitted && pInputs.moveDir.sqrMagnitude > 0.1f)
        {
            rb.AddForce(this.transform.up * upForce, ForceMode.Force);
        }
    }

    private void CheckLedgeGrab()
    {
        if (Physics.Raycast(upRayTr.position, -upRayTr.up, out ledgeGrabHit, upRayDistance, groundLayer))
        {
            UpRayHitted = true;
        }
        else UpRayHitted = false;

        Debug.DrawRay(upRayTr.position, -upRayTr.up * upRayDistance, Color.green);
    }
}
