using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;

public class JumpPlatformController : MonoBehaviour
{
    
    //[SerializeField] Vector3 Jumpdirection;
    [SerializeField] Transform JumpDirectionTr;
    [SerializeField] float Jumpspeed;
    
    [SerializeField] private bool blockPlayer = true;
    [SerializeField] private float blockInputTime = 0.05f;
    [SerializeField] private Animator platformAnim;
    [SerializeField] private bool forcePlayerToCenter = false;
    
    [SerializeField] private bool killMomentum = true;
    [SerializeField] private AudioSource jumpAS;
    private float basePich;
    private bool playerBlocked = false;
    [SerializeField] ParticleSystem particle;
    [SerializeField] private MMF_Player jumpFeedback;

    private PlayerInputs pInputs;
    private PlayerMovement pMovement;
    private Vector3 lastDir;
    private void Start()
    {
        basePich = jumpAS.pitch;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pMovement = other.GetComponent<PlayerMovement>();
            Rigidbody _rb = other.GetComponent<Rigidbody>();

            if (pMovement == null || _rb == null) return;

            pMovement.lastJumpPlatform = this;

            if (killMomentum)
            {
                pMovement.desiredMoveSpeed = pMovement.standardMoveSpeed;
                pMovement.currentMoveSpeed = pMovement.standardMoveSpeed;
                pMovement.extraForce = 0;
                _rb.linearVelocity = Vector3.zero;
                _rb.linearVelocity = Vector3.zero;
            }                
            else
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

            if (forcePlayerToCenter)
                other.transform.position = transform.position + transform.up * 0.75f;

            pMovement.ApplyExternalImpulse(JumpDirectionTr.up.normalized, Jumpspeed * 0.02f);
            


            if (blockPlayer)
            {
                StopAllCoroutines();
                StartCoroutine(_BlockPlayerOnJump(pMovement));
            }

            if (platformAnim != null) platformAnim.SetTrigger("On");
            if (jumpFeedback != null) jumpFeedback.PlayFeedbacks();
            if (particle != null) particle.Play();

            if (jumpAS != null)
            {
                jumpAS.pitch = basePich + Random.Range(-0.2f, 0.2f);
                jumpAS.Play();
            }
        }
    }

    private IEnumerator _BlockPlayerOnJump(PlayerMovement pM)
    {
        pM.movementBlocked = true;
        yield return new WaitForSeconds(blockInputTime);

        if (pM.lastJumpPlatform == this)
        {
            pM.movementBlocked = false;
        }
    }

}
