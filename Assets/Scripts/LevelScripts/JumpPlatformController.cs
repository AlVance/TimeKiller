using UnityEngine;

public class JumpPlatformController : MonoBehaviour
{
    
    //[SerializeField] Vector3 Jumpdirection;
    [SerializeField] Transform JumpDirectionTr;
    [SerializeField] float Jumpspeed;
    [SerializeField] ParticleSystem particle;
    [SerializeField] private float blockInputTime = 0.05f;
    [SerializeField] private Animator platformAnim;
    [SerializeField] private bool forcePlayerToCenter = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.gameObject.GetComponent<Rigidbody>().linearVelocity += Jumpdirection.normalized * Jumpspeed;
            Rigidbody _rb = other.gameObject.GetComponent<Rigidbody>();
            _rb.linearVelocity = new Vector3(0, 0, 0);
            if (forcePlayerToCenter) other.gameObject.transform.position = this.gameObject.transform.position + this.transform.up * 0.75f;
            //_rb.AddForce(Jumpdirection.normalized * Jumpspeed);
            other.gameObject.GetComponent<PlayerController>().BlockPlayer(blockInputTime);
            _rb.AddForce(JumpDirectionTr.up.normalized * Jumpspeed);
            platformAnim.SetTrigger("On");
            particle.Play();
        }
    }
}
