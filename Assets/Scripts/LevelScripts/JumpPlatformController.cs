using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;

public class JumpPlatformController : MonoBehaviour
{
    
    //[SerializeField] Vector3 Jumpdirection;
    [SerializeField] Transform JumpDirectionTr;
    [SerializeField] float Jumpspeed;
    [SerializeField] ParticleSystem particle;
    [SerializeField] private MMF_Player jumpFeedback;
    [SerializeField] private float blockInputTime = 0.05f;
    [SerializeField] private Animator platformAnim;
    [SerializeField] private bool forcePlayerToCenter = false;
    [SerializeField] private AudioSource jumpAS;
    private float basePich;

    private void Start()
    {
        basePich = jumpAS.pitch;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.gameObject.GetComponent<Rigidbody>().linearVelocity += Jumpdirection.normalized * Jumpspeed;
            Rigidbody _rb = other.gameObject.GetComponent<Rigidbody>();
            _rb.linearVelocity = new Vector3(0, 0, 0);
            if (forcePlayerToCenter) other.gameObject.transform.position = this.gameObject.transform.position + this.transform.up * 0.75f;
            //_rb.AddForce(Jumpdirection.normalized * Jumpspeed);
            //other.gameObject.GetComponent<PlayerController>().BlockPlayer(blockInputTime, false);
            StartCoroutine(_BlockPlayerOnJump(other.gameObject.GetComponent<PlayerController>()));
            _rb.AddForce(JumpDirectionTr.up.normalized * Jumpspeed);
            platformAnim.SetTrigger("On");
            jumpFeedback.PlayFeedbacks();
            //particle.Play();

            jumpAS.pitch = basePich + Random.Range(-0.2f, 0.2f);
            jumpAS.Play();
        }
    }

    private IEnumerator _BlockPlayerOnJump(PlayerController pC)
    {
        pC.BlockPlayer(false);
        yield return new WaitForSeconds(blockInputTime);
        pC.UnblockPlayer();
    }
   
}
