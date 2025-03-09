using UnityEngine;

public class JumpPlatformController : MonoBehaviour
{
    [SerializeField] Vector3 Jumpdirection;
    [SerializeField] float Jumpspeed;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.gameObject.GetComponent<Rigidbody>().linearVelocity += Jumpdirection.normalized * Jumpspeed;
            Rigidbody _rb = other.gameObject.GetComponent<Rigidbody>();
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            _rb.AddForce(Jumpdirection.normalized * Jumpspeed);
        }
    }
}
