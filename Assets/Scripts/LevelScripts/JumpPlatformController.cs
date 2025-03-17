using UnityEngine;

public class JumpPlatformController : MonoBehaviour
{
    //[SerializeField] Vector3 Jumpdirection;
    [SerializeField] Transform JumpDirectionTr;
    [SerializeField] float Jumpspeed;
    [SerializeField] ParticleSystem particle;
    [SerializeField] private float blockInputTime = 0.05f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.gameObject.GetComponent<Rigidbody>().linearVelocity += Jumpdirection.normalized * Jumpspeed;
            Rigidbody _rb = other.gameObject.GetComponent<Rigidbody>();
            _rb.linearVelocity = new Vector3(0, 0, 0);
            //_rb.AddForce(Jumpdirection.normalized * Jumpspeed);
            other.gameObject.GetComponent<PlayerController>().BlockPlayer(blockInputTime);
            _rb.AddForce(JumpDirectionTr.up.normalized * Jumpspeed);

            particle.Play();
        }
    }
}
