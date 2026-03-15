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
        if (other.gameObject.tag == "Player")
        {
            Rigidbody _rb = other.gameObject.GetComponent<Rigidbody>();
            pInputs = other.gameObject.GetComponent<PlayerInputs>();
            pMovement = other.gameObject.GetComponent<PlayerMovement>();

            if(killMomentum) _rb.linearVelocity = new Vector3(0, 0, 0);
            else _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

            if (blockPlayer)
            {
                if (other.gameObject.GetComponent<PlayerMovement>().lastJumpPlatform == this)
                {
                    StopCoroutine(_BlockPlayerOnJump(other.gameObject.GetComponent<PlayerBlock>(), other.gameObject.GetComponent<PlayerMovement>()));
                }
                StartCoroutine(_BlockPlayerOnJump(other.gameObject.GetComponent<PlayerBlock>(), other.gameObject.GetComponent<PlayerMovement>()));
            }

            if (forcePlayerToCenter) other.gameObject.transform.position = this.gameObject.transform.position + this.transform.up * 0.75f;

            _rb.AddForce(JumpDirectionTr.up.normalized * Jumpspeed, ForceMode.Impulse);

            platformAnim.SetTrigger("On");
            jumpFeedback.PlayFeedbacks();

            jumpAS.pitch = basePich + Random.Range(-0.2f, 0.2f);
            jumpAS.Play();

            other.gameObject.GetComponent<PlayerMovement>().lastJumpPlatform = this;
        }
    }

    private IEnumerator _BlockPlayerOnJump(PlayerBlock pB, PlayerMovement pM)
    {
        pMovement.movementBlocked = true;
        //pB.BlockPlayer();
        yield return new WaitForSeconds(blockInputTime);
        //if(pM.lastJumpPlatform == this)pB.UnblockPlayer();
        
        if (pM.lastJumpPlatform == this && pMovement.movementBlocked)
        {
            pMovement.movementBlocked = false;
            //if (pInputs.moveDirRelativeToCam == Vector3.zero) pInputs.moveDirRelativeToCam = lastDir;
        }
            

    }
   
}
