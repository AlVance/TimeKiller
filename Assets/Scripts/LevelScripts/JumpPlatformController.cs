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
            other.gameObject.GetComponent<Rigidbody>().AddForce(Jumpdirection.normalized * Jumpspeed);
        }
    }
}
